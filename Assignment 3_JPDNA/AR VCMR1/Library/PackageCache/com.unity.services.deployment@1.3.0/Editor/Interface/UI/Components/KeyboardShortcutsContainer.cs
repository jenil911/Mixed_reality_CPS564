using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class KeyboardShortcutsContainer : VisualElement
    {
        MultiSelect m_MultiSelect;

        public KeyboardShortcutsContainer()
        {
            focusable = true;
            RegisterCallback<GeometryChangedEvent>(_ => OnChildrenModified());
        }

        public void BindGUI(IKeyboardShortcuts keyboardShortcuts)
        {
            keyboardShortcuts.Root = this;
            RegisterCallback<KeyDownEvent>(keyboardShortcuts.OnKeyDown);
        }

        void OnChildrenModified()
        {
            if (m_MultiSelect != null)
            {
                m_MultiSelect.OnSelectionChanged -= Focus;
            }
            m_MultiSelect = this.Q<MultiSelect>();
            if (m_MultiSelect != null)
            {
                m_MultiSelect.OnSelectionChanged += Focus;
            }
        }

        new class UxmlFactory : UxmlFactory<KeyboardShortcutsContainer> {}
    }
}
