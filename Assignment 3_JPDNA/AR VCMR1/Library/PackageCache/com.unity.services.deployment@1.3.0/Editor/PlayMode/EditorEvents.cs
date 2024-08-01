using System;
using UnityEditor;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    class EditorEvents : IEditorEvents
    {
        public event Action<PlayModeStateChange> PlayModeStateChanged
        {
            add => EditorApplication.playModeStateChanged += value;
            remove => EditorApplication.playModeStateChanged -= value;
        }
    }
}
