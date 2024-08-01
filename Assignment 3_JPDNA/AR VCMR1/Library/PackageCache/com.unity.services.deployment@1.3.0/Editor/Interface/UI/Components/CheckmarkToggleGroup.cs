using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class CheckmarkToggleGroup : VisualElement
    {
        const string k_StylePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/Collapsible.uss";

        CheckmarkToggle m_HeaderToggle;
        internal HashSet<CheckmarkToggle> SubToggles;

        string LabelName { get; set; }

        public CheckmarkToggleGroup()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
            SubToggles = new HashSet<CheckmarkToggle>();
            RegisterCallback<GeometryChangedEvent>(SetupElement);
        }

        public void RegisterToggle(CheckmarkToggle toggle)
        {
            if (toggle.isHeader) return;

            if (SubToggles.Add(toggle))
            {
                UpdateHeaderToggleState();

                toggle.ValueChanged -= UpdateHeaderToggleState;
                toggle.ValueChanged += UpdateHeaderToggleState;
            }
        }

        public void UnregisterToggle(CheckmarkToggle toggle)
        {
            if (SubToggles.Remove(toggle))
            {
                toggle.ValueChanged -= UpdateHeaderToggleState;
            }
        }

        void SetupElement(GeometryChangedEvent e)
        {
            m_HeaderToggle = this
                .Query<CheckmarkToggle>()
                .Build()
                .First(ct => ct.isHeader);
            m_HeaderToggle.RegisterCallback<ChangeEvent<bool>>(changeEvent =>
                OnHeaderToggleValueChanged(changeEvent.newValue));
            UpdateHeaderToggleState();

            UnregisterCallback<GeometryChangedEvent>(SetupElement);
        }

        internal void OnHeaderToggleValueChanged(bool newValue)
        {
            foreach (var subToggle in SubToggles)
            {
                subToggle.value = newValue;
            }
        }

        void UpdateHeaderToggleState()
        {
            if (m_HeaderToggle == null) return;

            var checkedCount = SubToggles.Count(toggle => toggle.value);
            if (checkedCount == 0)
            {
                m_HeaderToggle.SetValueWithoutNotify(false);
                m_HeaderToggle.OnToggleValueChanged(false);
            }
            else if (checkedCount < SubToggles.Count)
            {
                m_HeaderToggle.SetValueWithoutNotify(false);
                m_HeaderToggle.SetMixed();
            }
            else
            {
                m_HeaderToggle.SetValueWithoutNotify(true);
                m_HeaderToggle.OnToggleValueChanged(true);
            }
        }

        class CheckmarkToggleGroupUxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_LabelName =
                new UxmlStringAttributeDescription { name = "labelName", defaultValue = null };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var toggleGroup = ve as CheckmarkToggleGroup;
                toggleGroup.LabelName = m_LabelName.GetValueFromBag(bag, cc);
            }
        }

        new class UxmlFactory : UxmlFactory<CheckmarkToggleGroup, CheckmarkToggleGroupUxmlTraits> {}
    }
}
