#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics.Events
{
    [AnalyticInfo(
        eventName: DeployOnPlayAnalytics.EventNameDeployOnPlay,
        vendorKey: AnalyticsConstants.k_VendorKey,
        version: DeployOnPlayAnalytics.VersionDeployOnPlay)]
    class DeployOnPlayAnalyticEvent : AnalyticBase {}
}
#endif
