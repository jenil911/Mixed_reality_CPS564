using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;

namespace Unity.Services.Deployment.Editor.Commands
{
    partial class CommandManager : ICommandManager
    {
        protected internal ObservableCollection<ICommandProvider> CommandProviders { get; }

        public CommandManager()
        {
            CommandProviders = new ObservableCollection<ICommandProvider>();
            CommandProviders.CollectionChanged += CommandProvidersOnCollectionChanged;
        }

        public IReadOnlyList<ICommand> GetCommandsForObjects(IEnumerable<object> objects)
        {
            IReadOnlyList<object> objs = objects as IReadOnlyList<object> ?? objects.ToList();
            if (!objs.Any())
            {
                return Array.Empty<ICommand>();
            }

            var commonCommandNames = GetCommonCommandNames(objs);

            if (!commonCommandNames.Any())
            {
                return Array.Empty<ICommand>();
            }

            var commonCommands = commonCommandNames.ToDictionary(s => s, s => new AggregateCommand(s));
            foreach (var(name, aggregate) in commonCommands)
            {
                foreach (var provider in CommandProviders)
                {
                    var command = provider.Commands.FirstOrDefault(c => c.Name == name);
                    if (command != null)
                    {
                        aggregate.Commands[provider] = command;
                    }
                }
            }

            return commonCommands.Values.ToList();
        }

        public void AddCommandProvider(ICommandProvider commandProvider)
        {
            ValidateCommandProvider(commandProvider);

            CommandProviders.Add(commandProvider);
        }

        public void RemoveCommandProvider(ICommandProvider commandProvider)
        {
            CommandProviders.Remove(commandProvider);
        }

        void CommandProvidersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            e.NewItems.Cast<ICommandProvider>().ForEach(ValidateCommandProvider);
        }

        static void ValidateCommandProvider(ICommandProvider commandProvider)
        {
            if (commandProvider.Commands.Any(c => c == null))
            {
                throw new InvalidOperationException(
                    $"{nameof(ICommandProvider)} '{commandProvider.GetType().Name}' has 'null' commands");
            }

            if (commandProvider.Commands.Any(c => c.Name == null))
            {
                throw new InvalidOperationException(
                    $"{nameof(ICommandProvider)} '{commandProvider.GetType().Name}' has commands with 'null' names");
            }

            if (commandProvider.Commands.Select(c => c.Name).ToHashSet().Count
                != commandProvider.Commands.Count)
            {
                throw new InvalidOperationException(
                    $"{nameof(ICommandProvider)} '{commandProvider.GetType().Name}' has commands with duplicate names");
            }
        }

        HashSet<string> GetCommonCommandNames(
            IReadOnlyList<object> objects)
        {
            var commandNames = GetCommandsForObject(objects.First())
                .Select(c => c.Name)
                .ToHashSet();

            for (int i = 1; i < objects.Count; ++i)
            {
                var objCommands = GetCommandsForObject(objects[i])
                    .Select(c => c.Name);
                commandNames.IntersectWith(objCommands);
            }

            return commandNames;
        }

        IReadOnlyList<ICommand> GetCommandsForObject(object obj)
        {
            var providers = CommandProviders
                .Where(p => p.IsItemSupported(obj))
                .ToList();

            if (!providers.Any())
            {
                return Array.Empty<ICommand>();
            }

            var allCommands = providers
                .SelectMany(p => p.Commands)
                .ToList();
            return allCommands;
        }
    }
}
