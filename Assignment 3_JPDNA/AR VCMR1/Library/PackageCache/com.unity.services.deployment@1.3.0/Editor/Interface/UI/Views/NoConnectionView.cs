using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class NoConnectionView : ViewBase
    {
        protected override string UxmlName => "DeploymentWindow_NoConnection";

        new class UxmlFactory : UxmlFactory<NoConnectionView> {}
    }
}
