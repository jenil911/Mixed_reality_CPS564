using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    interface IKeyboardSelectionLogic
    {
        MultiSelect MultiSelectComponent { get; set; }

        Selectable GetTargetSelectable(KeyDownEvent keyDownEvent, bool isSingleSelect, bool isDirectionTowardsAnchor, bool isSelectPrevious);
        void ApplySelectionInputOnTarget(Selectable targetSelectable, bool isSingleSelect, bool isDirectionTowardsAnchor);
        bool IsDirectionTowardsAnchor(bool selectPrevious);
        bool TrySetVisibility(bool targetVisibility);
    }
}
