using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.Deployment.Editor.Tracking;
using Unity.Services.DeploymentApi.Editor;
using Logger = Unity.Services.Deployment.Editor.Shared.Logging.Logger;

namespace Unity.Services.Deployment.Editor.Validation
{
    sealed class DeploymentItemValidator : IDisposable
    {
        public int ValidationDelayMs { get; set; } = 100;
        public Task ValidationTask => m_ValidateTask;

        readonly ObservableCollection<DeploymentProvider> m_Providers;

        HashSet<DeploymentProvider> m_ToValidate = new();
        Task m_ValidateTask;

        public DeploymentItemValidator(ObservableCollection<DeploymentProvider> providers, IDeploymentItemTracker tracker)
        {
            m_Providers = providers;

            m_Providers.CollectionChanged += ProvidersOnCollectionChanged;
            m_Providers.ForEach(TriggerValidation);
            tracker.ItemAdded += TriggerValidation;
            tracker.ItemChanged += TriggerValidation;
            tracker.ItemDeleted += TriggerValidation;
        }

        void ProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.Cast<DeploymentProvider>().ForEach(TriggerValidation);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    e.NewItems.Cast<DeploymentProvider>().ForEach(TriggerValidation);
                    e.OldItems.Cast<DeploymentProvider>().ForEach(m_ToValidate.Remove);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_Providers.ForEach(TriggerValidation);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<DeploymentProvider>().ForEach(m_ToValidate.Remove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void TriggerValidation(IDeploymentItem item)
        {
            var provider = m_Providers.FirstOrDefault(p => p.DeploymentItems.Contains(item));
            if (provider != null)
            {
                TriggerValidation(provider);
            }
        }

        void TriggerValidation(DeploymentProvider provider)
        {
            m_ToValidate.Add(provider);
            if (m_ValidateTask == null || m_ValidateTask.IsCompleted)
            {
                m_ValidateTask = Sync.SafeAsync(Validate);
            }
        }

        async Task Validate()
        {
            Logger.LogVerbose("[Validation] Job Triggered");
            await Task.Delay(ValidationDelayMs);

            Logger.LogVerbose("[Validation] Job Started");
            var validationTasks = m_ToValidate
                .Where(p => p.ValidateCommand != null)
                .Select(p => p.ValidateCommand.ExecuteAsync(p.DeploymentItems))
                .ToList();
            m_ToValidate.Clear();

            await Task.WhenAll(validationTasks);
            Logger.LogVerbose("[Validation] Job Completed");
        }

        public void Dispose()
        {
            if (m_ValidateTask != null && m_ValidateTask.IsCompleted)
            {
                m_ValidateTask?.Dispose();
            }
        }
    }
}
