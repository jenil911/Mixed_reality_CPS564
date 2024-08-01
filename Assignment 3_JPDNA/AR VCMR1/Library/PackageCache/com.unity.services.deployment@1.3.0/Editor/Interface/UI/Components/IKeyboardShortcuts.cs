using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Components
{
    interface IKeyboardShortcuts
    {
        VisualElement Root { get; set; }
        void OnKeyDown(KeyDownEvent keyDownEvent);
        void OnSpaceKeyDown(KeyDownEvent keyDownEvent);
        void OnArrowKeyDown(KeyDownEvent keyDownEvent);
    }
}
