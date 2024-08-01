using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Unity.Services.Deployment.Core;
using Unity.Services.Deployment.Core.Model;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.DeploymentApi.Editor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    sealed class EditorDeploymentDefinitionService :
        DeploymentDefinitionServiceBase,
        IEditorDeploymentDefinitionService,
        IDisposable
    {
        internal const string DefaultName = "Default";
        internal const string DefaultPath = "DefaultPath";

        public IEditorDeploymentDefinition DefaultDefinition { get; }
        public ObservableCollection<DeploymentDefinition> ObservableDeploymentDefinitions => m_DeploymentDefinitions;
        public override IReadOnlyList<IDeploymentDefinition> DeploymentDefinitions => m_DeploymentDefinitions.AsReadonly();
        readonly DeploymentDefinitionCollection m_DeploymentDefinitions = new DeploymentDefinitionCollection(true);

        ObservableCollection<DeploymentProvider> m_Providers;
        MergedObservableCollection<IDeploymentItem> m_MergedDeploymentItems;

        public event Action DeploymentDefinitionPathChanged;
        public event Action DeploymentDefinitionExcludePathsChanged;
        public event Action DeploymentItemPathChanged;

        public EditorDeploymentDefinitionService(
            ObservableCollection<DeploymentProvider> providers)
        {
            m_Providers = providers;
            m_Providers.CollectionChanged += ProvidersOnCollectionChanged;
            m_MergedDeploymentItems = new MergedObservableCollection<IDeploymentItem>(
                m_Providers.Select(p => p.DeploymentItems.AsReadonly()));
            m_MergedDeploymentItems.CollectionChanged += ItemsOnCollectionChanged;
            m_MergedDeploymentItems.ForEach(RegisterItemModified);

            var defaultDdef = ScriptableObject.CreateInstance<DeploymentDefinition>();
            defaultDdef.Name = DefaultName;
            defaultDdef.Path = DefaultPath;
            DefaultDefinition = defaultDdef;

            ObservableDeploymentDefinitions.CollectionChanged += ObservableDeploymentDefinitionsOnCollectionChanged;
        }

        public override IDeploymentDefinition DefinitionForPath(string path)
        {
            return base.DefinitionForPath(path) ?? DefaultDefinition;
        }

        void ProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<DeploymentProvider>()
                        .ForEach(p => m_MergedDeploymentItems.AddCollection(p.DeploymentItems.AsReadonly()));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count > 1)
                    {
                        throw new NotImplementedException("Can not remove more than 1 DeploymentProvider at a time");
                    }

                    e.OldItems.Cast<DeploymentProvider>()
                        .ForEach(p => m_MergedDeploymentItems.RemoveCollectionAt(e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_MergedDeploymentItems.ClearCollections();
                    m_MergedDeploymentItems = new MergedObservableCollection<IDeploymentItem>(
                        m_Providers.Select(p => p.DeploymentItems.AsReadonly()));
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException($"{nameof(EditorDeploymentDefinitionService)} does not support {e.Action}");
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<IDeploymentItem>()
                        .ForEach(i =>
                        {
                            UnregisterItemModified(i);
                            RegisterItemModified(i);
                        });
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<IDeploymentItem>()
                        .ForEach(UnregisterItemModified);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_MergedDeploymentItems.CollectionChanged -= ItemsOnCollectionChanged;
                    m_MergedDeploymentItems.ForEach(UnregisterItemModified);
                    m_MergedDeploymentItems = new MergedObservableCollection<IDeploymentItem>(
                        m_Providers.Select(p => p.DeploymentItems.AsReadonly()));
                    m_MergedDeploymentItems.CollectionChanged += ItemsOnCollectionChanged;
                    m_MergedDeploymentItems.ForEach(RegisterItemModified);
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException($"{nameof(EditorDeploymentDefinitionService)} does not support {e.Action}");
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void RegisterItemModified(IDeploymentItem deploymentItem)
        {
            deploymentItem.PropertyChanged += OnDeploymentItemPropertyChanged;
        }

        void UnregisterItemModified(IDeploymentItem deploymentItem)
        {
            deploymentItem.PropertyChanged -= OnDeploymentItemPropertyChanged;
        }

        void OnDeploymentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDeploymentItem.Path))
            {
                DeploymentItemPathChanged?.Invoke();
            }
        }

        void ObservableDeploymentDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    m_DeploymentDefinitions
                        .ForEach(d =>
                    {
                        d.PropertyChanged -= OnDeploymentDefinitionPropertyChanged;
                        d.ExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
                    });
                    break;
                default:
                    if (e.OldItems != null)
                    {
                        e.OldItems.Cast<DeploymentDefinition>()
                            .ForEach(d =>
                            {
                                d.PropertyChanged -= OnDeploymentDefinitionPropertyChanged;
                                d.ExcludePaths.CollectionChanged -= ObservableExcludePathsOnCollectionChanged;
                            });
                    }

                    if (e.NewItems != null)
                    {
                        e.NewItems.Cast<DeploymentDefinition>()
                            .ForEach(d =>
                            {
                                d.PropertyChanged += OnDeploymentDefinitionPropertyChanged;
                                d.ExcludePaths.CollectionChanged += ObservableExcludePathsOnCollectionChanged;
                            });
                    }
                    break;
            }
        }

        void ObservableExcludePathsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DeploymentDefinitionExcludePathsChanged?.Invoke();
        }

        void OnDeploymentDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDeploymentDefinition.Path))
            {
                DeploymentDefinitionPathChanged?.Invoke();
            }
        }

        public void Dispose()
        {
            Object.DestroyImmediate((DeploymentDefinition)DefaultDefinition);
            m_DeploymentDefinitions.Dispose();
        }
    }
}
