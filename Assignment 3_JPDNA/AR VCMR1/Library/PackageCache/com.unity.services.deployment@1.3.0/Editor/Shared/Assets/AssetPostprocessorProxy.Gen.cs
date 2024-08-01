// WARNING: Auto generated code. Modifications will be lost!
using System;
using UnityEditor;

namespace Unity.Services.Deployment.Editor.Shared.Assets
{
    class AssetPostprocessorProxy : AssetPostprocessor
    {
        static EventHandler<PostProcessEventArgs> s_AllAssetsPostprocessed;

        public virtual event EventHandler<PostProcessEventArgs> AllAssetsPostprocessed
        {
            add => s_AllAssetsPostprocessed += value;
            remove => s_AllAssetsPostprocessed -= value;
        }

        static void OnPostprocessAllAssets(
            string[] importedAssetPaths,
            string[] deletedAssetPaths,
            string[] movedAssetPaths,
            string[] movedFromAssetPaths,
            bool didDomainReload)
        {
            s_AllAssetsPostprocessed?.Invoke(null, new PostProcessEventArgs
            {
                ImportedAssetPaths = importedAssetPaths,
                DeletedAssetPaths = deletedAssetPaths,
                MovedAssetPaths = movedAssetPaths,
                MovedFromAssetPaths = movedFromAssetPaths,
                DidDomainReload = didDomainReload
            });
        }
    }
}
