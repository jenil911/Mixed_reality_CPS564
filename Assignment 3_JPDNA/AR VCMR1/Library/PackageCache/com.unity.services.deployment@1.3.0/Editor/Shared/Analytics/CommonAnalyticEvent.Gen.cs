// WARNING: Auto generated code. Modifications will be lost!
#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Shared.Analytics
{
    [AnalyticInfo(
        eventName: CommonAnalytics.eventName,
        vendorKey: CommonAnalytics.vendorKey,
        version: CommonAnalytics.version)]
    class CommonAnalyticEvent : IAnalytic
    {
        ICommonAnalytics.CommonEventPayload m_Payload;

        public CommonAnalyticEvent(ICommonAnalytics.CommonEventPayload payload)
        {
            m_Payload = payload;
        }

        public bool TryGatherData(out IAnalytic.IData data, out Exception error)
        {
            error = null;
            data = m_Payload;
            return data != null;
        }
    }
}
#endif
