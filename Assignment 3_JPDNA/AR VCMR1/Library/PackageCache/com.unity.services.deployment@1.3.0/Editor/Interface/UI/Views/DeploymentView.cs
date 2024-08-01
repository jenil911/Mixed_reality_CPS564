using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.Deployment.Editor.Analytics;
using Unity.Services.Deployment.Editor.Commands;
using Unity.Services.Deployment.Core.Model;
using Unity.Services.Deployment.Editor.DeploymentDefinitions;
using Unity.Services.Deployment.Editor.Interface.UI.Components;
using Unity.Services.Deployment.Editor.Interface.UI.Events;
using Unity.Services.Deployment.Editor.Interface.UI.Serialization;
using Unity.Services.Deployment.Editor.Shared.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class DeploymentView : ViewBase
    {
        const string k_DeployCommandName = "Deploy";
        const string k_OpenCommandName = "Open";

        readonly CollectionBinding<IDeploymentDefinitionViewModel> m_DeploymentDefinitionBindings;

        IDeploymentViewModel m_DeploymentViewModel;
        IDeploymentWindowAnalytics m_DeploymentWindowAnalytics;
        IEditorDeploymentDefinitionService m_DeploymentDefinitionService;
        ICommandManager m_CommandManager;
        IKeyboardShortcuts m_KeyboardShortcuts;
        ISerializationManager m_SerializationManager;
        TreeViewElement m_TreeViewElement;
        StatusPanel m_StatusPanel;

        public enum ItemRetrieval
        {
            Selected,
            Checked
        }

        protected override string UxmlName => "DeploymentWindow_Deployment";

        public DeploymentView()
        {
            m_DeploymentDefinitionBindings = new CollectionBinding<IDeploymentDefinitionViewModel>(this);
            m_DeploymentDefinitionBindings.BindCollectionChanged(OnDefinitionObservableCollectionChanged);
        }

        public void Bind(
            IDeploymentViewModel deploymentViewModel,
            IDeploymentWindowAnalytics deploymentWindowAnalytics,
            IEditorDeploymentDefinitionService deploymentDefinitionService,
            ICommandManager commandManager,
            IKeyboardShortcuts keyboardShortcuts,
            ISerializationManager serializationManager)
        {
            m_DeploymentViewModel = deploymentViewModel;
            m_DeploymentWindowAnalytics = deploymentWindowAnalytics;
            m_DeploymentDefinitionService = deploymentDefinitionService;
            m_CommandManager = commandManager;
            m_KeyboardShortcuts = keyboardShortcuts;
            m_SerializationManager = serializationManager;
            BindGUI();
        }

        void BindGUI()
        {
            RegisterCallback<DeployAllClicked>(_ => DeployAllButtonOnClicked());
            RegisterCallback<DeploySelectedClicked>(_ => DeploySelectedButtonOnClicked());
            RegisterCallback<AttachToPanelEvent>(_ => OnAttachedToPanel());
            RegisterCallback<DetachFromPanelEvent>(_ => OnDetachedFromPanel());

            var statusLabel = this.Query<Label>(VisualElementNames.StatusLabel);
            m_StatusPanel = new StatusPanel(statusLabel);

            m_TreeViewElement = this.Query<TreeViewElement>().First();
            m_TreeViewElement.BindGUI(m_KeyboardShortcuts);
            m_TreeViewElement.OnSelectionChanged += OnDeploymentItemSelectionChanged;
            OnDeploymentItemSelectionChanged();

            m_DeploymentDefinitionBindings.Source = m_DeploymentViewModel.DeploymentDefinitions;

            m_SerializationManager.Bind(this);
            m_SerializationManager.ApplySerialization();
        }

        void OnDeploymentItemSelectionChanged()
        {
            var selectedItems = GetDeploymentViewsForDeployment(ItemRetrieval.Selected)
                .ToList();
            var lastSelected = selectedItems.LastOrDefault();
            if (lastSelected == null)
                m_StatusPanel.Clear();
            else
                m_StatusPanel.SelectedItem = lastSelected.Item;
        }

        async void DeploySelectedButtonOnClicked()
        {
            var checkedDeploymentItems = GetDeploymentViewsForDeployment(ItemRetrieval.Checked)
                .Select(vm => vm.Item)
                .ToList();
            var itemsPerDeploymentDefinitions = GetDeploymentDefinitionViews()
                .Where(view => view.Checked)
                .Select(view => view.ItemViews.Count)
                .ToList();

            await m_DeploymentViewModel.DeployItemsAsync(checkedDeploymentItems, itemsPerDeploymentDefinitions);
        }

        async void DeployAllButtonOnClicked()
        {
            await m_DeploymentViewModel.DeployDefinitionsAsync(m_DeploymentViewModel.DeploymentDefinitions);
        }

        void OnAttachedToPanel()
        {
            m_SerializationManager.Bind(this);
            m_SerializationManager.ApplySerialization();
        }

        void OnDetachedFromPanel()
        {
            m_SerializationManager.Unbind();
        }

        List<DeploymentDefinitionView> GetDeploymentDefinitionViews()
        {
            return this.Query<DeploymentDefinitionView>().ToList();
        }

        void BuildContextMenu(DeploymentElementViewBase viewBase, ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction(k_DeployCommandName, _ => OnItemDeployedFromContextMenuClicked(viewBase));

            var postCommandAction = new Dictionary<string, Action>
            {
                { k_OpenCommandName, () => OnContextMenuItemOpened(viewBase as DeploymentItemView) }
            };

            var modelSelection = GetSelection().ToList();
            var commands = m_CommandManager.GetCommandsForObjects(modelSelection);
            bool first = true;

            foreach (var command in commands)
            {
                var status = CommandUtils.GetMenuItemStatus(command, modelSelection);

                if (status == DropdownMenuAction.Status.Hidden)
                {
                    continue;
                }

                if (first)
                {
                    evt.menu.AppendSeparator();
                    first = false;
                }

                evt.menu.AppendAction(
                    command.Name,
                    _ =>
                    {
                        command.Execute(modelSelection);
                        if (postCommandAction.TryGetValue(command.Name, out Action action))
                            action();
                    },
                    status);
            }

            evt.StopPropagation();
        }

        internal async void OnItemDeployedFromContextMenuClicked(DeploymentElementViewBase itemView)
        {
            var selectedModels = GetDeploymentViewsForDeployment(ItemRetrieval.Selected)
                .Select(di => di.Item)
                .ToList();
            var itemsPerDeploymentDefinitions = GetDeploymentDefinitionViews()
                .Where(view => view.Checked)
                .Select(view => view.ItemViews.Count)
                .ToList();

            await m_DeploymentViewModel.DeployItemsAsync(selectedModels, itemsPerDeploymentDefinitions);
        }

        IEnumerable<object> GetSelection()
        {
            foreach (var deploymentDefinitionView in GetDeploymentDefinitionViews())
            {
                if (deploymentDefinitionView.Selected)
                {
                    yield return deploymentDefinitionView.DeploymentDefinition;
                }

                foreach (var div in deploymentDefinitionView.GetSelectedDeploymentItemViews())
                {
                    yield return div.Item.OriginalItem;
                }
            }
        }

        IEnumerable<DeploymentItemView> GetDeploymentViewsForDeployment(ItemRetrieval itemRetrieval)
        {
            return GetDeploymentDefinitionViews().SelectMany(item => item.GetDeploymentViewsForDeployment(itemRetrieval));
        }

        void OnDefinitionObservableCollectionChanged(
            IReadOnlyCollection<IDeploymentDefinitionViewModel> collection,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var toRemove = GetDeploymentDefinitionViews()
                    .Select(i => i.DeploymentDefinition)
                    .Where(m => m != m_DeploymentDefinitionService.DefaultDefinition);
                RemoveOldItems(toRemove);
                AddNewItems(collection);
            }
            else
            {
                RemoveOldItems(e.OldItems?.Cast<IDeploymentDefinitionViewModel>());
                AddNewItems(e.NewItems?.Cast<IDeploymentDefinitionViewModel>());
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                m_SerializationManager.ApplySerialization();
            }
        }

        void RemoveOldItems(IEnumerable<IDeploymentDefinitionViewModel> definitionViewModels)
        {
            if (definitionViewModels == null)
                return;

            foreach (var definitionViewModel in definitionViewModels)
            {
                RemoveDeploymentDefinition(definitionViewModel);
            }
        }

        void AddNewItems(IEnumerable<IDeploymentDefinitionViewModel> definitionViewModels)
        {
            if (definitionViewModels == null)
                return;

            foreach (var definitionViewModel in definitionViewModels)
            {
                AddDeploymentDefinition(definitionViewModel);
            }
        }

        DeploymentDefinitionView GetViewFromModel(IDeploymentDefinition definition)
        {
            return GetDeploymentDefinitionViews().First(x => x.DeploymentDefinition.Model == definition);
        }

        void AddDeploymentDefinition(IDeploymentDefinitionViewModel definitionViewModel)
        {
            var definitionView = new DeploymentDefinitionView();
            definitionView.Bind(definitionViewModel, definitionViewModel.Model == m_DeploymentDefinitionService.DefaultDefinition);
            definitionView.DoubleClickDeployed += (elementView) => OnDefinitionDeployedFromDoubleClick(elementView as DeploymentDefinitionView);
            definitionView.ContextMenuRequested += BuildContextMenu;
            definitionView.ItemAdded += item =>
            {
                item.ContextMenuRequested += BuildContextMenu;
                item.DoubleClickDeployed += viewBase => OnItemDeployedFromDoubleClick(viewBase as DeploymentItemView);
            };
            definitionView.ItemRemoved += item =>
            {
                item.ContextMenuRequested -= BuildContextMenu;
                if (item.Selected)
                {
                    m_StatusPanel.Clear();
                }
            };

            PlaceDefinitionViewInCorrectOrder(definitionView);
        }

        void PlaceDefinitionViewInCorrectOrder(DeploymentDefinitionView definitionView)
        {
            var index = definitionView.DeploymentDefinition.Model == m_DeploymentDefinitionService.DefaultDefinition
                ? 0
                : m_DeploymentDefinitionService.ObservableDeploymentDefinitions
                    .IndexOf((DeploymentDefinition)definitionView.DeploymentDefinition.Model) + 1;
            m_TreeViewElement.Insert(Mathf.Clamp(index, 0, m_TreeViewElement.childCount), definitionView);
        }

        async void OnDefinitionDeployedFromDoubleClick(DeploymentDefinitionView definitionView)
        {
            var itemModels = definitionView.ItemViews.Select(x => x.Item);
            var enumeratedItemModels = itemModels.ToList();
            await m_DeploymentViewModel.DeployItemsAsync(
                enumeratedItemModels,
                new List<int>() {enumeratedItemModels.Count()});
            m_DeploymentWindowAnalytics.SendDoubleClickEvent(definitionView.DeploymentDefinition.Path);
        }

        async void OnItemDeployedFromDoubleClick(DeploymentItemView itemView)
        {
            var itemModel = GetModelFromView(itemView);
            await m_DeploymentViewModel.DeployItemsAsync(
                new List<IDeploymentItemViewModel> {itemModel},
                new List<int>());
            m_DeploymentWindowAnalytics.SendDoubleClickEvent(itemModel.Path);
        }

        internal void OnContextMenuItemOpened(DeploymentItemView itemView)
        {
            var model = GetModelFromView(itemView);
            m_DeploymentWindowAnalytics.SendContextMenuOpenEvent(model.Path);
        }

        internal IDeploymentItemViewModel GetModelFromView(DeploymentItemView itemView)
        {
            return GetDeploymentDefinitionViews()
                .SelectMany(v => v.ItemViews)
                .First(x => x == itemView)
                .Item;
        }

        void RemoveDeploymentDefinition(IDeploymentDefinitionViewModel definitionViewModel)
        {
            var definitionView = GetViewFromModel(definitionViewModel.Model);
            m_TreeViewElement.Remove(definitionView);
        }

        internal static class VisualElementNames
        {
            public const string StatusLabel = "StatusLabel";
        }

        new class UxmlFactory : UxmlFactory<DeploymentView> {}
    }
}
