using Unity.Services.DeploymentApi.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class DeploymentItemStateView : VisualElement
    {
        const string k_TemplatePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Templates/DeploymentItemStateTemplate.uxml";
        const string k_NameWarning = "ItemWarning";
        const string k_NameError = "ItemError";

        VisualElement m_WarningContainer;
        VisualElement m_ErrorContainer;

        public AssetState ItemState { get; private set; }

        public DeploymentItemStateView()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_TemplatePath);
            visualTreeAsset.CloneTree(this);

            m_WarningContainer = this.Q<VisualElement>(name: k_NameWarning);
            m_ErrorContainer = this.Q<VisualElement>(name: k_NameError);
        }

        public void Bind(AssetState assetState)
        {
            ItemState = assetState;
            UpdateContainer(m_WarningContainer, assetState, assetState.Level == SeverityLevel.Warning);
            UpdateContainer(m_ErrorContainer, assetState, assetState.Level == SeverityLevel.Error);
        }

        void UpdateContainer(VisualElement container, AssetState assetState, bool shouldShow)
        {
            container.style.display =
                shouldShow
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            if (shouldShow)
            {
                container.Q<Label>().text = assetState.Description;
            }
        }
    }
}
