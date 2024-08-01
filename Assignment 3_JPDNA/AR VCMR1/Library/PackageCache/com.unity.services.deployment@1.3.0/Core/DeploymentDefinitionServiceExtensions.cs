using System.IO;
using System.Linq;
using System.Text;
using GlobExpressions;
using Unity.Services.Deployment.Core.Model;

namespace Unity.Services.Deployment.Core
{
    static class DeploymentDefinitionServiceExtensions
    {
        public static bool HasDuplicateDeploymentDefinitions(this IDeploymentDefinitionService service, out string duplicationError)
        {
            duplicationError = string.Empty;
            var paths = service.DeploymentDefinitions
                .Select(dDef => Path.GetDirectoryName(dDef.Path));
            var duplicates = paths
                .GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicates.Any())
            {
                var error = new StringBuilder();
                duplicates.ForEach(path =>
                    error.AppendLine($"Folder <{path}> contains multiple deployment definition files."));

                duplicationError = error.ToString().TrimEnd();

                return true;
            }

            return false;
        }

        public static bool IsPathExcludedByDeploymentDefinition(
            this IDeploymentDefinitionService service,
            string path,
            IDeploymentDefinition deploymentDefinition)
        {
            foreach (var excludePath in deploymentDefinition.ExcludePaths)
            {
                var itemFullPath = Path.GetFullPath(path);
                var unifiedExcludePath = excludePath.Replace('\\', '/');

                if (Glob.IsMatch(path, unifiedExcludePath)
                    || Glob.IsMatch(itemFullPath, unifiedExcludePath))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
