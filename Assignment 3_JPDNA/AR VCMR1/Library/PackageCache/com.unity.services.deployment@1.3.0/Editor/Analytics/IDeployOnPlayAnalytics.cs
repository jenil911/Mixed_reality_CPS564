using System;

namespace Unity.Services.Deployment.Editor.Analytics
{
    interface IDeployOnPlayAnalytics
    {
        IDisposable GetEventScope();
    }
}
