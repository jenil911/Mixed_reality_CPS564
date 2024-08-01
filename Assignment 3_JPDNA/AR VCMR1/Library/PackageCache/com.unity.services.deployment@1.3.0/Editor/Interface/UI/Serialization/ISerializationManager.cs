using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Serialization
{
    interface ISerializationManager
    {
        void Bind(VisualElement control);
        void Unbind();
        void ApplySerialization();
        Dictionary<string, object> GetSavedSerialization();
    }
}
