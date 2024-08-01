using System.IO;
using Unity.Services.Deployment.Editor.Shared.Analytics;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions.UI
{
    class CreateDeploymentDefinition : EndNameEditAction
    {
        const string k_DefaultName = "new_deployment_definition";
        const string k_EventNameCreatedDeploymentDefinition = "deployment_definition_created";
        static readonly string k_MonoDefinitionPath = Path.Combine(Constants.k_EditorRootPath, "DeploymentDefinitions/DeploymentDefinition.cs");

        readonly ICommonAnalytics m_CommonAnalytics;

        public CreateDeploymentDefinition()
        {
            m_CommonAnalytics = DeploymentServices.Instance.GetService<ICommonAnalytics>();
        }

        [MenuItem("Assets/Create/Deployment Definition", false, 81)]
        public static void CreateDeploymentDefinitionFile()
        {
            var filePath = k_DefaultName + DeploymentDefinitionResources.FileExtension;
            var icon = DeploymentDefinitionResources.Icon;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                CreateInstance<CreateDeploymentDefinition>(),
                filePath,
                icon,
                null);
        }

        [InitializeOnLoadMethod]
        static void SetMonoDefinitionIcon()
        {
            var monoImporter = (MonoImporter)AssetImporter.GetAtPath(k_MonoDefinitionPath);
            var monoScript = monoImporter.GetScript();
            EditorGUIUtility.SetIconForObject(monoScript,  DeploymentDefinitionResources.Icon);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var definition = CreateInstance<DeploymentDefinition>();

            definition.Name = Path.GetFileNameWithoutExtension(pathName);

            m_CommonAnalytics.Send(new ICommonAnalytics.CommonEventPayload()
            {
                action = k_EventNameCreatedDeploymentDefinition,
                context = nameof(CreateDeploymentDefinition)
            });

            File.WriteAllText(pathName, definition.ToJson());
            AssetDatabase.Refresh();
        }
    }
}
