using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Validations
{
    class ProjectAccessConfigValidator : IProjectAccessConfigValidator
    {
        public List<AccessControlStatement> FilterNonDuplicatedAuthoringStatementsAndUpdateStatus(
            IReadOnlyList<IProjectAccessFile> files,
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var nonDuplicatedStatements =
                FilterNonDuplicatedAuthoringStatements(files, deploymentExceptions);

            UpdateInfoForDuplicateExceptions(deploymentExceptions,
                SetStatusDuplicateSidInSingleFile,
                SetStatusDuplicateSidInMultipleFiles);

            return nonDuplicatedStatements;
        }

        public void UpdateProjectAccessFileStates(IReadOnlyList<IProjectAccessFile> files)
        {
            var deploymentExceptions = new List<ProjectAccessPolicyDeploymentException>();
            FilterNonDuplicatedAuthoringStatements(files, deploymentExceptions);
            UpdateInfoForDuplicateExceptions(deploymentExceptions,
                SetStateDuplicateInSidInSingleFile,
                SetStateDuplicateSidInMultipleFiles);
        }

        public bool ValidateContent(
            IProjectAccessFile file,
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var validated = true;

            foreach (var authoringStatement in file.Statements)
            {
                var isSidValidated = ValidateSid(authoringStatement.Sid, (ProjectAccessFile)file, deploymentExceptions);
                var isResourceValidated = ValidateResource(authoringStatement.Resource, (ProjectAccessFile)file, deploymentExceptions);
                var isActionValidated = ValidateAction(authoringStatement.Action, (ProjectAccessFile)file, deploymentExceptions);
                var isPrincipalValidated = ValidatePrincipal(authoringStatement.Principal, (ProjectAccessFile)file, deploymentExceptions);
                var isEffectValidated = ValidateEffect(authoringStatement.Effect, (ProjectAccessFile)file, deploymentExceptions);

                if (!isSidValidated || !isResourceValidated || !isActionValidated || !isPrincipalValidated || !isEffectValidated)
                {
                    validated = false;
                }
            }

            return validated;
        }

        internal static List<AccessControlStatement> FilterNonDuplicatedAuthoringStatements(
            IReadOnlyList<IProjectAccessFile> files,
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var nonDuplicatedStatements = new List<AccessControlStatement>();

            foreach (var file in files)
            {
                foreach (var statement in file.Statements)
                {
                    var hasFailedValidation = false;
                    var containingFiles = files
                        .Where(paf => paf.Statements.Exists(fs => fs.Sid == statement.Sid))
                        .ToList();

                    var duplicatedStatementsInASameFileCount = file.Statements
                        .GroupBy(s => s.Sid).Count(t => t.Count() > 1);

                    if (duplicatedStatementsInASameFileCount > 0)
                    {
                        containingFiles.Add(file);
                        deploymentExceptions.Add(new DuplicateAuthoringStatementsException(statement.Sid, containingFiles));
                        hasFailedValidation = true;
                    }

                    if (!hasFailedValidation && containingFiles.Count > 1)
                    {
                        deploymentExceptions.Add(new DuplicateAuthoringStatementsException(statement.Sid, containingFiles));
                        hasFailedValidation = true;
                    }

                    if (!hasFailedValidation)
                    {
                        nonDuplicatedStatements.Add(statement);
                    }
                }
            }

            return nonDuplicatedStatements;
        }

        internal static void UpdateInfoForDuplicateExceptions(
            ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions,
            Action<IProjectAccessFile, DuplicateAuthoringStatementsException> onSingleFileSidDuplicate,
            Action<IProjectAccessFile, DuplicateAuthoringStatementsException> onMultipleFilesSidDuplicate)
        {
            var processedSids = new HashSet<string>();
            foreach (var deploymentException in deploymentExceptions)
            {
                if (deploymentException is DuplicateAuthoringStatementsException duplicateException)
                {
                    if (processedSids.Add(duplicateException.Sid))
                    {
                        foreach (var file in deploymentException.AffectedFiles.Distinct())
                        {
                            var thisFileCount = deploymentException.AffectedFiles.Count(x => x == file);
                            if (thisFileCount > 1)
                            {
                                onSingleFileSidDuplicate(file, duplicateException);
                            }
                            else
                            {
                                onMultipleFilesSidDuplicate(file, duplicateException);
                            }
                        }
                    }
                }
            }
        }

        internal static void SetStatusDuplicateSidInSingleFile(
            IProjectAccessFile file,
            DuplicateAuthoringStatementsException duplicateException)
        {
            file.Status = new DeploymentStatus(
                Statuses.ValidationErrorMessage,
                $"Sid '{duplicateException.Sid}' was found multiple times in this file. Sid must be a unique identifier for each statement.",
                SeverityLevel.Error);
        }

        internal static void SetStatusDuplicateSidInMultipleFiles(
            IProjectAccessFile file,
            DuplicateAuthoringStatementsException duplicateException)
        {
            file.Status = new DeploymentStatus(
                Statuses.ValidationErrorMessage,
                GetMultipleFilesDetail(duplicateException),
                SeverityLevel.Error);
        }

        internal static void SetStateDuplicateInSidInSingleFile(
            IProjectAccessFile file,
            DuplicateAuthoringStatementsException duplicateException)
        {
            file.States.Add(new AssetState(
                Statuses.ValidationMessageDuplicateSidSameFile,
                $"Sid '{duplicateException.Sid}' was found multiple times in this file. Sid must be a unique identifier for each statement.",
                SeverityLevel.Error));
        }

        internal static void SetStateDuplicateSidInMultipleFiles(
            IProjectAccessFile file,
            DuplicateAuthoringStatementsException duplicateException)
        {
            file.States.Add(new AssetState(
                Statuses.ValidationMessageDuplicateSidMultipleFiles,
                GetMultipleFilesDetail(duplicateException),
                SeverityLevel.Warning));
        }

        internal static string GetMultipleFilesDetail(DuplicateAuthoringStatementsException duplicateException)
        {
            var detail = new StringBuilder();
            detail.Append($"Sid '{duplicateException.Sid}' was found in these files: ");
            detail.Append(string.Join(
                ", ",
                duplicateException.AffectedFiles.Select(f => f.Name)));
            detail.Append(". Sid must be a unique identifier for each statement.");
            return detail.ToString();
        }

        static bool ValidateSid(string sid, ProjectAccessFile projectAccessFile, ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var regex = new Regex("^[A-Za-z0-9][A-Za-z0-9_-]{5,59}$", RegexOptions.CultureInvariant, matchTimeout: TimeSpan.FromSeconds(2));
            if (regex.Match(sid).Success) return true;

            deploymentExceptions.Add(new InvalidDataException(projectAccessFile, "Invalid value for Sid, must match a pattern of " + regex));
            projectAccessFile.Status = new DeploymentStatus("Validation Error", "Invalid value for Sid, must match a pattern of " + regex, SeverityLevel.Error);
            return false;
        }

        static bool ValidateResource(string resource, ProjectAccessFile projectAccessFile, ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var regex = new Regex("^urn:ugs:(([a-z-]*:){1}[*/]*[/a-z0-9-*]*|\\*{1})", RegexOptions.CultureInvariant, matchTimeout: TimeSpan.FromSeconds(2));
            if (regex.Match(resource).Success) return true;

            deploymentExceptions.Add(new InvalidDataException(projectAccessFile, "Invalid value for Resource, must match a pattern of " + regex));
            projectAccessFile.Status = new DeploymentStatus("Validation Error", "Invalid value for Resource, must match a pattern of " + regex, SeverityLevel.Error);
            return false;
        }

        static bool ValidateAction(List<string> action, ProjectAccessFile projectAccessFile, ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var validActions = new List<string> { "*", "Read", "Write", "Vivox:JoinMuted", "Vivox:JoinAllMuted" };
            var invalidActions = action.Where(v => !validActions.Contains(v));
            if (!invalidActions.Any()) return true;

            deploymentExceptions.Add(new InvalidDataException(projectAccessFile, "Invalid Value for Action, must be '*', 'Read', 'Write', 'Vivox:JoinMuted' or 'Vivox:JoinAllMuted'"));
            projectAccessFile.Status = new DeploymentStatus("Validation Error", "Invalid Value for Action, must be '*', 'Read', 'Write', 'Vivox:JoinMuted' or 'Vivox:JoinAllMuted'", SeverityLevel.Error);
            return false;
        }

        static bool ValidatePrincipal(string principal, ProjectAccessFile projectAccessFile, ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var validPrincipals = new List<string> { "Player" };
            if (validPrincipals.Contains(principal)) return true;

            deploymentExceptions.Add(new InvalidDataException(projectAccessFile, "Invalid Value for Principal, must be 'Player'"));
            projectAccessFile.Status = new DeploymentStatus("Validation Error", "Invalid Value for Principal, must be 'Player'", SeverityLevel.Error);
            return false;
        }

        static bool ValidateEffect(string effect, ProjectAccessFile projectAccessFile, ICollection<ProjectAccessPolicyDeploymentException> deploymentExceptions)
        {
            var validEffects = new List<string> { "Allow", "Deny" };
            if (validEffects.Contains(effect)) return true;

            deploymentExceptions.Add(new InvalidDataException(projectAccessFile, "Invalid Value for Effect, must be 'Allow' or 'Deny"));
            projectAccessFile.Status = new DeploymentStatus("Validation Error", "Invalid Value for Effect, must be 'Allow' or 'Deny", SeverityLevel.Error);
            return false;
        }
    }
}
