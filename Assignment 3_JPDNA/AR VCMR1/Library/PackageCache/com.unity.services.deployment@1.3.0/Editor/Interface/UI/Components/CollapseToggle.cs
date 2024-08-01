using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class CollapseToggle : Toggle
    {
        const string k_StylePath = "Packages/com.unity.services.deployment/Editor/Interface/UI/Assets/CollapseToggle.uss";
        const string k_CollapseUssClass = "collapse-toggle";
        const string k_UncollapseUssClass = "uncollapse-toggle";

        public event Action ValueChanged;
        public CollapseToggle()
        {
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
            SetToggleClass(k_CollapseUssClass);
            RegisterCallback<ChangeEvent<bool>>(e => OnToggleValueChanged(e.newValue));
        }

        void OnToggleValueChanged(bool newValue)
        {
            SetToggleClass(
                newValue
                ? k_UncollapseUssClass
                : k_CollapseUssClass);

            ValueChanged?.Invoke();
        }

        void SetToggleClass(string ussClass)
        {
            ClearClassList();
            AddToClassList(ussClass);
        }

        new class UxmlFactory : UxmlFactory<CollapseToggle> {}
    }
}
