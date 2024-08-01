using System.Collections.Generic;

namespace Unity.Services.Deployment.Editor.Commands
{
    interface ICommandProvider
    {
        bool IsItemSupported(object obj);
        IReadOnlyList<ICommand> Commands { get; }
    }

    class TypeCommandProvider<T> : ICommandProvider
    {
        public bool IsItemSupported(object obj)
        {
            return obj is T;
        }

        public IReadOnlyList<ICommand> Commands { get; }

        public TypeCommandProvider(params Command[] commands)
        {
            Commands = commands;
        }
    }
}
