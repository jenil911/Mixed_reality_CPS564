using System;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.Tracking
{
    interface IDeploymentItemTracker : IDisposable
    {
        event Action<IDeploymentItem> ItemAdded;
        event Action<IDeploymentItem> ItemChanged;
        event Action<IDeploymentItem> ItemDeleted;
    }
}
