using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Unity.Services.Deployment.Core;
using Unity.Services.Deployment.Editor.DeploymentDefinitions;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.DeploymentApi.Editor;
using IoPath = System.IO.Path;

namespace Unity.Services.Deployment.Editor.Interface
{
    class DeploymentDefinitionViewModel : IDeploymentDefinitionViewModel, IDisposable
    {
        IEditorDeploymentDefinitionService m_DefinitionService;
        ObservableCollection<DeploymentProvider> m_Providers;
        ObservableCollection<IDeploymentItemViewModel> m_DeploymentItemViewModels;

        public IEditorDeploymentDefinition Model { get; }
        public IReadOnlyObservable<IDeploymentItemViewModel> DeploymentItemViewModels => m_DeploymentItemViewModels.AsReadonly();

        public string Name
        {
            get => Model.Name;
            set => Model.Name = value;
        }

        public string Path
        {
            get => Model.Path;
            set => Model.Path = value;
        }

        public ObservableCollection<string> ExcludePaths => Model.ExcludePaths;

        public event PropertyChangedEventHandler PropertyChanged;

        internal readonly Sync.Throttler ThrottleVerifyItems;

        public DeploymentDefinitionViewModel(
            IEditorDeploymentDefinition originalDefinition,
            IEditorDeploymentDefinitionService deploymentDefinitionService,
            ObservableCollection<DeploymentProvider> providers)
        {
            Model = originalDefinition;
            m_DefinitionService = deploymentDefinitionService;
            m_Providers = providers;

            m_DeploymentItemViewModels = new ObservableCollection<IDeploymentItemViewModel>();
            m_Providers.ForEach(AddItemsForProvider);
            ThrottleVerifyItems = new Sync.Throttler(VerifyItemContents, TimeSpan.FromMilliseconds(100), Sync.ThrottleOption.Debounce);

            m_Providers.CollectionChanged += ProvidersOnCollectionChanged;
            Model.PropertyChanged += DefinitionModelOnPropertyChanged;
            m_DefinitionService.DeploymentDefinitionPathChanged += ThrottleVerifyItems.Trigger;
            m_DefinitionService.DeploymentDefinitionExcludePathsChanged += ThrottleVerifyItems.Trigger;
            m_DefinitionService.DeploymentItemPathChanged += ThrottleVerifyItems.Trigger;
            m_DefinitionService.ObservableDeploymentDefinitions.CollectionChanged += DeploymentDefinitionsOnCollectionChanged;
        }

        public void Dispose()
        {
            m_Providers.CollectionChanged -= ProvidersOnCollectionChanged;
            Model.PropertyChanged -= DefinitionModelOnPropertyChanged;
            m_DefinitionService.DeploymentDefinitionPathChanged -= ThrottleVerifyItems.Trigger;
            m_DefinitionService.DeploymentDefinitionExcludePathsChanged -= ThrottleVerifyItems.Trigger;
            m_DefinitionService.DeploymentItemPathChanged -= ThrottleVerifyItems.Trigger;
            m_DefinitionService.ObservableDeploymentDefinitions.CollectionChanged -= DeploymentDefinitionsOnCollectionChanged;
        }

        void DeploymentDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ThrottleVerifyItems.Trigger();
            }
        }

        internal void ProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<DeploymentProvider>()
                        .ForEach(AddItemsForProvider);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count > 1)
                    {
                        throw new NotImplementedException("Can not remove more than 1 DeploymentProvider at a time");
                    }

                    var provider = e.OldItems.Cast<DeploymentProvider>().ToList()[0];
                    RemoveItemsForProvider(provider);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    m_Providers.ForEach(RemoveItemsForProvider);
                    m_DeploymentItemViewModels.Clear();
                    m_Providers.ForEach(AddItemsForProvider);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException($"{nameof(DeploymentViewModel)} does not support {e.Action}");
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void DeploymentItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var provider = m_Providers.First(p => p.DeploymentItems == (ObservableCollection<IDeploymentItem>)sender);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<IDeploymentItem>()
                        .ForEach(i => TryAddItemViewModel(i, provider));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<IDeploymentItem>()
                        .ForEach(TryRemoveItemViewModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_DeploymentItemViewModels.Clear();
                    AddItemsForProvider(provider);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException($"{nameof(DeploymentViewModel)} does not support {e.Action}");
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void DefinitionModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        void AddItemsForProvider(DeploymentProvider provider)
        {
            foreach (var item in provider.DeploymentItems)
            {
                TryAddItemViewModel(item, provider);
            }
            provider.DeploymentItems.CollectionChanged += DeploymentItemsOnCollectionChanged;
        }

        internal void TryAddItemViewModel(IDeploymentItem item, DeploymentProvider provider)
        {
            if (m_DefinitionService.DefinitionForPath(item.Path) == Model
                && !m_DefinitionService.IsPathExcludedByDeploymentDefinition(item.Path, Model))
            {
                AddItemViewModel(item, provider);
            }
        }

        internal void AddItemViewModel(IDeploymentItem item, DeploymentProvider provider)
        {
            var itemViewModel = new DeploymentItemViewModel(item, provider.Service);
            m_DeploymentItemViewModels.Add(itemViewModel);
        }

        void RemoveItemsForProvider(DeploymentProvider provider)
        {
            provider.DeploymentItems.CollectionChanged -= DeploymentItemsOnCollectionChanged;
            foreach (var item in provider.DeploymentItems)
            {
                TryRemoveItemViewModel(item);
            }
        }

        internal void TryRemoveItemViewModel(IDeploymentItem item)
        {
            var itemViewModel = m_DeploymentItemViewModels
                .FirstOrDefault(ivm => ivm.OriginalItem == item);
            if (itemViewModel != null)
            {
                RemoveItemViewModel(itemViewModel);
            }
        }

        void RemoveItemViewModel(IDeploymentItemViewModel itemViewModel)
        {
            m_DeploymentItemViewModels.Remove(itemViewModel);
        }

        internal void VerifyItemContents()
        {
            foreach (var provider in m_Providers)
            {
                foreach (var item in provider.DeploymentItems)
                {
                    var isOurs = m_DefinitionService.DefinitionForPath(item.Path) == Model;
                    var itemViewModel = m_DeploymentItemViewModels.FirstOrDefault(dvm => dvm.OriginalItem == item);
                    var itemViewModelIsInOurList = itemViewModel != null;
                    var isItemExcluded = m_DefinitionService.IsPathExcludedByDeploymentDefinition(item.Path, Model);

                    if ((!isOurs || isItemExcluded)
                        && itemViewModelIsInOurList)
                    {
                        RemoveItemViewModel(itemViewModel);
                    }
                    else if (isOurs
                             && !itemViewModelIsInOurList
                             && !isItemExcluded)
                    {
                        AddItemViewModel(item, provider);
                    }
                }
            }
        }
    }
}
