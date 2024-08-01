using UnityEditor;

namespace Unity.Services.Deployment.Editor
{
    static class DocumentationHelper
    {
        public static void OpenHelpDocumentation(string anchor = null)
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{Constants.PackageName}");
            var shortVersion = packageInfo.version.Substring(
                0,
                packageInfo.version.IndexOf('.', packageInfo.version.IndexOf('.') + 1));
            var anchorPostfix = anchor == null ? string.Empty : $"#{anchor}";
            var documentationUrl = $"https://docs.unity3d.com/Packages/{Constants.PackageName}@{shortVersion}{anchorPostfix}";
            Help.ShowHelpPage(documentationUrl);
        }
    }
}
