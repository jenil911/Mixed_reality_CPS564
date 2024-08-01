using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI.Serialization
{
    class UiTreeSerializableComponentFetcher : ISerializableComponentFetcher
    {
        public List<ISerializableComponent> GetSerializableComponents(object sender)
        {
            var visualElement = (VisualElement)sender;
            return visualElement == null
                ? new List<ISerializableComponent>()
                : visualElement
                .Query<VisualElement>()
                .Build()
                .OfType<ISerializableComponent>()
                .ToList();
        }
    }
}
