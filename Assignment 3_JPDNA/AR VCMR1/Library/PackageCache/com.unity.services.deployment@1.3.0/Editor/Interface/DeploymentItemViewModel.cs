using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.Interface
{
    class DeploymentItemViewModel : IDeploymentItemViewModel
    {
        bool m_IsBeingDeployed;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => OriginalItem.PropertyChanged += value;
            remove => OriginalItem.PropertyChanged -= value;
        }
        public string Name => OriginalItem.Name;
        public string Path => OriginalItem.Path;
        public float Progress => OriginalItem.Progress;
        public string Type { get; }

        public DeploymentStatus Status
        {
            get => OriginalItem.Status;
            set => OriginalItem.Status = value;
        }

        public ObservableCollection<AssetState> States => OriginalItem.States;

        public event Action<bool> DeploymentStateChanged;

        public bool IsBeingDeployed
        {
            get => m_IsBeingDeployed;
            set
            {
                if (m_IsBeingDeployed == value)
                {
                    return;
                }

                m_IsBeingDeployed = value;
                DeploymentStateChanged?.Invoke(m_IsBeingDeployed);
            }
        }
        public string Service { get; }
        public IDeploymentItem OriginalItem { get; }

        public DeploymentItemViewModel(IDeploymentItem originalItem, string service)
        {
            Service = service;
            OriginalItem = originalItem;
            Type = (OriginalItem as ITypedItem)?.Type ?? Service;
        }
    }
}
