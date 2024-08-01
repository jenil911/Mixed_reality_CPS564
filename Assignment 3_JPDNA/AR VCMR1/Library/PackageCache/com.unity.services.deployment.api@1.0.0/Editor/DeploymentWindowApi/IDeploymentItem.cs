using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// Interface representing Deployment Item.
    /// </summary>
    public interface IDeploymentItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Represents the name with an extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Represents the full path of a deployment item.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Represents the deployment progress of a deployment item.
        /// Is expected to be a value between 0 and 100.
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Represents the status of the item relative to what is available live.
        /// </summary>
        DeploymentStatus Status { get; set; }

        /// <summary>
        /// Represents possible states of the local asset(s) the item represents,
        /// including with regards to its validity, format or other.
        /// </summary>
        ObservableCollection<AssetState> States { get; }
    }
}
