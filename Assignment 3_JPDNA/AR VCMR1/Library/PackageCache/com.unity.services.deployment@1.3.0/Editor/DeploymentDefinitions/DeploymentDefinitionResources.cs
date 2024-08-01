using UnityEditor;
using UnityEngine;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    static class DeploymentDefinitionResources
    {
        public const string FileExtension = ".ddef";

        const string k_TexturePath = "DefaultAsset Icon";

        public static readonly Texture2D Icon = (Texture2D)EditorGUIUtility.IconContent(k_TexturePath).image;
    }
}
