#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics.Events
{
    class StaticAnalyticEvent
    {
        [AnalyticInfo(
            eventName: StaticAnalytics.EventNameOpenDeploymentWindow,
            vendorKey: AnalyticsConstants.k_VendorKey,
            version: StaticAnalytics.VersionOpen)]
        internal class Open : IAnalytic
        {
            public bool TryGatherData(out IAnalytic.IData data, out Exception error)
            {
                error = null;
                data = null;
                return true;
            }
        }
    }
}
#endif
