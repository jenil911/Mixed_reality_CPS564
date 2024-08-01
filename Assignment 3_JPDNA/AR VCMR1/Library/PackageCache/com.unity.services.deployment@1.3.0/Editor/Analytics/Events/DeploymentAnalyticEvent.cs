#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics.Events
{
    [AnalyticInfo(
        eventName: DeploymentAnalytics.EventNameDeploy,
        vendorKey: AnalyticsConstants.k_VendorKey,
        version: DeploymentAnalytics.VersionDeploy)]
    class DeploymentAnalyticEvent : AnalyticBase {}
}
#endif
