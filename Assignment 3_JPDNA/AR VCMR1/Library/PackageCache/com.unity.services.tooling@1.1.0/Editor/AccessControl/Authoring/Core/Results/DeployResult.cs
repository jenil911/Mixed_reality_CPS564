using System.Collections.Generic;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Results
{
    class DeployResult
    {
        public IReadOnlyList<AccessControlStatement> Created { get; set; }
        public IReadOnlyList<AccessControlStatement> Updated { get; set; }
        public IReadOnlyList<AccessControlStatement> Deleted { get; set; }
        public IReadOnlyList<IProjectAccessFile> Deployed { get; set; }
        public IReadOnlyList<IProjectAccessFile> Failed { get; set; }
    }
}
