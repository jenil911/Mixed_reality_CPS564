using Unity.Services.Deployment.Editor.Shared.EditorUtils;
using Unity.Services.Deployment.Editor.Shared.Logging;
using UnityEditor;

#if UNITY_2023_2_OR_NEWER
using UnityEngine.Analytics;
#endif

namespace Unity.Services.Deployment.Editor.Analytics
{
    static class AnalyticsUtils
    {
#if UNITY_2023_2_OR_NEWER
        public static void SendEvent(IAnalytic analytic)
        {
            EditorAnalytics.SendAnalytic(analytic);
        }

#else
        public static void RegisterEventDefault(string eventName, int version = 1)
        {
            Sync.RunNextUpdateOnMain(() =>
            {
                var result = EditorAnalytics.RegisterEventWithLimit(
                    eventName,
                    AnalyticsConstants.k_MaxEventPerHour,
                    AnalyticsConstants.k_MaxItems,
                    AnalyticsConstants.k_VendorKey,
                    version);

                Logger.LogVerbose($"Registered Analytics: {eventName}.v{version}. Result: {result}");
            });
        }

        public static void SendEvent(string name, object parameters, int version)
        {
            var result = EditorAnalytics.SendEventWithLimit(name, parameters, version);
            Logger.LogVerbose($"Sent Analytics Event: {name}.v{version}. Result: {result}");
        }

#endif
    }
}
