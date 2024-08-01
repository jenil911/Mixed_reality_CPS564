using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.Deployment.Editor.Analytics;
using Unity.Services.Deployment.Editor.Configuration;
using Unity.Services.Deployment.Editor.Shared.Logging;
using Unity.Services.Deployment.Editor.Shared.UI;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    class DeployOnPlay
    {
        const string k_ProgressBarTitle = "Deploy On Play Progress";
        const string k_ProgressBarInfo = "Deploying files";
        const string k_AnalyticsSource = "deploy on play";

        readonly ObservableCollection<DeploymentProvider> m_Providers;
        readonly IDeploymentSettings m_Settings;
        readonly IDeployOnPlayAnalytics m_DeployOnPlayAnalytics;
        readonly IDeploymentAnalytics m_DeploymentAnalytics;
        readonly INotifications m_Notifications;
        readonly IDeployOnPlayItemRetriever m_DeployOnPlayItemRetriever;

        public DeployOnPlay(IPlayModeInterrupt playModeInterrupt,
                            ObservableCollection<DeploymentProvider> providers,
                            IDeploymentSettings settings,
                            IDeployOnPlayAnalytics deployOnPlayAnalytics,
                            INotifications notifications,
                            IDeployOnPlayItemRetriever deployOnPlayItemRetriever,
                            IDeploymentAnalytics deploymentAnalytics,
                            IEnvironmentsApi environmentsApi)
        {
            m_Providers = providers;
            m_Settings = settings;
            m_DeployOnPlayAnalytics = deployOnPlayAnalytics;
            m_Notifications = notifications;
            m_DeployOnPlayItemRetriever = deployOnPlayItemRetriever;
            m_DeploymentAnalytics = deploymentAnalytics;

            playModeInterrupt.OnPlay(
                environmentsApi.ValidateEnvironmentAsync,
                DeployAllAsync);
        }

        async Task DeployAllAsync()
        {
            if (!m_Settings.ShouldDeployOnPlay)
            {
                return;
            }

            var itemsToDeploy = m_DeployOnPlayItemRetriever.GetItemsForDeployOnPlay().ToHashSet();
            if (itemsToDeploy.Count <= 0)
            {
                return;
            }

            using (m_DeployOnPlayAnalytics.GetEventScope())
            {
                var tasks = new List<Task>();
                using var progressBar = m_Notifications.ProgressBar(k_ProgressBarTitle,
                    k_ProgressBarInfo,
                    itemsToDeploy.Count);

                var itemsPerProvider = new Dictionary<string, List<IDeploymentItem>>();

                foreach (var provider in m_Providers)
                {
                    var deployCommand = provider.DeployCommand;
                    if (deployCommand == null)
                    {
                        var errorMessage =
                            $"{nameof(DeploymentProvider)} for service '{provider.Service}' does not provide a DeployCommand";
                        Logger.LogError(errorMessage);
                        continue;
                    }

                    var providerItemsToDeploy = provider.DeploymentItems.Where(item => itemsToDeploy.Contains(item)).ToList();
                    itemsPerProvider[provider.Service ?? "NULL"] = providerItemsToDeploy;

                    tasks.Add(DeployItemsAsync(provider.DeployCommand, providerItemsToDeploy, progressBar));
                }

                var deployEvent = m_DeploymentAnalytics.BeginDeploy(itemsPerProvider, k_AnalyticsSource);
                try
                {
                    Logger.LogVerbose($"[DeployOnPlay] Deployment Started");
                    await Task.WhenAll(tasks);
                    deployEvent.SendSuccess();
                    Logger.LogVerbose($"[DeployOnPlay] Deployment Succeeded");
                }
                catch (Exception e)
                {
                    Logger.LogVerbose($"[DeployOnPlay] Deployment Failed: {e}");
                    deployEvent.SendFailure(e);
                    throw;
                }
            }
        }

        static async Task DeployItemsAsync(Command deployCommand, IEnumerable<IDeploymentItem> items, IProgressBar progressBar)
        {
            void itemOnPropertyChanged(object sender, PropertyChangedEventArgs e) => ItemOnPropertyChanged(sender, e, progressBar);

            var itemsList = items.ToList();
            itemsList.ForEach(item =>
                item.PropertyChanged += itemOnPropertyChanged);

            await deployCommand.ExecuteAsync(itemsList);

            itemsList.ForEach(item => item.PropertyChanged -= itemOnPropertyChanged);
        }

        static void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e, IProgressBar progressBar)
        {
            var item = sender as IDeploymentItem;

            if (e.PropertyName == nameof(item.Progress) && item.Progress == 100f)
            {
                progressBar.CompleteStep();
            }
        }
    }
}
