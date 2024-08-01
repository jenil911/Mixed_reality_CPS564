using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Analytics;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Deploy;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.ErrorHandling;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.IO;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Service;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Validations;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.IO;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model;
using Unity.Services.Tooling.Editor.Shared.Infrastructure.Collections;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Deployment
{
    class DeployCommand : Command<IProjectAccessFile>
    {
        readonly IProjectAccessDeploymentHandler m_DeploymentHandler;
        readonly IProjectAccessClient m_Client;
        readonly IEnvironmentsApi m_EnvironmentsApi;
        readonly IAccessControlResourcesLoader m_AccessControlResourceLoader;
        readonly IProjectIdProvider m_ProjectIdProvider;
        readonly IProjectAccessConfigValidator m_AccessConfigValidator;
        readonly IAccessControlEditorAnalytics m_Analytics;

        public override string Name => L10n.Tr("Deploy");

        public DeployCommand(
            IProjectAccessDeploymentHandler moduleDeploymentHandler,
            IProjectAccessClient client,
            IEnvironmentsApi environmentsApi,
            IAccessControlResourcesLoader resourcesLoader,
            IProjectIdProvider projectIdProvider,
            IProjectAccessConfigValidator accessConfigValidator,
            IAccessControlEditorAnalytics accessControlEditorAnalytics)
        {
            m_DeploymentHandler = moduleDeploymentHandler;
            m_Client = client;
            m_EnvironmentsApi = environmentsApi;
            m_AccessControlResourceLoader = resourcesLoader;
            m_ProjectIdProvider = projectIdProvider;
            m_AccessConfigValidator = accessConfigValidator;
            m_Analytics = accessControlEditorAnalytics;
        }

        public override async Task ExecuteAsync(IEnumerable<IProjectAccessFile> items, CancellationToken cancellationToken = default)
        {
            var itemList = items.ToList();

            m_Client.Initialize(m_EnvironmentsApi.ActiveEnvironmentId.ToString(), m_ProjectIdProvider.ProjectId, cancellationToken);

            var projectAccessFiles = itemList
                .Where(file => file.States
                    .All(state => state.Level != SeverityLevel.Error))
                .ToList()
                .AsReadOnly();

            OnPreDeploy(projectAccessFiles);

            await m_DeploymentHandler.DeployAsync(projectAccessFiles, false, false, cancellationToken);

            projectAccessFiles.ForEach(f =>
            {
                var exception = f.Status.MessageSeverity == SeverityLevel.Error ? f.Status.MessageDetail : "";
                m_Analytics.SendEvent("access_control_file_deployed", exception: exception);
            });
        }

        internal void OnPreDeploy(IReadOnlyList<IProjectAccessFile> items)
        {
            foreach (var i in items)
            {
                i.Progress = 33f;
                i.Status = new DeploymentStatus();
                i.States.Clear();
            }
        }
    }
}
