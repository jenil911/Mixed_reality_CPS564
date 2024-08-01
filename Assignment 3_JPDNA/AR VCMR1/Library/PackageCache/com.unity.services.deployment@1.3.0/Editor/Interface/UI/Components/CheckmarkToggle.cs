using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class CheckmarkToggle : Toggle
    {
        const string k_StylePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/CheckmarkToggle.uss";
        internal const string UncheckedClass = "checkmark-toggle-unchecked";
        internal const string CheckedClass = "checkmark-toggle-checked";
        internal const string MixedClass = "checkmark-toggle-mixed";

        VisualElement m_LastParent;
        CheckmarkToggleGroup m_ToggleGroup;

        public bool isHeader { get; set; }

        public event Action ValueChanged;

        public CheckmarkToggle()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
            SetToggleClass(UncheckedClass);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<ChangeEvent<bool>>(e => OnToggleValueChanged(e.newValue));
        }

        public void SetMixed()
        {
            SetToggleClass(MixedClass);
        }

        void OnGeometryChanged(GeometryChangedEvent changedEvent)
        {
            if (m_LastParent != parent)
            {
                m_LastParent = parent;
                m_ToggleGroup?.UnregisterToggle(this);

                var toggleGroup = GetFirstAncestorOfType<CheckmarkToggleGroup>();
                toggleGroup?.RegisterToggle(this);
                m_ToggleGroup = toggleGroup;
            }
        }

        internal void OnToggleValueChanged(bool newValue)
        {
            SetToggleClass(newValue ? CheckedClass : UncheckedClass);
            ValueChanged?.Invoke();
        }

        void SetToggleClass(string ussClass)
        {
            ClearClassList();
            AddToClassList(ussClass);
        }

        class CheckmarkToggleUxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_IsHeader =
                new UxmlStringAttributeDescription { name = "isHeader", defaultValue = "false" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var toggle = ve as CheckmarkToggle;
                bool.TryParse(m_IsHeader.GetValueFromBag(bag, cc), out var isHeader);
                toggle.isHeader = isHeader;
            }
        }

        new class UxmlFactory : UxmlFactory<CheckmarkToggle, CheckmarkToggleUxmlTraits> {}
    }
}
