using System.ComponentModel;
using System.Linq;
using Unity.Services.Deployment.Editor.Shared.Assets;

namespace Unity.Services.Deployment.Editor.DeploymentDefinitions
{
    class DeploymentDefinitionCollection : ObservableAssets<DeploymentDefinition>
    {
        public DeploymentDefinitionCollection(bool loadAssets)
            : base(new AssetPostprocessorProxy(), loadAssets)
        {
        }

        public void AddForPath(DeploymentDefinition asset)
        {
            AddForPath(asset.Path, asset);
        }

        void RemoveForPath(DeploymentDefinition asset)
        {
            RemoveForPath(asset.Path, asset);
        }

        protected override void AddForPath(string path, DeploymentDefinition asset)
        {
            base.AddForPath(path, asset);

            var itemsList = this.OrderBy(ddef => ddef.Name).ToList();

            var currentIndex = IndexOf(asset);
            var properIndex = itemsList.IndexOf(asset);

            MoveItem(currentIndex, properIndex);
            asset.PropertyChanged += OnDeploymentDefinitionPropertyChanged;
        }

        protected override void RemoveForPath(string path, DeploymentDefinition asset)
        {
            base.RemoveForPath(path, asset);
            asset.PropertyChanged -= OnDeploymentDefinitionPropertyChanged;
        }

        void OnDeploymentDefinitionPropertyChanged(object o, PropertyChangedEventArgs eventArgs)
        {
            var ddef = o as DeploymentDefinition;
            if (eventArgs.PropertyName == nameof(ddef.Name))
            {
                RemoveForPath(ddef);
                AddForPath(ddef);
            }
        }
    }
}
