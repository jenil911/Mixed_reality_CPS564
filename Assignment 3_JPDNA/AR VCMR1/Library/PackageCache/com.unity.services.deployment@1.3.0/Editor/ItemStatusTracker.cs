using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Services.Deployment.Editor.IO;
using Unity.Services.Deployment.Editor.Tracking;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor
{
    class ItemStatusTracker : IDisposable
    {
        readonly IDeploymentItemTracker m_DeploymentItemTracker;
        readonly IFileTracker m_FileTracker;
        readonly Dictionary<string, DateTime> m_ItemNameToTimeStamp;
        readonly HashSet<IDeploymentItem> m_ItemsToTrack;

        public ItemStatusTracker(IDeploymentItemTracker deploymentItemTracker, IFileTracker fileTracker)
        {
            m_DeploymentItemTracker = deploymentItemTracker;
            m_FileTracker = fileTracker;
            m_ItemNameToTimeStamp = new Dictionary<string, DateTime>();
            m_ItemsToTrack = new HashSet<IDeploymentItem>();

            m_DeploymentItemTracker.ItemAdded += OnItemAdded;
            m_DeploymentItemTracker.ItemChanged += OnItemChanged;
            m_DeploymentItemTracker.ItemDeleted += OnItemDeleted;
        }

        void UpdateItemStatus(IDeploymentItem item)
        {
            var fileLastWriteDate = m_FileTracker.GetLastWriteTime(item.Path);

            if (!m_ItemNameToTimeStamp.TryGetValue(item.Name, out var lastWriteDate))
            {
                item.Status = IsUpToDate(item.Status)
                    ? DeploymentStatus.ModifiedLocally
                    : DeploymentStatus.Empty;

                m_ItemNameToTimeStamp[item.Name] = fileLastWriteDate;
                return;
            }

            if (fileLastWriteDate != lastWriteDate)
            {
                item.Status = DeploymentStatus.ModifiedLocally;
            }
        }

        void OnItemChanged(IDeploymentItem item)
        {
            TrackItem(item);
            UpdateItemStatus(item);
        }

        void OnItemAdded(IDeploymentItem item)
        {
            var fileLastWriteDate = m_FileTracker.GetLastWriteTime(item.Path);
            m_ItemNameToTimeStamp[item.Name] = fileLastWriteDate;

            TrackItem(item);
        }

        void TrackItem(IDeploymentItem item)
        {
            if (!m_ItemsToTrack.Contains(item))
            {
                item.PropertyChanged += OnItemPropertyChanged;
                m_ItemsToTrack.Add(item);
            }
        }

        void OnItemDeleted(IDeploymentItem item)
        {
            if (item != null)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                m_ItemsToTrack.Remove(item);
            }

            m_ItemNameToTimeStamp.Remove(item.Name);
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IDeploymentItem.Status))
            {
                return;
            }

            var item = sender as IDeploymentItem;

            if (IsUpToDate(item.Status))
            {
                m_ItemNameToTimeStamp[item.Name] = m_FileTracker.GetLastWriteTime(item.Path);
            }
        }

        bool IsUpToDate(DeploymentStatus status)
        {
            var upToDate = DeploymentStatus.UpToDate;
            return status.Message == upToDate.Message && status.MessageSeverity == upToDate.MessageSeverity;
        }

        public void Dispose()
        {
            m_DeploymentItemTracker.ItemAdded -= OnItemChanged;
            m_DeploymentItemTracker.ItemChanged -= OnItemChanged;
            m_DeploymentItemTracker.ItemDeleted -= OnItemDeleted;
        }
    }
}
