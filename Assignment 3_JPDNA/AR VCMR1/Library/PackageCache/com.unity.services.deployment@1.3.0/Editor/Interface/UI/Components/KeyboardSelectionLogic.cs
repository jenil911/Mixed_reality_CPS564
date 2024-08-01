using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    class KeyboardSelectionLogic : IKeyboardSelectionLogic
    {
        public MultiSelect MultiSelectComponent { get; set; }

        public Selectable GetTargetSelectable(KeyDownEvent keyDownEvent, bool isSingleSelect, bool isDirectionTowardsAnchor, bool isSelectPrevious)
        {
            if (MultiSelectComponent == null) return null;

            Selectable targetSelectable = null;
            switch (keyDownEvent.keyCode)
            {
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                    targetSelectable =
                        !isSingleSelect && isDirectionTowardsAnchor
                        ? MultiSelectComponent.SelectionEnd
                        : MultiSelectComponent.GetNextSelectable(
                            MultiSelectComponent.SelectionStart, isSelectPrevious, true, true);
                    break;
                case KeyCode.LeftArrow:
                    if (!TrySetVisibility(false))
                    {
                        targetSelectable = MultiSelectComponent.GetParentItem(MultiSelectComponent.SelectionStart);
                    }
                    break;
                case KeyCode.RightArrow:
                    if (!TrySetVisibility(true))
                    {
                        targetSelectable =
                            MultiSelectComponent.GetNextSelectable(MultiSelectComponent.SelectionStart, false, false, true)
                            ?? MultiSelectComponent.GetNextSelectable(MultiSelectComponent.SelectionStart, false, true, true);
                    }
                    break;
            }

            return targetSelectable;
        }

        public void ApplySelectionInputOnTarget(Selectable targetSelectable, bool isSingleSelect, bool isDirectionTowardsAnchor)
        {
            if (MultiSelectComponent == null) return;

            if (targetSelectable != null)
            {
                if (isSingleSelect)
                {
                    MultiSelectComponent.SetSelection(targetSelectable);
                }
                else
                {
                    if (isDirectionTowardsAnchor)
                        MultiSelectComponent.RemoveSelection(targetSelectable);
                    else
                        MultiSelectComponent.AddSelection(targetSelectable);
                }
            }
        }

        public bool IsDirectionTowardsAnchor(bool selectPrevious)
        {
            if (MultiSelectComponent == null) return false;

            if (MultiSelectComponent.SelectionEnd == MultiSelectComponent.SelectionStart)
                return false;

            var isBeforeTarget =
                MultiSelectComponent.IsItemBeforeTarget(MultiSelectComponent.SelectionEnd, MultiSelectComponent.SelectionStart);
            return selectPrevious ? !isBeforeTarget : isBeforeTarget;
        }

        public bool TrySetVisibility(bool targetVisibility)
        {
            if (MultiSelectComponent == null) return false;

            if (MultiSelectComponent.SelectionStart?.parent is Collapsible collapsible
                && collapsible.IsContentVisible != targetVisibility)
            {
                collapsible.SetVisibility(targetVisibility);
                return true;
            }

            return false;
        }
    }
}
