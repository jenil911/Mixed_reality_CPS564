using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Unity.Services.DeploymentApi.Editor;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Interface.UI
{
    class StatusPanel
    {
        const string k_Status = "Status";

        readonly Label m_StatusLabel;
        IDeploymentItem m_SelectedItem;

        public IDeploymentItem SelectedItem
        {
            get => m_SelectedItem;
            set
            {
                if (m_SelectedItem != null)
                {
                    m_SelectedItem.PropertyChanged -= OnItemPropertyChanged;
                    m_SelectedItem.States.CollectionChanged -= StatesOnCollectionChanged;
                }

                m_SelectedItem = value;
                m_SelectedItem.PropertyChanged += OnItemPropertyChanged;
                m_SelectedItem.States.CollectionChanged += StatesOnCollectionChanged;

                UpdateStatus();
            }
        }

        public StatusPanel(Label statusLabel)
        {
            m_StatusLabel = statusLabel;
        }

        public void Clear()
        {
            m_StatusLabel.text = string.Empty;
        }

        void UpdateStatus()
        {
            var itemStatus = m_SelectedItem.Status;
            var statusBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(itemStatus.Message))
            {
                statusBuilder.AppendLine(itemStatus.Message);
                statusBuilder.AppendLine(itemStatus.MessageDetail);
            }

            foreach (var state in m_SelectedItem.States)
            {
                statusBuilder.AppendLine(state.Description);
                statusBuilder.AppendLine(state.Detail);
            }

            m_StatusLabel.text = statusBuilder.ToString();
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == k_Status)
            {
                UpdateStatus();
            }
        }

        void StatesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateStatus();
        }
    }
}
