using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.DeploymentApi.Editor;
using DeploymentCommand = Unity.Services.DeploymentApi.Editor.Command;

namespace Unity.Services.Deployment.Editor.Commands
{
    sealed class InteropCommandProvider : ICommandProvider, IDisposable
    {
        IReadOnlyList<Command> m_Commands;
        public DeploymentProvider DeploymentProvider { get; private set; }

        public IReadOnlyList<ICommand> Commands
        {
            get
            {
                if (m_Commands == null)
                {
                    var cmds = GetDeploymentCommands();
                    m_Commands = cmds.Select(InteropCommand.Wrap).ToList();
                }

                return m_Commands;
            }
        }

        public InteropCommandProvider(DeploymentProvider deploymentProvider)
        {
            DeploymentProvider = deploymentProvider;
            DeploymentProvider.Commands.CollectionChanged += CommandsOnCollectionChanged;
        }

        void CommandsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            m_Commands = null;
        }

        public bool IsItemSupported(object obj)
        {
            return DeploymentProvider.DeploymentItems.Contains(obj);
        }

        public void Dispose()
        {
            DeploymentProvider.Commands.CollectionChanged -= CommandsOnCollectionChanged;
        }

        IEnumerable<DeploymentCommand> GetDeploymentCommands()
        {
            IEnumerable<DeploymentCommand> cmds = DeploymentProvider
                .Commands;

            var deployCommand = DeploymentProvider.DeployCommand;
            var openCommand = DeploymentProvider.OpenCommand;
            if (openCommand != null
                && !DeploymentProvider.Commands.Contains(openCommand))
            {
                cmds = cmds.Prepend(DeploymentProvider.OpenCommand);
            }

            cmds = cmds.Except(new[] {deployCommand});

            return cmds;
        }

        class InteropCommand : Command
        {
            readonly DeploymentCommand m_DeploymentProviderCommand;

            public override string Name => m_DeploymentProviderCommand.Name;

            InteropCommand(DeploymentCommand deploymentProviderCommand)
            {
                m_DeploymentProviderCommand = deploymentProviderCommand;
            }

            public static Command Wrap(DeploymentCommand apiCmd)
            {
                return new InteropCommand(apiCmd);
            }

            public override Task Execute(IList objects)
            {
                return m_DeploymentProviderCommand.ExecuteAsync(objects.Cast<IDeploymentItem>());
            }

            public override bool IsEnabled(IList objects)
            {
                return m_DeploymentProviderCommand.IsEnabled(objects.Cast<IDeploymentItem>());
            }

            public override bool IsVisible(IList objects)
            {
                return m_DeploymentProviderCommand.IsVisible(objects.Cast<IDeploymentItem>());
            }
        }
    }
}
