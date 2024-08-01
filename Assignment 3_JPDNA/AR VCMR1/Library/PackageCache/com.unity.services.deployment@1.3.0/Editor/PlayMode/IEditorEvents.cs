using System;
using UnityEditor;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    interface IEditorEvents
    {
        event Action<PlayModeStateChange> PlayModeStateChanged;
    }
}
