using System;

namespace Unity.Services.Deployment.Editor.Interface.UI.Serialization
{
    interface ISerializableComponent
    {
        string SerializationKey { get; }
        object SerializationValue { get; }
        event Action ValueChanged;

        void ApplySerialization(object serializationValue);
    }
}
