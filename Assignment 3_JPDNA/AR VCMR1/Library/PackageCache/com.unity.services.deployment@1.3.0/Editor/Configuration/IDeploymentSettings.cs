using System.ComponentModel;

namespace Unity.Services.Deployment.Editor.Configuration
{
    interface IDeploymentSettings : INotifyPropertyChanged
    {
        bool ShouldDeployOnPlay { get; set; }
        bool BlockPlaymodeOnFailure { get; set; }
    }
}
