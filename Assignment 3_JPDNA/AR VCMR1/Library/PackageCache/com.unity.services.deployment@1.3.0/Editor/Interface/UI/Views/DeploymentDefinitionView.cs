using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Unity.Services.Deployment.Editor.Interface.UI.Components;
using Unity.Services.Deployment.Editor.Interface.UI.Serialization;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using Unity.Services.Deployment.Editor.Shared.UI;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class DeploymentDefinitionView : DeploymentElementViewBase, ISerializableComponent
    {
        const string k_TemplatePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Templates/DeploymentDefinitionTemplate.uxml";

        public IDeploymentDefinitionViewModel DeploymentDefinition { get; private set; }

        readonly ModelBinding<IDeploymentDefinitionViewModel> m_ItemBindings;
        readonly CollectionBinding<IDeploymentItemViewModel> m_DeploymentItemBindings;
        readonly List<DeploymentItemView> m_ItemViews;
        bool m_IsDefault;
        string m_Path;
        State m_SortState;

        CollapseToggle m_CollapseToggle;
        CheckmarkToggle m_CheckmarkToggle;

        public string SerializationKey => DeploymentDefinition.Path;
        public object SerializationValue => new SerializationContainer(m_CheckmarkToggle.value, m_CollapseToggle.value);
        public List<DeploymentItemView> ItemViews => m_ItemViews;

        public event Action ValueChanged;
        public event Action<DeploymentItemView> ItemAdded;
        public event Action<DeploymentItemView> ItemRemoved;

        public DeploymentDefinitionView()
            : base(k_TemplatePath)
        {
            m_ItemBindings = new ModelBinding<IDeploymentDefinitionViewModel>(this);
            m_ItemBindings.BindProperty(nameof(DeploymentDefinitions.DeploymentDefinition.Name), def =>
            {
                this.Q<Label>(VisualElementNames.DefinitionName).text = def.Name;
            });
            m_ItemBindings.BindProperty(nameof(DeploymentDefinitions.DeploymentDefinition.Path), def =>
            {
                if (def.Path != null
                    && m_Path != def.Path)
                {
                    m_Path = def.Path;
                }
            });

            m_DeploymentItemBindings = new CollectionBinding<IDeploymentItemViewModel>(this);
            m_DeploymentItemBindings.BindCollectionChanged(OnItemObservableCollectionChanged);

            m_ItemViews = new List<DeploymentItemView>();
        }

        public void Bind(IDeploymentDefinitionViewModel definition, bool isDefault)
        {
            Model = definition;
            m_IsDefault = isDefault;
            DeploymentDefinition = definition;
            m_ItemBindings.Source = definition;

            m_CollapseToggle = this.Q<CollapseToggle>();
            m_CollapseToggle.ValueChanged += OnSerializableValueChanged;
            m_CheckmarkToggle = this.Q<CheckmarkToggle>();
            m_CheckmarkToggle.ValueChanged += OnSerializableValueChanged;

            m_DeploymentItemBindings.Source = DeploymentDefinition.DeploymentItemViewModels;

            RefreshVisibility();
        }

        public void ApplySerialization(object serializationValue)
        {
            if (serializationValue is SerializationContainer sc)
            {
                m_CheckmarkToggle.value = sc.Checkmark;
                m_CollapseToggle.value = sc.Collapse;
            }
        }

        void AddChild(DeploymentItemView itemView)
        {
            this.Q(VisualElementNames.ContainerElement).Add(itemView);

            TriggerSort();
            RefreshVisibility();
        }

        void OnItemObservableCollectionChanged(
            IReadOnlyCollection<IDeploymentItemViewModel> collection,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                RemoveOldItems(new List<DeploymentItemView>(m_ItemViews));
                AddNewItems(collection);
            }
            else
            {
                if (e.OldItems != null)
                {
                    RemoveOldItems(e.OldItems);
                }

                if (e.NewItems != null)
                {
                    AddNewItems(e.NewItems.Cast<IDeploymentItemViewModel>().ToList());
                }
            }
        }

        void AddNewItems(IReadOnlyCollection<IDeploymentItemViewModel> viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                AddDeploymentItem(viewModel);
            }
        }

        public void AddDeploymentItem(IDeploymentItemViewModel itemViewModel)
        {
            var itemView = new DeploymentItemView();
            itemView.Bind(itemViewModel);
            m_ItemViews.Add(itemView);
            AddChild(itemView);
            ItemAdded?.Invoke(itemView);
        }

        void RemoveOldItems(IEnumerable oldItems)
        {
            foreach (var oldItem in oldItems)
            {
                if (oldItem is DeploymentItemViewModel item)
                {
                    RemoveDeploymentItem(item);
                }
            }
        }

        public void RemoveDeploymentItem(IDeploymentItemViewModel viewModel)
        {
            var itemView = m_ItemViews.SingleOrDefault(iv => iv.Item == viewModel);
            if (itemView != null)
            {
                itemView.Unbind();
                m_ItemViews.Remove(itemView);
                RemoveChild(itemView);
                ItemRemoved?.Invoke(itemView);
            }
        }

        void TriggerSort()
        {
            if (m_SortState == State.Idle)
            {
                Sync.RunNextUpdateOnMain(() =>
                {
                    this.Q(VisualElementNames.ContainerElement).Sort((a, b) =>
                    {
                        var itemA = (DeploymentItemView)a;
                        var itemB = (DeploymentItemView)b;
                        var itemAName = itemA.Item.Name ?? itemA.Item.Service;
                        var itemBName = itemB.Item.Name ?? itemB.Item.Service;
                        return string.Compare(itemAName, itemBName, StringComparison.Ordinal);
                    });
                    m_SortState = State.Idle;
                });
                m_SortState = State.SortPending;
            }
        }

        void RemoveChild(DeploymentItemView itemView)
        {
            this.Q(VisualElementNames.ContainerElement).Remove(itemView);
            RefreshVisibility();
        }

        public IEnumerable<DeploymentItemView> GetDeploymentViewsForDeployment(DeploymentView.ItemRetrieval itemRetrieval)
        {
            if (itemRetrieval == DeploymentView.ItemRetrieval.Selected && Selected
                || itemRetrieval == DeploymentView.ItemRetrieval.Checked && Checked)
            {
                return m_ItemViews;
            }

            return itemRetrieval == DeploymentView.ItemRetrieval.Checked
                ? GetCheckedDeploymentItemViews()
                : GetSelectedDeploymentItemViews();
        }

        public IEnumerable<DeploymentItemView> GetSelectedDeploymentItemViews()
        {
            return m_ItemViews
                .Where(i => i.Selected);
        }

        public IEnumerable<DeploymentItemView> GetCheckedDeploymentItemViews()
        {
            return m_ItemViews
                .Where(i => i.Checked);
        }

        void OnSerializableValueChanged()
        {
            ValueChanged?.Invoke();
        }

        void RefreshVisibility()
        {
            if (m_IsDefault)
            {
                var hasItems = m_ItemViews.Any();
                visible = hasItems;
                style.display = hasItems
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }

        internal static class VisualElementNames
        {
            public const string DefinitionName = "DeploymentSetName";
            public const string ContainerElement = "DeploymentSetContainer";
        }

        internal class SerializationContainer
        {
            [JsonProperty("checkmark")]
            public bool Checkmark;
            [JsonProperty("collapse")]
            public bool Collapse;
            public SerializationContainer(bool checkmark, bool collapse)
            {
                Checkmark = checkmark;
                Collapse = collapse;
            }
        }

        new class UxmlFactory : UxmlFactory<DeploymentDefinitionView> {}

        enum State
        {
            Idle,
            SortPending
        }
    }
}
