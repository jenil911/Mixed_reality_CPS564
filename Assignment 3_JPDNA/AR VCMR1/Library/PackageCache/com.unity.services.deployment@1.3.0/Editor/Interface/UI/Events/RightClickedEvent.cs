using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Events
{
    class RightClickedEvent : EventBase<RightClickedEvent>
    {
        public static void Send(VisualElement sender)
        {
            using var evt = GetPooled();
            evt.target = sender;
            evt.bubbles = true;
            sender.SendEvent(evt);
        }
    }
}
