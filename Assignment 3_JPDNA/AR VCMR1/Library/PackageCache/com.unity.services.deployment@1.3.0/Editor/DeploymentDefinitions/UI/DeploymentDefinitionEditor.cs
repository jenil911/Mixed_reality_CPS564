using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions.UI
{
    [CustomEditor(typeof(DeploymentDefinition))]
    [CanEditMultipleObjects]
    class DeploymentDefinitionEditor : UnityEditor.Editor
    {
        static readonly string k_UxmlPath = Path.Combine(Constants.k_EditorRootPath, "DeploymentDefinitions/UI/Assets/DeploymentDefinitionEditorUI.uxml");

        IEnumerable<DeploymentDefinition> Targets => serializedObject.targetObjects.Cast<DeploymentDefinition>();

        ApplyRevertChangeTracker<DeploymentDefinition> m_ChangeTracker;
        ListView m_ListView;
        VisualElement m_ApplyFooter;
        readonly Dictionary<TextField, int> m_BindingMap = new Dictionary<TextField, int>();

        public override VisualElement CreateInspectorGUI()
        {
            DisableReadonlyFlags();
            m_ChangeTracker = new ApplyRevertChangeTracker<DeploymentDefinition>(serializedObject);

            var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_UxmlPath);
            var rootElement = new VisualElement();
            uxmlAsset.CloneTree(rootElement);

            BindControls(rootElement);

            return rootElement;
        }

        void BindControls(VisualElement rootElement)
        {
            rootElement.Bind(m_ChangeTracker.SerializedObject);

            BindApplyFooter(rootElement);
            BindExcludePaths(rootElement);

            foreach (var property in rootElement.Query<PropertyField>().Build())
            {
                property.RegisterValueChangeCallback(_ => UpdateApplyRevertEnabled());
            }

            UpdateApplyRevertEnabled();
        }

        void BindExcludePaths(VisualElement rootElement)
        {
            m_ListView = rootElement.Q<ListView>();
            m_ListView.bindItem = BindItem;
            m_ListView.makeItem = MakeItem;
            var ddef = (DeploymentDefinition)m_ChangeTracker.SerializedObject.targetObjects[0];
            m_ListView.itemsSource = ddef.ExcludePaths;
            m_ListView.itemsRemoved += _ => UpdateOnNextFrame();

            void UpdateOnNextFrame()
            {
                Sync.RunNextUpdateOnMain(UpdateApplyRevertEnabled);
            }
        }

        static VisualElement MakeItem()
        {
            return new TextField()
            {
                isDelayed = true,
            };
        }

        void BindItem(VisualElement visualElement, int index)
        {
            var textField = (TextField)visualElement;
            m_BindingMap[textField] = index;
            textField.UnregisterValueChangedCallback(OnExcludePathValueChanged);
            var ddef = (DeploymentDefinition)m_ChangeTracker.SerializedObject.targetObjects[0];
            textField.value = ddef.ExcludePaths[index];
            textField.RegisterValueChangedCallback(OnExcludePathValueChanged);
        }

        void OnExcludePathValueChanged(ChangeEvent<string> changeEvent)
        {
            var ddef = (DeploymentDefinition)m_ChangeTracker.SerializedObject.targetObjects[0];
            ddef.ExcludePaths[m_BindingMap[(TextField)changeEvent.target]] = changeEvent.newValue;
            UpdateApplyRevertEnabled();
        }

        void BindApplyFooter(VisualElement rootElement)
        {
            m_ApplyFooter = rootElement.Q<VisualElement>(UxmlNames.ApplyFooter);

            rootElement.Q<Button>(UxmlNames.Apply).clicked += ApplyChanges;
            rootElement.Q<Button>(UxmlNames.Revert).clicked += RevertChanges;
        }

        void ApplyChanges()
        {
            m_ChangeTracker.Apply();
            foreach (var definition in Targets)
            {
                definition.SaveChanges();
            }
            UpdateApplyRevertEnabled();
            AssetDatabase.Refresh();
        }

        void RevertChanges()
        {
            m_ChangeTracker.Reset();
            m_ListView.Rebuild();
            UpdateApplyRevertEnabled();
        }

        void UpdateApplyRevertEnabled()
        {
            m_ApplyFooter.SetEnabled(m_ChangeTracker.IsDirty());
        }

        void DisableReadonlyFlags()
        {
            serializedObject.targetObject.hideFlags = HideFlags.None;
        }

        static class UxmlNames
        {
            public const string Apply = "Apply";
            public const string Revert = "Revert";
            public const string ApplyFooter = "Apply Footer";
        }
    }
}
