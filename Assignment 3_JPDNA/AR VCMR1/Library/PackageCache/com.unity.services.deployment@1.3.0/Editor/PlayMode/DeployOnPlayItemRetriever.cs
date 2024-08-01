using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Services.Deployment.Editor.Interface.UI.Serialization;
using Unity.Services.DeploymentApi.Editor;

using SerializationContainer = Unity.Services.Deployment.Editor.Interface.UI.Views.DeploymentItemView.SerializationContainer;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    class DeployOnPlayItemRetriever : IDeployOnPlayItemRetriever
    {
        readonly ISerializationManager m_SerializationManager;
        readonly ObservableCollection<DeploymentProvider> m_Providers;

        public DeployOnPlayItemRetriever(
            ISerializationManager serializationManager,
            ObservableCollection<DeploymentProvider> providers)
        {
            m_SerializationManager = serializationManager;
            m_Providers = providers;
        }

        public IEnumerable<IDeploymentItem> GetItemsForDeployOnPlay()
        {
            var payload = m_SerializationManager.GetSavedSerialization();
            var items = m_Providers.SelectMany(p => p.DeploymentItems).ToList();

            var checkedDeploymentItems = new List<IDeploymentItem>();
            foreach (var kvp in payload)
            {
                if (kvp.Value is SerializationContainer container
                    && container.Checkmark)
                {
                    var(name, path) = SerializationContainer.DisassembleKey(kvp.Key);

                    if (!string.IsNullOrEmpty(name)
                        && !string.IsNullOrEmpty(path))
                    {
                        checkedDeploymentItems.Add(
                            items.First(item =>
                                path.Equals(item.Path)
                                && name.Equals(item.Name)));
                    }
                }
            }

            return checkedDeploymentItems;
        }
    }
}
