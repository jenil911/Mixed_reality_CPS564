using System.Collections.Generic;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    interface IProjectAccessFile : IDeploymentItem
    {
        List<AccessControlStatement> Statements { get; set; }

        new float Progress { get; set; }
    }
}
