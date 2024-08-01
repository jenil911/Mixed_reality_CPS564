using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class EnvironmentNotSetView : ViewBase
    {
        const string k_SettingsLocation = "Project/Services/Environments";

        protected override string UxmlName => "DeploymentWindow_EnvironmentNotSet";

        public EnvironmentNotSetView()
        {
            var deploymentSettingsBtn = this.Q<Button>();
            deploymentSettingsBtn.clicked += OpenEnvironmentProvider;
        }

        static void OpenEnvironmentProvider()
        {
            SettingsService.OpenProjectSettings(k_SettingsLocation);
        }

        new class UxmlFactory : UxmlFactory<EnvironmentNotSetView> {}
    }
}
