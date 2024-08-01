using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class NotSignedInView : ViewBase
    {
        protected override string UxmlName => "DeploymentWindow_NotSignedIn";

        public NotSignedInView()
        {
            var settingsBtn = this.Q<Button>();
            settingsBtn.clicked += OnSettingsClicked;
        }

        static void OnSettingsClicked()
        {
            SettingsService.OpenProjectSettings("Project/Services");
        }

        new class UxmlFactory : UxmlFactory<NotSignedInView> {}
    }
}
