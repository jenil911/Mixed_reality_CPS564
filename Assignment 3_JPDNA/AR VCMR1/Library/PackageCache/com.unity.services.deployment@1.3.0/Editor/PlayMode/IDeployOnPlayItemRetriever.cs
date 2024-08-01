using System.Collections.Generic;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    interface IDeployOnPlayItemRetriever
    {
        IEnumerable<IDeploymentItem> GetItemsForDeployOnPlay();
    }
}
