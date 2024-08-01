using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class MultipleDefinitionsView : ViewBase
    {
        const string k_UnableToDisplaySets = "Unable to display deployment sets, because";

        HelpBox m_HelpBox;

        protected override string UxmlName => "DeploymentWindow_MultipleDefinitions";

        public MultipleDefinitionsView()
        {
            m_HelpBox = this.Q<HelpBox>();
        }

        public void SetError(string error)
        {
            m_HelpBox.text = $"{k_UnableToDisplaySets} {error}";
        }

        new class UxmlFactory : UxmlFactory<MultipleDefinitionsView> {}
    }
}
