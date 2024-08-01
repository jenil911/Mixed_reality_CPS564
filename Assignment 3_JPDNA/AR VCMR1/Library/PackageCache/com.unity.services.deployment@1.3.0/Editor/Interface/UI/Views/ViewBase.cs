using Unity.Services.Deployment.Editor.Shared.Infrastructure.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    abstract class ViewBase : VisualElement
    {
        static readonly string s_DeploymentWindowAssetsPath = PathUtils.Join(Constants.k_EditorRootPath, "Interface", "UI", "Assets", "DeploymentWindowStateTemplates");

        protected abstract string UxmlName { get; }

        protected ViewBase()
        {
            CreateVisualTree();
        }

        void CreateVisualTree()
        {
            var assetPath = PathUtils.Join(s_DeploymentWindowAssetsPath, $"{UxmlName}.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
            visualTree.CloneTree(this);
        }
    }
}
