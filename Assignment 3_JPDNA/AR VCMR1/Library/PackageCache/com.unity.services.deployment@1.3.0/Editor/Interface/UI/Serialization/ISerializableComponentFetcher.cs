using System.Collections.Generic;

namespace Unity.Services.Deployment.Editor.Interface.UI.Serialization
{
    interface ISerializableComponentFetcher
    {
        List<ISerializableComponent> GetSerializableComponents(object sender);
    }
}
