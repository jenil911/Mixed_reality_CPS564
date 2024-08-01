using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class UnlinkedProjectView : ViewBase
    {
        protected override string UxmlName => "DeploymentWindow_UnlinkedProject";

        public UnlinkedProjectView()
        {
            var settingsBtn = this.Q<Button>();
            settingsBtn.clicked += OnSettingsClicked;
        }

        void OnSettingsClicked()
        {
            SettingsService.OpenProjectSettings("Project/Services");
        }

        new class UxmlFactory : UxmlFactory<UnlinkedProjectView> {}
    }
}
