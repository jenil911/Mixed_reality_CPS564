using System.Collections.Generic;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model
{
    interface IProjectAccessMerger
    {
        List<AccessControlStatement> MergeStatementsToDeploy(
            IReadOnlyList<AccessControlStatement> toCreate,
            IReadOnlyList<AccessControlStatement> toUpdate,
            IReadOnlyList<AccessControlStatement> toDelete,
            IReadOnlyList<AccessControlStatement> remoteStatements);
    }
}
