using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    [ScriptedImporter(1, DeploymentDefinitionResources.FileExtension)]
    class DeploymentDefinitionImporter : ScriptedImporter
    {
        public void OnValidate()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileContent = File.ReadAllText(ctx.assetPath);
            var definition = ScriptableObject.CreateInstance<DeploymentDefinition>();

            definition.FromJson(fileContent);

            ctx.AddObjectToAsset("MainAsset", definition, DeploymentDefinitionResources.Icon);
            ctx.SetMainObject(definition);
        }
    }
}
