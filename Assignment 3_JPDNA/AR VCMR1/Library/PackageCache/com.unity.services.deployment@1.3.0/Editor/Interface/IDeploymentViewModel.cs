using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;

namespace Unity.Services.Deployment.Editor.Interface
{
    interface IDeploymentViewModel
    {
        IReadOnlyObservable<IDeploymentDefinitionViewModel> DeploymentDefinitions { get; }

        Task DeployItemsAsync(
            IEnumerable<IDeploymentItemViewModel> items,
            IEnumerable<int> itemsPerDeploymentDefinitions);
        Task DeployDefinitionsAsync(IEnumerable<IDeploymentDefinitionViewModel> definitions);
    }
}
