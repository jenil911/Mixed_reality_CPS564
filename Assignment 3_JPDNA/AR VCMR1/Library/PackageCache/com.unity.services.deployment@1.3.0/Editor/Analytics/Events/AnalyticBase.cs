#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics.Events
{
    class AnalyticBase : IAnalytic
    {
        internal IAnalytic.IData Data;

        public void SetData(IAnalytic.IData data)
        {
            Data = data;
        }

        public bool TryGatherData(out IAnalytic.IData data, out Exception error)
        {
            error = null;
            data = Data;
            return data != null;
        }
    }
}
#endif
