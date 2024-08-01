using System;
using System.Threading.Tasks;

namespace Unity.Services.Deployment.Editor.PlayMode
{
    interface IPlayModeInterrupt
    {
        void OnPlay(params Func<Task>[] actions);
    }
}
