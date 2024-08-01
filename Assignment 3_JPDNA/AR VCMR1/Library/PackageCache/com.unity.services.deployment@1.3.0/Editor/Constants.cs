using Unity.Services.Deployment.Editor.Shared.Infrastructure.IO;

namespace Unity.Services.Deployment.Editor
{
    static class Constants
    {
        public const string PackageName = "com.unity.services.deployment";
        internal static readonly string k_PackageRootPath = $"Packages/{PackageName}";
        internal static readonly string k_EditorRootPath = PathUtils.Join(k_PackageRootPath, "Editor");
    }
}
