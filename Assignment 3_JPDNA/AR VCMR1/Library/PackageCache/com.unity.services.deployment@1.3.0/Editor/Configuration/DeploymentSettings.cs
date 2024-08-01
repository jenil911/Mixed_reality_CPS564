using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unity.Services.Deployment.Editor.Configuration
{
    class DeploymentSettings : IDeploymentSettings
    {
        const string k_DeployOnPlayToggleKey = "Deployment_DeployOnPlay";
        const string k_BlockPlaymodeOnFailure = "Deployment_BlockPlaymodeOnFailure";
        IProjectPreferences m_ProjectPreferences;

        public DeploymentSettings(IProjectPreferences projectProjectPreferences)
        {
            m_ProjectPreferences = projectProjectPreferences;
        }

        public bool ShouldDeployOnPlay
        {
            get => m_ProjectPreferences.GetBool(k_DeployOnPlayToggleKey);
            set
            {
                m_ProjectPreferences.SetBool(k_DeployOnPlayToggleKey, value);
                OnPropertyChanged();
            }
        }

        public bool BlockPlaymodeOnFailure
        {
            get => m_ProjectPreferences.GetBool(k_BlockPlaymodeOnFailure);
            set
            {
                m_ProjectPreferences.SetBool(k_BlockPlaymodeOnFailure, value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
