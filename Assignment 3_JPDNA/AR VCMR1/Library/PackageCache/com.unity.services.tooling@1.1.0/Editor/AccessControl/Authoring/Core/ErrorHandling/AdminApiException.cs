using System;
using System.Collections.Generic;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling
{
    // Exception encountered during a transaction with the Access Control backend
    class AdminApiException: ProjectAccessPolicyDeploymentException
    {
        string m_Description;
        string m_Detail;
        public override string StatusDescription => m_Description;
        public override string StatusDetail => m_Detail;
        public override StatusLevel Level => StatusLevel.Error;

        public AdminApiException(string description, string detail, Exception inner)
            : base(description, inner)
        {
            m_Description = description;
            m_Detail = detail;
            AffectedFiles = new List<IProjectAccessFile>();
        }

    }
}
