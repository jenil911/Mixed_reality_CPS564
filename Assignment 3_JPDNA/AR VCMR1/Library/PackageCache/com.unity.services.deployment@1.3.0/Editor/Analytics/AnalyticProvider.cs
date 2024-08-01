#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics
{
    class AnalyticProvider : IAnalyticProvider
    {
        public IAnalytic GetAnalytic<T>(IAnalytic.IData data) where T : AnalyticBase, new()
        {
            var analytic = new T();
            analytic.SetData(data);
            return analytic;
        }
    }
}
#endif
