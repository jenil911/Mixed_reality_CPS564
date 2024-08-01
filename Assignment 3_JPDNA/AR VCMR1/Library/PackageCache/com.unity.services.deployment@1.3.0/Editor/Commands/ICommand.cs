using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.Deployment.Editor.Commands
{
    interface ICommand
    {
        string Name { get; }
        Task Execute(IList objects);
        bool IsVisible(IList objects);
        bool IsEnabled(IList objects);
    }

    interface ICommand<in T>
    {
        Task Execute(IReadOnlyList<T> objects);
        bool IsVisible(IReadOnlyList<T> objects);
        bool IsEnabled(IReadOnlyList<T> objects);
    }
}
