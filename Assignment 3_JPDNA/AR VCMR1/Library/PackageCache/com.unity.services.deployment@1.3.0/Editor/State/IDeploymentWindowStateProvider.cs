using System;

namespace Unity.Services.Deployment.Editor.State
{
    interface IDeploymentWindowStateProvider : IDisposable
    {
        bool IsInternetConnected();
        bool IsSignedIn();
        bool IsEnvironmentSet();
        bool IsProjectLinked();
        bool ContainsDuplicateDeploymentDefinitions(out string error);
        bool AreSupportedPackagesInstalled();
    }
}
