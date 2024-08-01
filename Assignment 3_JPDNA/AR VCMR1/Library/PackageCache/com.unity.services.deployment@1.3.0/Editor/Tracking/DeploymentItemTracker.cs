using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.Deployment.Editor.Shared.Assets;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.Tracking
{
    class DeploymentItemTracker : IDeploymentItemTracker
    {
        readonly AssetPostprocessorProxy m_AssetPostprocessor;
        readonly Deployments m_Deployments;
        readonly List<IDeploymentItem> m_Items;

        public event Action<IDeploymentItem> ItemAdded;
        public event Action<IDeploymentItem> ItemChanged;
        public event Action<IDeploymentItem> ItemDeleted;

        public DeploymentItemTracker(AssetPostprocessorProxy assetPostprocessor, Deployments deployments)
        {
            m_AssetPostprocessor = assetPostprocessor;
            m_Deployments = deployments;
            m_Items = new List<IDeploymentItem>();

            m_AssetPostprocessor.AllAssetsPostprocessed += OnAllAssetsPostprocessed;
            SubscribeToDeploymentProviders();
        }

        void OnAllAssetsPostprocessed(object sender, PostProcessEventArgs e)
        {
            foreach (var addedPath in e.ImportedAssetPaths)
            {
                OnAssetImported(addedPath);
            }

            foreach (var movedAssetPath in e.MovedAssetPaths)
            {
                OnAssetImported(movedAssetPath);
            }

            foreach (var deletedPath in e.DeletedAssetPaths)
            {
                OnAssetDeleted(deletedPath);
            }
        }

        void OnAssetImported(string path)
        {
            var items = m_Items.Where(item => item.Path == path);

            foreach (var item in items)
            {
                ItemChanged?.Invoke(item);
            }
        }

        void OnAssetDeleted(string path)
        {
            var items = m_Items.Where(item => item.Path == path);

            foreach (var item in items)
            {
                ItemDeleted?.Invoke(item);
            }
        }

        void SubscribeToDeploymentProviders()
        {
            m_Deployments.DeploymentProviders.CollectionChanged += DeploymentProvidersOnCollectionChanged;

            foreach (var provider in m_Deployments.DeploymentProviders)
            {
                SubscribeToProvider(provider);
            }
        }

        void DeploymentProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged<DeploymentProvider>(e, SubscribeToProvider);
        }

        void SubscribeToProvider(DeploymentProvider provider)
        {
            provider.DeploymentItems.CollectionChanged += DeploymentItemsOnCollectionChanged;
            provider.DeploymentItems.ForEach(AddItem);
        }

        void DeploymentItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged<IDeploymentItem>(e, AddItem);
        }

        void AddItem(IDeploymentItem item)
        {
            if (m_Items.Contains(item))
            {
                return;
            }

            m_Items.Add(item);
            ItemAdded?.Invoke(item);
        }

        void OnCollectionChanged<T>(NotifyCollectionChangedEventArgs e, Action<T> onNewItem)
        {
            if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems == null)
            {
                return;
            }

            foreach (T item in e.NewItems)
            {
                onNewItem(item);
            }
        }

        public void Dispose()
        {
            foreach (var provider in m_Deployments.DeploymentProviders)
            {
                provider.DeploymentItems.CollectionChanged -= DeploymentItemsOnCollectionChanged;
            }

            m_AssetPostprocessor.AllAssetsPostprocessed -= OnAllAssetsPostprocessed;
        }
    }
}
