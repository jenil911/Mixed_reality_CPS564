#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics
{
    interface IAnalyticProvider
    {
        IAnalytic GetAnalytic<T>(IAnalytic.IData data) where T : AnalyticBase, new();
    }
}
#endif
