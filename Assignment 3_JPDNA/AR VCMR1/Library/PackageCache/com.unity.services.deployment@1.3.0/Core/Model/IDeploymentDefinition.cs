using System.Collections.ObjectModel;

namespace Unity.Services.Deployment.Core.Model
{
    internal interface IDeploymentDefinition
    {
        string Name { get; set; }
        string Path { get; set; }
        ObservableCollection<string> ExcludePaths { get; }
    }
}
