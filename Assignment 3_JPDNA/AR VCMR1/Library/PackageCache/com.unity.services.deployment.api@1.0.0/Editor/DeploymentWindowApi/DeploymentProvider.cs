using System.Collections.ObjectModel;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// The class responsible for providing the deployment items and the commands that can be invoked on them.
    /// </summary>
    public abstract class DeploymentProvider
    {
        /// <summary>
        /// Represents the name of a service (CloudCode, RemoteConfig etc.).
        /// </summary>
        public abstract string Service { get; }

        /// <summary>
        /// Collection of the items belonging to the specified service available for deployment.
        /// </summary>
        public ObservableCollection<IDeploymentItem> DeploymentItems { get; }

        /// <summary>
        /// Collection of the commands applicable to the deployment item type.
        /// </summary>
        public ObservableCollection<Command> Commands { get; }

        /// <summary>
        /// Command that specifies the deployment process.
        /// </summary>
        public abstract Command DeployCommand { get; }

        /// <summary>
        /// Command the specifies the double click behaviour on an item.
        /// </summary>
        public virtual Command OpenCommand => null;

        /// <summary>
        /// Command to trigger validation of items.
        /// </summary>
        public virtual Command ValidateCommand => null;

        /// <summary>
        /// Command that triggers the syncing with remote behaviour on an item.
        /// </summary>
        public virtual Command SyncItemsWithRemoteCommand => null;

        /// <summary>
        /// Deployment provider constructor.
        /// </summary>
        /// <param name="deploymentItems">Deployment items to be contained in the deployment provider.</param>
        /// <param name="commands">Commands to be contained in the deployment provider.</param>
        protected DeploymentProvider(ObservableCollection<IDeploymentItem> deploymentItems = null, ObservableCollection<Command> commands = null)
        {
            DeploymentItems = deploymentItems ?? new ObservableCollection<IDeploymentItem>();
            Commands = commands ?? new ObservableCollection<Command>();
        }

        /// <summary>
        /// Deployment provider default constructor.
        /// </summary>
        protected DeploymentProvider()
        {
            DeploymentItems = new ObservableCollection<IDeploymentItem>();
            Commands = new ObservableCollection<Command>();
        }
    }
}
