using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Tooling.Editor.AccessControl.Authoring.Model;
using UnityEditor;

namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Deployment
{
    class AccessControlDeploymentProvider : DeploymentProvider
    {
        public override string Service => L10n.Tr("Access Control");

        public override Command DeployCommand { get; }

        public AccessControlDeploymentProvider(
            DeployCommand deployCommand,
            ObservableAccessControlAssets observableAccessControlAssets)
            : base(observableAccessControlAssets.DeploymentItems)
        {
            DeployCommand = deployCommand;
        }
    }
}
