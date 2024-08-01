using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Results;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Deploy
{
    interface IProjectAccessDeploymentHandler
    {
        Task<DeployResult> DeployAsync(
            IReadOnlyList<IProjectAccessFile> files,
            bool dryRun = false,
            bool reconcile = false,
            CancellationToken cancellationToken = default);
    }
}
