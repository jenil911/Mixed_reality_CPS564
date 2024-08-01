using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.Deployment.Editor.Interface;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;
using Unity.Services.DeploymentApi.Editor;

namespace Unity.Services.Deployment.Editor.Commands
{
    sealed class DeploymentCommandManager : CommandManager, IDisposable
    {
        readonly List<InteropCommandProvider> m_InteropProviders;
        readonly ObservableCollection<DeploymentProvider> m_DeploymentProviders;

        public DeploymentCommandManager(
            SelectInProjectWindowCommand selectInProjectWindow,
            ObservableCollection<DeploymentProvider> deploymentProviders)
        {
            m_InteropProviders = new List<InteropCommandProvider>();
            AddCommandProvider(selectInProjectWindow);
            m_DeploymentProviders = deploymentProviders;
            InitializeCommandProviders();
            m_DeploymentProviders.CollectionChanged += DeploymentProvidersOnCollectionChanged;
        }

        void DeploymentProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    AddRemoveItems(e.NewItems, e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    var interopProviders = m_InteropProviders
                        .ToList();

                    AddRemoveItems(null, interopProviders);
                    InitializeCommandProviders();
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown action {e.Action}");
            }
        }

        void InitializeCommandProviders()
        {
            m_DeploymentProviders
                .Select(CreateInteropCommand)
                .ForEach(AddCommandProvider);
        }

        void AddRemoveItems(IList newItems, IList oldItems)
        {
            newItems = newItems ?? Array.Empty<DeploymentProvider>();
            oldItems = oldItems ?? Array.Empty<DeploymentProvider>();

            newItems.OfType<DeploymentProvider>()
                .Select(CreateInteropCommand)
                .ForEach(p =>
                {
                    m_InteropProviders.Add(p);
                    AddCommandProvider(p);
                });

            var removedProviders = m_InteropProviders
                .Where(p => oldItems.Contains(p.DeploymentProvider))
                .ToList();

            foreach (var provider in removedProviders)
            {
                m_InteropProviders.Remove(provider);
                RemoveCommandProvider(provider);
                provider.Dispose();
            }
        }

        InteropCommandProvider CreateInteropCommand(DeploymentProvider provider)
        {
            var interopCommandProvider = new InteropCommandProvider(provider);
            m_InteropProviders.Add(interopCommandProvider);
            return interopCommandProvider;
        }

        public void Dispose()
        {
            m_DeploymentProviders.CollectionChanged -= DeploymentProvidersOnCollectionChanged;
            m_InteropProviders.ForEach(c => c.Dispose());
        }
    }
}
