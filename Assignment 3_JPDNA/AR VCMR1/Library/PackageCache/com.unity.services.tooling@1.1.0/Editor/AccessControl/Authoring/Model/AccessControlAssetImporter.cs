using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.Shared.DependencyInversion;
using UnityEditor.AssetImporters;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model
{
    [ScriptedImporter(1, ProjectAccessFileExtension.Extension)]
    class AccessControlAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var asset = AbstractRuntimeServices<AccessControlAuthoringServices>.Instance.GetService<ObservableAccessControlAssets>()
                .GetOrCreateInstance(ctx.assetPath);

            ctx.AddObjectToAsset("MainAsset", asset);
            ctx.SetMainObject(asset);
        }
    }
}
