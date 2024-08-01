using System.Diagnostics;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Analytics;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Core.Model;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model.File;
using Unity.Services.Tooling.Editor.Shared.Assets;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Model
{
    class AccessControlAsset : ScriptableObject, IPath
    {
        const string k_DefaultFileName = "access_control_config";
        string m_Path;
        public string Path { get => m_Path; set => SetPath(value); }
        public IProjectAccessFile ResourceDeploymentItem { get; set; }

        void SetPath(string path)
        {
            ResourceDeploymentItem ??= new ProjectAccessFile
            {
                Path = path,
                Name = string.IsNullOrEmpty(path) ? string.Empty : System.IO.Path.GetFileName(path)
            };

            m_Path = path;
        }

        [MenuItem("Assets/Create/Services/Access Control Configuration", false, 81)]
        public static void CreateConfig()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var fileName = k_DefaultFileName + ProjectAccessFileExtension.Extension;

            ProjectWindowUtil.CreateAssetWithContent(fileName, new AccessControlConfigFile().FileBodyText);

            stopWatch.Stop();
            AccessControlAuthoringServices.Instance.GetService<IAccessControlEditorAnalytics>()
                .SendEvent("access_control_file_created", default, stopWatch.ElapsedMilliseconds);
        }

        public void ClearOwnedStates()
        {
            var states = ResourceDeploymentItem.States;
            var i = 0;
            while (i < states.Count)
            {
                if (ObservableAccessControlAssets.IsLoadError(states[i].Description) ||
                    ObservableAccessControlAssets.IsValidationError(states[i].Description) ||
                    ObservableAccessControlAssets.IsValidationWarning(states[i].Description))
                {
                    states.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
