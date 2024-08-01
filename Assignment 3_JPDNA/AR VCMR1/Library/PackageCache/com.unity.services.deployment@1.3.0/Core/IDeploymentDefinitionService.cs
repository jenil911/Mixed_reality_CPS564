using System.Collections.Generic;
using Unity.Services.Deployment.Core.Model;

namespace Unity.Services.Deployment.Core
{
    internal interface IDeploymentDefinitionService
    {
        IReadOnlyList<IDeploymentDefinition> DeploymentDefinitions { get; }
        IDeploymentDefinition DefinitionForPath(string path);
    }
}
