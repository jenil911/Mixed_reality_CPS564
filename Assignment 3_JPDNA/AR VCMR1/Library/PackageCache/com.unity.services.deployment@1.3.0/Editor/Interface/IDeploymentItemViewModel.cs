using System;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.Interface
{
    interface IDeploymentItemViewModel : IDeploymentItem
    {
        event Action<bool> DeploymentStateChanged;
        bool IsBeingDeployed { get; set; }
        string Service { get; }
        IDeploymentItem OriginalItem { get; }
    }
}
