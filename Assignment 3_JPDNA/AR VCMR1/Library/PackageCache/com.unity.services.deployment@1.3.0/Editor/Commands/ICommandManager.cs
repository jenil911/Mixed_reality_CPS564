using System.Collections.Generic;

namespace Unity.Services.Deployment.Editor.Commands
{
    interface ICommandManager
    {
        IReadOnlyList<ICommand> GetCommandsForObjects(IEnumerable<object> objects);
    }
}
