using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    static class Statuses
    {
        public const string ValidationErrorMessage = "Validation Error";
        public const string ValidationMessageDuplicateSidSameFile = "Duplicate Sid were found in this file.";
        public const string ValidationMessageDuplicateSidMultipleFiles = "Duplicate Sid were found in multiple files.";
        public const string FailedToLoad = "Failed to Load";
        public const string Created = "Created";
        public const string Updated = "Updated";
        public const string Deleted = "Deleted";
        public const string Loading = "Loading";
        public const string Loaded = "Loaded";
        public const string Deployed = "Deployed";
        public const string Fetched = "Fetched";

        public static DeploymentStatus GetFailedToFetch(string messageDetail) =>
            new DeploymentStatus("Failed to fetch", messageDetail, SeverityLevel.Error);

        public static AssetState FailedToLoadErrorAssetState(string detail)
            => new(FailedToLoad, detail, SeverityLevel.Error);

        public static AssetState ValidationErrorAssetState(ProjectAccessPolicyDeploymentException exception)
        {
            var severity = exception.Level.Equals(ProjectAccessPolicyDeploymentException.StatusLevel.Error)
                ? SeverityLevel.Error
                : SeverityLevel.Warning;

            return new AssetState(
                Statuses.ValidationErrorMessage,
                string.Join(" - ", exception.StatusDescription, exception.StatusDetail), severity);
        }
    }
}
