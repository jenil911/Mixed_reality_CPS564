using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Deployment.Editor.Analytics;
using Unity.Services.Deployment.Editor.Commands;
using Unity.Services.Deployment.Editor.Shared.Assets;
using Unity.Services.DeploymentApi.Editor;
using UnityEditor;
using Command = Unity.Services.Deployment.Editor.Commands.Command;

namespace Unity.Services.Deployment.Editor.Interface
{
    class SelectInProjectWindowCommand : Command, ICommandProvider
    {
        readonly IDeploymentWindowAnalytics m_DeploymentWindowAnalytics;

        public override string Name => "Select in Project Window";

        public SelectInProjectWindowCommand(IDeploymentWindowAnalytics deploymentWindowAnalytics)
        {
            m_DeploymentWindowAnalytics = deploymentWindowAnalytics;
        }

        public override Task Execute(IList objects)
        {
            var obj = objects[0];
            string path = null;
            if (obj is IDeploymentItem item)
            {
                path = item.Path;
            }
            else if (obj is IPath iPath)
            {
                path = iPath.Path;
            }

            if (string.IsNullOrEmpty(path))
                return Task.CompletedTask;

            m_DeploymentWindowAnalytics.SendContextMenuSelectEvent(path);
            SelectInProjectWindow(path);
            return Task.CompletedTask;
        }

        public override bool IsVisible(IList objects)
        {
            return objects.Count == 1;
        }

        public bool IsItemSupported(object obj)
        {
            return obj is IDeploymentItem || obj is IPath;
        }

        public IReadOnlyList<ICommand> Commands => new[] { this };

        internal static void SelectInProjectWindow(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Selection.activeObject = asset;

            var types = TypeCache
                .GetTypesDerivedFrom<EditorWindow>()
                .ToList();
            var pb = types
                .FirstOrDefault(t => t.Name == "ProjectBrowser");
            if (pb != null)
            {
                var window = EditorWindow.GetWindow(pb);
                window?.ShowTab();
            }
        }




    }
}
