using System;
using Unity.Services.Deployment.Editor.DeploymentDefinitions;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;

namespace Unity.Services.Deployment.Editor.Interface
{
    interface IDeploymentDefinitionViewModel : IEditorDeploymentDefinition
    {
        IEditorDeploymentDefinition Model { get; }
        IReadOnlyObservable<IDeploymentItemViewModel> DeploymentItemViewModels { get; }
    }
}
