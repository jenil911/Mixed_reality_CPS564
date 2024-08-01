using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Validations;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.IO;
using Unity.Services.Tooling.Editor.Shared.Assets;
using Unity.Services.Tooling.Editor.Shared.Infrastructure.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model
{
    /// <summary>
    /// This class serves to track creation and deletion of assets of the
    /// associated service type
    /// </summary>
    sealed class ObservableAccessControlAssets : ObservableCollection<IDeploymentItem>, IDisposable
    {
        readonly IAccessControlResourcesLoader m_ResourceLoader;
        readonly IProjectAccessConfigValidator m_AccessConfigValidator;
        readonly ObservableAssets<AccessControlAsset> m_AccessControlAssets;
        public ObservableCollection<IDeploymentItem> DeploymentItems { get; } = new ObservableCollection<IDeploymentItem>();

        public ObservableAccessControlAssets(
            IAccessControlResourcesLoader resourceLoader,
            IProjectAccessConfigValidator accessConfigValidator)
        {
            m_ResourceLoader = resourceLoader;
            m_AccessConfigValidator = accessConfigValidator;
            m_AccessControlAssets = new ObservableAssets<AccessControlAsset>();

            foreach (var asset in m_AccessControlAssets)
            {
                OnNewAsset(asset);
                DeploymentItems.Add(asset.ResourceDeploymentItem);
            }

            m_AccessControlAssets.CollectionChanged += AccessControlAssetsOnCollectionChanged;
        }

        public void Dispose()
        {
            m_AccessControlAssets.CollectionChanged -= AccessControlAssetsOnCollectionChanged;
        }

        void OnNewAsset(AccessControlAsset asset)
        {
            PopulateModel(asset);
            Add(asset.ResourceDeploymentItem);
        }

        void PopulateModel(AccessControlAsset asset)
        {
            asset.ClearOwnedStates();
            asset.ResourceDeploymentItem.Progress = 0;
            asset.ResourceDeploymentItem.Status = default;

            m_ResourceLoader.LoadFileAsync(asset.ResourceDeploymentItem, default).GetAwaiter().GetResult();
        }

        void AccessControlAssetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            m_AccessControlAssets.ForEach(asset => asset.ClearOwnedStates());

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems.Cast<AccessControlAsset>())
                {
                    DeploymentItems.Remove(oldItem.ResourceDeploymentItem);
                    Remove(oldItem.ResourceDeploymentItem);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems.Cast<AccessControlAsset>())
                {
                    DeploymentItems.Add(newItem.ResourceDeploymentItem);
                    OnNewAsset(newItem);
                }
            }

            UpdateAllAssetStates();
        }

        void UpdateAllAssetStates()
        {
            var projectAccessFiles = new List<IProjectAccessFile>();
            foreach (var asset in m_AccessControlAssets)
            {
                projectAccessFiles.Add(asset.ResourceDeploymentItem);

                var exceptions = new List<ProjectAccessPolicyDeploymentException>();
                m_AccessConfigValidator.ValidateContent(asset.ResourceDeploymentItem, exceptions);

                exceptions.ForEach(e =>
                    asset.ResourceDeploymentItem.States.Add(Statuses.ValidationErrorAssetState(e)));
            }

            m_AccessConfigValidator.UpdateProjectAccessFileStates(projectAccessFiles);
        }

        AccessControlAsset RegenAsset(AccessControlAsset asset)
        {
            var newAsset = ScriptableObject.CreateInstance<AccessControlAsset>();
            newAsset.ResourceDeploymentItem = asset.ResourceDeploymentItem;
            asset = newAsset;
            PopulateModel(asset);
            foreach (var accessControlAsset in m_AccessControlAssets)
            {
                if (asset != accessControlAsset)
                {
                    accessControlAsset.ClearOwnedStates();
                }
            }
            UpdateAllAssetStates();

            return asset;
        }

        public AccessControlAsset GetOrCreateInstance(string assetPath)
        {
            foreach (var a in m_AccessControlAssets)
            {
                if (assetPath == a.Path)
                {
                    return a == null ? RegenAsset(a) : a;
                }
            }

            var asset = ScriptableObject.CreateInstance<AccessControlAsset>();
            asset.Path = assetPath;
            return asset;
        }

        public static bool IsValidationError(string error)
            => error.Equals(Statuses.ValidationErrorMessage)
            || error.Equals(Statuses.ValidationMessageDuplicateSidSameFile);

        public static bool IsValidationWarning(string error)
            => error.Equals(Statuses.ValidationMessageDuplicateSidMultipleFiles);

        public static bool IsLoadError(string error)
            => error.Equals(Statuses.FailedToLoad);
    }
}
