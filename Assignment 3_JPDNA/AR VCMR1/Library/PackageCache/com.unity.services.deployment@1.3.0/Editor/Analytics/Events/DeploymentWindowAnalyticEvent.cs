#if UNITY_2023_2_OR_NEWER
using System;
using UnityEngine.Analytics;

namespace Unity.Services.Deployment.Editor.Analytics.Events
{
    class DeploymentWindowAnalyticEvent
    {
        [AnalyticInfo(
            eventName: DeploymentWindowAnalytics.EventNameDoubleClickItem,
            vendorKey: AnalyticsConstants.k_VendorKey,
            version: DeploymentWindowAnalytics.VersionDoubleClick)]
        internal class DoubleClick : AnalyticBase {}

        [AnalyticInfo(
            eventName: DeploymentWindowAnalytics.EventNameContextMenuSelect,
            vendorKey: AnalyticsConstants.k_VendorKey,
            version: DeploymentWindowAnalytics.VersionContextMenuSelect)]
        internal class ContextMenuSelect : AnalyticBase {}

        [AnalyticInfo(
            eventName: DeploymentWindowAnalytics.EventNameContextMenuOpened,
            vendorKey: AnalyticsConstants.k_VendorKey,
            version: DeploymentWindowAnalytics.VersionContextMenuOpened)]
        internal class ContextMenuOpened : AnalyticBase {}
    }
}
#endif
