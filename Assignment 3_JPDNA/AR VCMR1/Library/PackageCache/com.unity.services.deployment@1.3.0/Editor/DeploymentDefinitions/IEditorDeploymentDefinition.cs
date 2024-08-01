using System.ComponentModel;
using Unity.Services.Deployment.Core.Model;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    interface IEditorDeploymentDefinition : IDeploymentDefinition, INotifyPropertyChanged
    {
    }
}
