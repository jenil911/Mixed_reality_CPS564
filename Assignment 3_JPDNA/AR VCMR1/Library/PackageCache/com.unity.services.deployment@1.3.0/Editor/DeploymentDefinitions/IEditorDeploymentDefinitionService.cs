using System;
using System.Collections.ObjectModel;
using Unity.Services.Deployment.Core;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    interface IEditorDeploymentDefinitionService : IDeploymentDefinitionService
    {
        IEditorDeploymentDefinition DefaultDefinition { get; }
        ObservableCollection<DeploymentDefinition> ObservableDeploymentDefinitions { get; }
        event Action DeploymentDefinitionPathChanged;
        event Action DeploymentDefinitionExcludePathsChanged;
        event Action DeploymentItemPathChanged;
    }
}
