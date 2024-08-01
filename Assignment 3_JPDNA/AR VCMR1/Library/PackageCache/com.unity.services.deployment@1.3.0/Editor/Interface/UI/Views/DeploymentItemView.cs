using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Unity.Services.Deployment.Editor.Interface.UI.Components;
using Unity.Services.Deployment.Editor.Interface.UI.Events;
using Unity.Services.Deployment.Editor.Interface.UI.Serialization;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.Deployment.Editor.Shared.UI;
using Unity.Services.DeploymentApi.Editor;
using UnityEngine.UIElements;
using ProgressBar = UnityEngine.UIElements.ProgressBar;
using SeverityLevel = Unity.Services.DeploymentApi.Editor.SeverityLevel;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class DeploymentItemView : DeploymentElementViewBase, ISerializableComponent
    {
        const string k_TemplatePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Templates/DeploymentItemTemplate.uxml";
        const string k_WarningClassName = "warning";
        const string k_ErrorClassName = "error";
        const string k_DelayDisplayName = "delay_display";
        const string k_DeployStateName = "deploy";

        CheckmarkToggle m_CheckmarkToggle;
        VisualElement m_ItemStateContainer;
        readonly List<DeploymentItemStateView> m_StateViews;

        string m_StatusClass;
        readonly ModelBinding<IDeploymentItemViewModel> m_ItemBindings;
        readonly Dictionary<SeverityLevel, string> m_DeploymentSeverityLevelToClassName;


        public IDeploymentItemViewModel Item { get; private set; }
        public string SerializationKey => SerializationContainer.CreateKey(Item.Name, Item.Path);
        public object SerializationValue =>
            new SerializationContainer(m_CheckmarkToggle.value, m_StatusClass, Item.States.ToList());
        public event Action ValueChanged;

        public DeploymentItemView()
            : base(k_TemplatePath)
        {
            m_ItemBindings = new ModelBinding<IDeploymentItemViewModel>(this);
            m_StateViews = new List<DeploymentItemStateView>();

            m_ItemBindings.BindProperty(nameof(Item.Name), item =>
            {
                var itemNameLabel = this.Q<Label>(VisualElementNames.ItemName);
                itemNameLabel.text = item.Name;
            });
            m_ItemBindings.BindProperty(nameof(ITypedItem.Type), item =>
            {
                var itemTypeLabel = this.Q<Label>(VisualElementNames.ItemType);
                itemTypeLabel.text = (item.OriginalItem as ITypedItem)?.Type ?? item.Service;
            });
            m_ItemBindings.BindProperty(nameof(Item.Progress), item =>
            {
                var progressBar = this.Q<ProgressBar>();
                progressBar.value = item.Progress;
                progressBar.title = $"{item.Progress:0.##}%";
            });
            m_ItemBindings.BindProperty(nameof(Item.Status), item =>
            {
                if (m_StatusClass == k_DeployStateName && item.Status.MessageSeverity == SeverityLevel.Success)
                {
                    AddToClassList(k_DelayDisplayName);
                }
                else
                {
                    RemoveFromClassList(k_DelayDisplayName);
                }

                SetStatus(item.Status);
            });

            m_DeploymentSeverityLevelToClassName = new Dictionary<SeverityLevel, string>()
            {
                { SeverityLevel.Warning, "modified" },
                { SeverityLevel.Info, "info" },
                { SeverityLevel.Success, "up-to-date" },
                { SeverityLevel.Error, "deploy-error" },
                { SeverityLevel.None, ""}
            };
        }

        public void Bind(IDeploymentItemViewModel item)
        {
            Item = item;
            Model = item;
            m_ItemBindings.Source = item;

            SetStatus(Item.Status);

            m_CheckmarkToggle = this.Q<CheckmarkToggle>();
            m_CheckmarkToggle.ValueChanged += OnSerializableValueChanged;

            this.Q<Label>(VisualElementNames.ItemService).text = item.Service;
            this.Q<Label>(VisualElementNames.ItemType).text = (item.OriginalItem as ITypedItem)?.Type ?? item.Service;
            m_ItemStateContainer = this.Q<VisualElement>(name: VisualElementNames.ItemStateContainer);

            Item.States.CollectionChanged += OnItemStateCollectionChanged;
            Item.States.ForEach(AddAssetStateView);
            Item.DeploymentStateChanged += OnDeploymentStateChanged;
            Item.PropertyChanged += OnItemPropertyChanged;
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Item.Path))
            {
                RebuildTreeEvent.Send(this);
            }
        }

        void OnDeploymentStateChanged(bool deploying)
        {
            if (deploying)
            {
                m_StatusClass = k_DeployStateName;
                SetStatusClass();
            }
        }

        public void Unbind()
        {
            Item.DeploymentStateChanged -= OnDeploymentStateChanged;
            Item.States.CollectionChanged -= OnItemStateCollectionChanged;

            m_ItemBindings.Source = null;
            Item = null;
            Model = null;
        }

        internal void OnItemStateCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                m_StateViews.ForEach(stateView => m_ItemStateContainer.Remove(stateView));
                m_StateViews.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var oldItem in e.OldItems)
                    {
                        var assetState = (AssetState)oldItem;
                        var viewIndex = m_StateViews
                            .FindIndex(itemStateView => itemStateView.ItemState.Equals(assetState));

                        if (viewIndex >= 0)
                        {
                            m_ItemStateContainer.Remove(m_StateViews[viewIndex]);
                            m_StateViews.RemoveAt(viewIndex);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var newItem in e.NewItems)
                    {
                        var assetState = (AssetState)newItem;
                        AddAssetStateView(assetState);
                    }
                }
            }

            OnSerializableValueChanged();
        }

        void AddAssetStateView(AssetState assetState)
        {
            var itemStateView = new DeploymentItemStateView();
            itemStateView.Bind(assetState);
            m_ItemStateContainer.Add(itemStateView);
            m_StateViews.Add(itemStateView);
        }

        public void ApplySerialization(object serializationValue)
        {
            // If this fails, it means that payload was changed and the previous
            // payload is incompatible.
            if (serializationValue is SerializationContainer sc)
            {
                m_CheckmarkToggle.value = sc.Checkmark;

                //Need to make sure that we are not saving status across editor sessions.
                if (!string.IsNullOrEmpty(Item.OriginalItem.Status.Message))
                {
                    m_StatusClass = sc.StatusClass;
                    SetStatusClass();
                }

                ApplyAssetStates(sc.States);
                return;
            }

            m_CheckmarkToggle.value = false;
            m_StatusClass = string.Empty;
        }

        internal void OnSerializableValueChanged()
        {
            ValueChanged?.Invoke();
        }

        protected override void OnClick(ClickEvent click)
        {
            base.OnClick(click);

            if (click.clickCount == 2)
            {
                click.StopPropagation();
            }
        }

        void SetStatus(DeploymentStatus status)
        {
            if (Item.IsBeingDeployed &&
                status.MessageSeverity != SeverityLevel.Error &&
                status.MessageSeverity != SeverityLevel.Success)
            {
                return;
            }

            SetStatusClass(status);
            var itemStatusLabel = this.Q<Label>(VisualElementNames.ItemStatus);
            itemStatusLabel.text = status.Message;

            SetStatusIcon(status.MessageSeverity);
        }

        void SetStatusIcon(SeverityLevel severityLevel)
        {
            m_StatusClass = m_DeploymentSeverityLevelToClassName[severityLevel];
            SetStatusClass();
            OnSerializableValueChanged();
        }

        void SetStatusClass()
        {
            var containsDelay = ClassListContains(k_DelayDisplayName);

            ClearClassList();

            if (containsDelay)
            {
                AddToClassList(k_DelayDisplayName);
            }

            AddToClassList(m_StatusClass);
        }

        void SetStatusClass(DeploymentStatus status)
        {
            RemoveFromClassList(k_ErrorClassName);
            RemoveFromClassList(k_WarningClassName);

            switch (status.MessageSeverity)
            {
                case SeverityLevel.Error:
                    AddToClassList(k_ErrorClassName);
                    break;
                case SeverityLevel.Warning:
                    AddToClassList(k_WarningClassName);
                    break;
            }
        }

        void ApplyAssetStates(List<AssetState> states)
        {
            if (states == null) return;

            foreach (var assetState in states)
            {
                if (!Item.States.Any())
                {
                    Item.States.Add(assetState);
                }
            }
        }

        new class UxmlFactory : UxmlFactory<DeploymentItemView> {} //NOSONAR

        internal static class VisualElementNames
        {
            public const string ItemName = "ItemName";
            public const string ItemStatus = "ItemStatus";
            public const string ItemStatusIcon = "ItemStatusIcon";
            public const string ItemService = "ItemService";
            public const string ItemType = "ItemType";
            public const string ItemStateContainer = "ItemStateContainer";
        }

        internal class SerializationContainer
        {
            [JsonProperty("checkmark")]
            public bool Checkmark;
            [JsonProperty("statusClass")]
            public string StatusClass;
            [JsonProperty("states")]
            public List<AssetState> States;
            public SerializationContainer(bool checkmark, string statusClass, List<AssetState> states)
            {
                Checkmark = checkmark;
                StatusClass = statusClass;
                States = states;
            }

            public static string CreateKey(string itemName, string itemPath)
            {
                return Path.Join(itemName, itemPath);
            }

            public static (string, string) DisassembleKey(string key)
            {
                var name = key;
                var path = string.Empty;

                if (key.Contains(Path.DirectorySeparatorChar))
                {
                    var indexOfSeparator = key.IndexOf(Path.DirectorySeparatorChar);
                    name = key.Substring(0, indexOfSeparator);
                    path = key.Substring(indexOfSeparator + 1);
                }

                return new ValueTuple<string, string>(name, path);
            }
        }
    }
}
