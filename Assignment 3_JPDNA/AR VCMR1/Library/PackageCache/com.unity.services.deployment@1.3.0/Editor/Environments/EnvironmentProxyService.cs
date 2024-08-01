using System;
using Unity.Services.Core.Editor.Environments;
using IDwProvider = Unity.Services.DeploymentApi.Editor.IEnvironmentProvider;

namespace Unity.Services.Deployment.Editor.Environments
{
    // IEnvironment provider from deployment.api is now obsolete
    // until it is removed, create a proxy to allow the combination of the old provider and the new one
#pragma warning disable
    class EnvironmentProxyService : IDwProvider
#pragma warning restore
    {
        readonly IEnvironmentsApi m_EnvironmentsApi;

        public string Current => m_EnvironmentsApi.ActiveEnvironmentId.ToString();

        public EnvironmentProxyService(IEnvironmentsApi environmentsApi)
        {
            m_EnvironmentsApi = environmentsApi;
        }
    }
}
