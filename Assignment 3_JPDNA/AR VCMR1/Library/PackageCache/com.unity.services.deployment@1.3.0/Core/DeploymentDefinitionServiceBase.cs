using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GlobExpressions;
using Unity.Services.Deployment.Core.Model;

namespace Unity.Services.Deployment.Core
{
    abstract class DeploymentDefinitionServiceBase : IDeploymentDefinitionService
    {
        public abstract IReadOnlyList<IDeploymentDefinition> DeploymentDefinitions { get; }

        public virtual IDeploymentDefinition DefinitionForPath(string path)
        {
            if (path == null)
            {
                return null;
            }


            var dirPath = Path.GetDirectoryName(path) ?? "";

            var bestPath = string.Empty;

            IDeploymentDefinition bestDefinition = null;
            foreach (var definition in DeploymentDefinitions)
            {
                var definitionRootDir = Path.GetDirectoryName(definition.Path);
                if (!string.IsNullOrEmpty(definitionRootDir)
                    && dirPath.Contains(definitionRootDir)
                    && definitionRootDir.Length > bestPath.Length)
                {
                    bestDefinition = definition;
                    bestPath = definitionRootDir;
                }
            }

            return bestDefinition;
        }
    }
}
