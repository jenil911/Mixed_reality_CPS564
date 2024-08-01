using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// Concrete implementation of IDeploymentItem. Should be implemented over the interface.
    /// </summary>
    public class DeploymentItem : IDeploymentItem, ITypedItem
    {
        /// <summary>
        /// Name of the deployment item.
        /// </summary>
        protected string m_Name;

        /// <summary>
        /// Path of the deployment item.
        /// </summary>
        protected string m_Path;

        /// <summary>
        /// Type of the deployment item.
        /// </summary>
        protected string m_Type;

        /// <summary>
        /// Progress of the deployment item.
        /// </summary>
        protected float m_Progress;

        /// <summary>
        /// Status of the deployment item.
        /// </summary>
        protected DeploymentStatus m_Status;

        /// <summary>
        /// The asset state collection of the deployment item.
        /// </summary>
        protected ObservableCollection<AssetState> m_States = new ObservableCollection<AssetState>();

        /// <summary>
        /// An event that tracks a change of a property of the deployment item.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public virtual string Name
        {
            get => m_Name;
            set => SetField(ref m_Name, value);
        }

        /// <inheritdoc/>
        public virtual string Path
        {
            get => m_Path;
            set => SetField(ref m_Path, value);
        }

        /// <inheritdoc/>
        public string Type
        {
            get => m_Type;
            set => SetField(ref m_Type, value);
        }

        /// <inheritdoc/>
        public virtual float Progress
        {
            get => m_Progress;
            set => SetField(ref m_Progress, value);
        }

        /// <inheritdoc/>
        public virtual DeploymentStatus Status
        {
            get => m_Status;
            set => SetField(ref m_Status, value);
        }

        /// <inheritdoc/>
        public ObservableCollection<AssetState> States => m_States;

        /// <summary>
        /// Sets the field and raises an OnPropertyChanged event.
        /// </summary>
        /// <param name="field">The field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="onFieldChanged">The callback.</param>
        /// <param name="propertyName">Name of the property to set.</param>
        /// <typeparam name="T">Type of the parameter.</typeparam>
        protected void SetField<T>(
            ref T field,
            T value,
            Action<T> onFieldChanged = null,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
            onFieldChanged?.Invoke(field);
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
