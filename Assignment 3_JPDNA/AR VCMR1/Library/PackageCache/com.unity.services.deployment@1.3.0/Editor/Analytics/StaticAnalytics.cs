using Unity.Services.Deployment.Editor.Shared.Analytics;
using Unity.Services.Deployment.Editor.Shared.Logging;

#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
#endif

namespace Unity.Services.Deployment.Editor.Analytics
{
    static class StaticAnalytics
    {
        public const string EventNameOpenDeploymentWindow = "deployment_windowopened";
        public const int VersionOpen = 1;

#if !UNITY_2023_2_OR_NEWER
        public static void RegisterEvents()
        {
            AnalyticsUtils.RegisterEventDefault(EventNameOpenDeploymentWindow, VersionOpen);
            AnalyticsUtils.RegisterEventDefault(CommonAnalytics.eventName, CommonAnalytics.version);
        }

#endif

        public static void SendOpenedEvent()
        {
#if UNITY_2023_2_OR_NEWER
            AnalyticsUtils.SendEvent(new StaticAnalyticEvent.Open());
#else
            AnalyticsUtils.SendEvent(EventNameOpenDeploymentWindow, null, VersionOpen);
#endif
        }

        public static void SendInitializeTiming(string context, int duration)
        {
#if UNITY_2023_2_OR_NEWER
            var analyticProvider = DeploymentServices.Instance.GetService<ICommonAnalyticProvider>();
            var result = new CommonAnalytics(analyticProvider).Send(new ICommonAnalytics.CommonEventPayload
#else
            var result = new CommonAnalytics().Send(new ICommonAnalytics.CommonEventPayload
#endif
            {
                action = "initialize",
                duration = duration,
                context = context
            });
            Logger.LogVerbose($"Initialized {context} {duration}ms");
            Logger.LogVerbose($"Sent Analytics Event: {CommonAnalytics.eventName}.v{CommonAnalytics.version}. Result: {result}");
        }
    }
}
