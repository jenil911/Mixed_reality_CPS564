using System.Collections.Generic;
using Unity.Services.Deployment.Editor.Configuration;
using Unity.Services.Deployment.Editor.Interface.UI.Components;
using Unity.Services.Deployment.Editor.Interface.UI.Events;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Views
{
    class DeploymentToolbar : TemplateContainer
    {
        const string k_TemplatePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/DeploymentToolbar.uxml";
        internal const string deployAllButton = "DeployAllButton";
        internal const string deploySelectedButton = "DeploySelectedButton";
        internal const string helpButton = "HelpButton";
        internal const string ActionNameDeployAllOnPlay = "Deploy Selected On Play";
        internal const string ActionNameBlockPlaymodeOnFailure = "Block Playmode On Failure";
        const string k_ToolbarContentClass = "deployment-toolbar__content";

        public bool DeployEnabled { get => m_DeployEnabled; set => SetDeployEnabled(value); }

        bool m_DeployEnabled;

        public DeploymentToolbar()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_TemplatePath);
            visualTreeAsset.CloneTree(this);

            this.Q<ToolbarButton>(deployAllButton).clicked += () => ContentChildren().ForEach(DeployAllClicked.Send);
            this.Q<ToolbarButton>(deploySelectedButton).clicked += () => ContentChildren().ForEach(DeploySelectedClicked.Send);
            this.Q<ToolbarButton>(helpButton).clicked += () => DocumentationHelper.OpenHelpDocumentation();

            SetDeployEnabled(false);
        }

        public void Bind(IDeploymentSettings deploymentSettings)
        {
            this.Q<MultiSelect>().OnSelectionChanged += OnOnSelectionChanged;

            var toolbarMenu = this.Q<ToolbarMenu>(className: "menu").menu;
            toolbarMenu.AppendAction(
                ActionNameDeployAllOnPlay,
                _ => deploymentSettings.ShouldDeployOnPlay = !deploymentSettings.ShouldDeployOnPlay,
                _ => deploymentSettings.ShouldDeployOnPlay
                ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            toolbarMenu.AppendAction(
                ActionNameBlockPlaymodeOnFailure,
                _ => deploymentSettings.BlockPlaymodeOnFailure = !deploymentSettings.BlockPlaymodeOnFailure,
                _ => deploymentSettings.BlockPlaymodeOnFailure
                ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
        }

        void SetDeployEnabled(bool value)
        {
            if (m_DeployEnabled == value)
            {
                return;
            }

            m_DeployEnabled = value;
            OnOnSelectionChanged();
            this.Q<ToolbarButton>(deployAllButton).SetEnabled(m_DeployEnabled);
        }

        void OnOnSelectionChanged()
        {
            var checkedItems = this
                .Query<CheckmarkToggle>()
                .Where(ct => ct.value)
                .ToList();
            this.Q<ToolbarButton>(deploySelectedButton).SetEnabled(m_DeployEnabled && checkedItems.Count > 0);
        }

        IEnumerable<VisualElement> ContentChildren()
        {
            return this.Q<VisualElement>(className: k_ToolbarContentClass).Children();
        }

        new class UxmlFactory : UxmlFactory<DeploymentToolbar> {}
    }
}
