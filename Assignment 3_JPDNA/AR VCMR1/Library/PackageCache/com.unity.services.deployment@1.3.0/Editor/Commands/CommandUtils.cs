using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Services.Deployment.Editor.Commands
{
    static class CommandUtils
    {
        public static DropdownMenuAction.Status GetMenuItemStatus(ICommand command, List<object> modelSelection)
        {
            DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal;
            if (!command.IsVisible(modelSelection))
            {
                status = DropdownMenuAction.Status.Hidden;
            }
            else if (!command.IsEnabled(modelSelection))
            {
                status = DropdownMenuAction.Status.Disabled;
            }

            return status;
        }
    }
}
