using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Deployment.Editor.Shared.Infrastructure.Collections;

namespace Unity.Services.Deployment.Editor.Commands
{
    partial class CommandManager
    {
        class AggregateCommand : Command
        {
            internal readonly Dictionary<ICommandProvider, ICommand> Commands
                = new Dictionary<ICommandProvider, ICommand>();

            public override string Name  { get; }

            public AggregateCommand(string name)
            {
                Name = name;
            }

            public override Task Execute(IList objects)
            {
                var objs = objects.OfType<object>().ToList();

                var tasks = new List<Task>();
                foreach (var(provider, command) in Commands)
                {
                    var providerObjs = objs.Where(provider.IsItemSupported).ToList();

                    var task = command.Execute(providerObjs);
                    tasks.Add(task);

                    providerObjs.ForEach(objs.Remove);
                }

                return Task.WhenAll(tasks);
            }

            public override bool IsEnabled(IList objects)
            {
                var objs = objects.OfType<object>().ToList();

                foreach (var(provider, command) in Commands)
                {
                    var providerObjs = objs.Where(provider.IsItemSupported).ToList();

                    var isEnabled = command.IsEnabled(providerObjs);
                    if (!isEnabled)
                        return false;

                    providerObjs.ForEach(objs.Remove);
                }

                return true;
            }

            public override bool IsVisible(IList objects)
            {
                var objs = objects.OfType<object>().ToList();

                foreach (var(provider, command) in Commands)
                {
                    var providerObjs = objs.Where(provider.IsItemSupported).ToList();

                    var isVisible = command.IsVisible(providerObjs);
                    if (!isVisible)
                        return false;

                    providerObjs.ForEach(objs.Remove);
                }

                return true;
            }
        }
    }
}
