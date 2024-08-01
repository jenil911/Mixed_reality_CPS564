using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.Deployment.Core.Model;
using Unity.Services.Deployment.Editor.Analytics;
using Unity.Services.Deployment.Editor.DeploymentDefinitions;
using Unity.Services.Deployment.Editor.Environments;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.DeploymentApi.Editor;
using UnityEngine;
using Logger = Unity.Services.Deployment.Editor.Shared.Logging.Logger;

namespace Unity.Services.Deployment.Editor.Interface
{
    sealed class DeploymentViewModel : IDeploymentViewModel, IDisposable
    {
        const string k_AnalyticsSource = "deployment window";

        public IReadOnlyObservable<IDeploymentDefinitionViewModel> DeploymentDefinitions => m_DefinitionViewModels.AsReadonly();

        readonly ObservableCollection<DeploymentProvider> m_DeploymentProviders;
        readonly IDeploymentAnalytics m_Analytics;
        readonly IEnvironmentsApi m_EnvironmentsApi;
        readonly ObservableCollection<DeploymentDefinitionViewModel> m_DefinitionViewModels;
        readonly IEditorDeploymentDefinitionService m_DefinitionService;

        public DeploymentViewModel(
            IEnvironmentsApi environmentsApi,
            IEditorDeploymentDefinitionService definitionService,
            ObservableCollection<DeploymentProvider> deploymentProviders,
            IDeploymentAnalytics analytics)
        {
            m_EnvironmentsApi = environmentsApi;
            m_DefinitionService = definitionService;
            m_DeploymentProviders = deploymentProviders;
            m_Analytics = analytics;

            m_DefinitionViewModels = new ObservableCollection<DeploymentDefinitionViewModel>();
            AddAllViewModelsForDefinitionService(m_DefinitionService);

            m_DefinitionService.ObservableDeploymentDefinitions.CollectionChanged += DeploymentDefinitionsOnCollectionChanged;
        }

        void AddAllViewModelsForDefinitionService(IEditorDeploymentDefinitionService definitionService)
        {
            var allDefinitions = new List<IDeploymentDefinition>(definitionService.DeploymentDefinitions)
            { definitionService.DefaultDefinition };
            allDefinitions.ForEach(AddViewModelForDefinition);
        }

        public async Task DeployItemsAsync(
            IEnumerable<IDeploymentItemViewModel> items,
            IEnumerable<int> itemsPerDeploymentDefinitions)
        {
            foreach (var itemsPerDeployment in itemsPerDeploymentDefinitions)
            {
                m_Analytics.SendDeploymentDefinitionDeployedEvent(itemsPerDeployment);
            }

            await DeployItemsAsync(items);
        }

        async Task DeployItemsAsync(IEnumerable<IDeploymentItemViewModel> items)
        {
            var enumeratedItems = items
                .Where(item => !item.IsBeingDeployed)
                .EnumerateOnce();
            enumeratedItems.ForEach(item => item.IsBeingDeployed = true);
            var analytics = m_Analytics.BeginDeploy(ItemsPerProvider(enumeratedItems), k_AnalyticsSource);
            try
            {
                await ValidateEnvironment(enumeratedItems);
                await ExecuteCommandAsync(enumeratedItems, p => p.DeployCommand);
                analytics.SendSuccess();
            }
            catch (Exception e)
            {
                analytics.SendFailure(e);
                Logger.LogException(e);
                throw;
            }
            finally
            {
                enumeratedItems.ForEach(item => item.IsBeingDeployed = false);
            }
        }

        public async Task DeployDefinitionsAsync(IEnumerable<IDeploymentDefinitionViewModel> definitions)
        {
            var enumeratedDefinitions = definitions.EnumerateOnce();
            var itemsToDeploy = enumeratedDefinitions.SelectMany(dvm => dvm.DeploymentItemViewModels);
            var itemsPerDefinition = enumeratedDefinitions
                .Select(d => d.DeploymentItemViewModels.Count);
            await DeployItemsAsync(itemsToDeploy, itemsPerDefinition);
        }

        async Task ValidateEnvironment(IEnumerable<IDeploymentItemViewModel> items)
        {
            var validationResult = await m_EnvironmentsApi.ValidateEnvironmentAsync();
            if (validationResult.Failed)
            {
                items.ForEach(item => item.Status = new DeploymentStatus(
                    "Invalid Environment",
                    validationResult.ErrorMessage));
                throw new InvalidEnvironmentException(validationResult);
            }
        }

        void DeploymentDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems
                        .Cast<DeploymentDefinition>()
                        .ForEach(AddViewModelForDefinition);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems
                        .Cast<DeploymentDefinition>()
                        .ForEach(RemoveViewModelForDefinition);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_DefinitionViewModels.Clear();
                    AddAllViewModelsForDefinitionService(m_DefinitionService);
                    break;
                case NotifyCollectionChangedAction.Move:
                    m_DefinitionViewModels.Clear();
                    AddAllViewModelsForDefinitionService(m_DefinitionService);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException($"{nameof(DeploymentViewModel)} does not support {e.Action}");
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void AddViewModelForDefinition(IDeploymentDefinition ddef)
        {
            m_DefinitionViewModels.Add(new DeploymentDefinitionViewModel((IEditorDeploymentDefinition)ddef, m_DefinitionService, m_DeploymentProviders));
        }

        void RemoveViewModelForDefinition(IDeploymentDefinition ddef)
        {
            var viewModel = m_DefinitionViewModels.FirstOrDefault(vm => vm.Model == ddef);
            if (viewModel != null)
            {
                m_DefinitionViewModels.Remove(viewModel);
            }
            else
            {
                Logger.LogError($"Could not locate {nameof(DeploymentDefinitionViewModel)} for {nameof(IDeploymentDefinition)} '{ddef.Name}'");
            }
        }

        Task ExecuteCommandAsync(IReadOnlyCollection<IDeploymentItemViewModel> deploymentItemViewModels, Func<DeploymentProvider, Command> command)
        {
            var providerCommands = m_DeploymentProviders
                .Select(provider => new Tuple<DeploymentProvider, Command>(provider, command(provider)))
                .Where(tuple => tuple.Item2 != null)
                .ToList();
            return ExecuteCommandAsync(providerCommands, deploymentItemViewModels);
        }

        static IReadOnlyDictionary<string, List<IDeploymentItem>> ItemsPerProvider(IReadOnlyCollection<IDeploymentItemViewModel> items)
        {
            var mapping = new Dictionary<string, List<IDeploymentItem>>();
            foreach (var item in items)
            {
                var key = item.Service ?? "NULL";
                if (!mapping.ContainsKey(key))
                {
                    mapping[key] = new List<IDeploymentItem>();
                }
                mapping[key].Add(item.OriginalItem);
            }

            return mapping;
        }

        static async Task ExecuteCommandAsync(IEnumerable<Tuple<DeploymentProvider, Command>> providerCommands, IReadOnlyCollection<IDeploymentItemViewModel> deploymentItemViewModels)
        {
            var commandTasks = new List<Task>();

            foreach (var tuple in providerCommands)
            {
                var provider = tuple.Item1;
                var command = tuple.Item2;

                var providerItems = provider.DeploymentItems
                    .Intersect(deploymentItemViewModels.Select(i => i.OriginalItem))
                    .EnumerateOnce();

                if (!providerItems.Any())
                {
                    continue;
                }

                commandTasks.Add(command.ExecuteAsync(providerItems));
            }

            await Task.WhenAll(commandTasks);
        }

        public void Dispose()
        {
            m_DefinitionService.ObservableDeploymentDefinitions.CollectionChanged -= DeploymentDefinitionsOnCollectionChanged;
        }
    }
}
