using System.Linq;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class KeyboardShortcuts : IKeyboardShortcuts
    {
        VisualElement m_Root;
        MultiSelect m_MultiSelect;
        readonly IKeyboardSelectionLogic m_KeyboardSelectionLogic;

        public VisualElement Root
        {
            get => m_Root;
            set
            {
                m_Root = value;
                m_MultiSelect = m_Root?.Q<MultiSelect>();
                m_KeyboardSelectionLogic.MultiSelectComponent = m_MultiSelect;
            }
        }

        public KeyboardShortcuts(IKeyboardSelectionLogic keyboardSelectionLogic)
        {
            m_KeyboardSelectionLogic = keyboardSelectionLogic;
        }

        public void OnKeyDown(KeyDownEvent keyDownEvent)
        {
            keyDownEvent.StopPropagation();

            switch (keyDownEvent.keyCode)
            {
                case KeyCode.Space:
                    OnSpaceKeyDown(keyDownEvent);
                    break;
                case KeyCode.UpArrow
                    or KeyCode.DownArrow
                    or KeyCode.LeftArrow
                    or KeyCode.RightArrow:
                    OnArrowKeyDown(keyDownEvent);
                    break;
            }
        }

        public void OnSpaceKeyDown(KeyDownEvent keyDownEvent)
        {
            // flip the value for selected `CheckmarkToggle` components
            // make sure to filter out the children of selected parents
            var selectedItems = m_MultiSelect.Query<Selectable>()
                .Build()
                .Where(s => s.value)
                .ToList();
            var selectableToggleGroups = selectedItems
                .Where(s => s.Q<CheckmarkToggleGroup>() != null);
            var selectablesChildrenOfSelectedParents = selectableToggleGroups
                .SelectMany(s => s.Query<Selectable>().Build())
                .Except(selectableToggleGroups);
            var filteredSelectedItems = selectedItems
                .Except(selectablesChildrenOfSelectedParents);
            filteredSelectedItems
                .Select(s => s.Q<CheckmarkToggle>())
                .ForEach(ct => ct.value = !ct.value);
        }

        public void OnArrowKeyDown(KeyDownEvent keyDownEvent)
        {
            var isSingleSelect = !(keyDownEvent.shiftKey || keyDownEvent.commandKey);
            var isSelectPrevious = keyDownEvent.keyCode == KeyCode.UpArrow;
            var isDirectionTowardsAnchor = m_KeyboardSelectionLogic.IsDirectionTowardsAnchor(isSelectPrevious);
            var targetSelectable = m_KeyboardSelectionLogic.GetTargetSelectable(keyDownEvent, isSingleSelect, isDirectionTowardsAnchor, isSelectPrevious);
            m_KeyboardSelectionLogic.ApplySelectionInputOnTarget(targetSelectable, isSingleSelect, isDirectionTowardsAnchor);
        }
    }
}
