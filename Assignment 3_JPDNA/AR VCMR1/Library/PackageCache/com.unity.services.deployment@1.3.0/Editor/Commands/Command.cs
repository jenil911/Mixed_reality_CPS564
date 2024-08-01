using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unity.Services.Deployment.Editor.Commands
{
    abstract class Command : ICommand
    {
        public abstract string Name { get; }
        public abstract Task Execute(IList objects);
        public virtual bool IsVisible(IList objects) { return true; }
        public virtual bool IsEnabled(IList objects) { return true; }
        public override string ToString() => Name;
    }

    abstract class Command<T> : Command, ICommand<T>
    {
        public sealed override Task Execute(IList objects) => Execute(objects.Cast<T>().ToList());
        public abstract Task Execute(IReadOnlyList<T> objects);

        public sealed override bool IsVisible(IList objects) => IsVisible(objects.Cast<T>().ToList());
        public virtual bool IsVisible(IReadOnlyList<T> objects) { return true; }

        public sealed override bool IsEnabled(IList objects) => IsEnabled(objects.Cast<T>().ToList());
        public virtual bool IsEnabled(IReadOnlyList<T> objects) { return true; }
    }
}
