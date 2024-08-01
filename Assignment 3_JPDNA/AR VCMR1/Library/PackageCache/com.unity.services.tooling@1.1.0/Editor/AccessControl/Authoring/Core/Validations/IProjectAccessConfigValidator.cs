using System.Collections.Generic;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Validations
{
    interface IProjectAccessConfigValidator
    {
        List<AccessControlStatement> FilterNonDuplicatedAuthoringStatementsAndUpdateStatus(
            IReadOnlyList<IProjectAccessFile> files,
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions);

        void UpdateProjectAccessFileStates(IReadOnlyList<IProjectAccessFile> files);

        bool ValidateContent(
            IProjectAccessFile file,
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions);
    }
}
