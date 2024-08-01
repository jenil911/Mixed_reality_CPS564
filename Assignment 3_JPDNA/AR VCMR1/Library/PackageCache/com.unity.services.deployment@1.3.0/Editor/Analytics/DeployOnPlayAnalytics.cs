using System;
using Unity.Services.Deployment.Editor.Shared.Analytics;

#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
using UnityEngine.Analytics;
#endif

namespace Unity.Services.Deployment.Editor.Analytics
{
    class DeployOnPlayAnalytics : IDeployOnPlayAnalytics
    {
        public const string EventNameDeployOnPlay = "deployment_deployonplay";
        public const int VersionDeployOnPlay = 1;

#if UNITY_2023_2_OR_NEWER
        readonly IAnalyticProvider m_AnalyticProvider;

        public DeployOnPlayAnalytics(IAnalyticProvider analyticProvider)
        {
            m_AnalyticProvider = analyticProvider;
        }

#else
        public DeployOnPlayAnalytics()
        {
            AnalyticsUtils.RegisterEventDefault(EventNameDeployOnPlay, VersionDeployOnPlay);
        }

#endif

        internal void SendDeployOnPlayEvent(int duration)
        {
            var evt = new DeployOnPlayEvent()
            {
                msDuration = duration
            };

#if UNITY_2023_2_OR_NEWER
            AnalyticsUtils.SendEvent(m_AnalyticProvider.GetAnalytic<DeployOnPlayAnalyticEvent>(evt));
#else
            AnalyticsUtils.SendEvent(EventNameDeployOnPlay, evt, VersionDeployOnPlay);
#endif
        }

        public IDisposable GetEventScope()
        {
            return new DeployOnPlayScope(this);
        }

        struct DeployOnPlayScope : IDisposable
        {
            readonly AnalyticsTimer m_Timer;
            public DeployOnPlayScope(DeployOnPlayAnalytics parent)
            {
                m_Timer = new AnalyticsTimer(parent.SendDeployOnPlayEvent);
            }

            public void Dispose()
            {
                m_Timer.Dispose();
            }
        }

        [Serializable]
        struct DeployOnPlayEvent
#if UNITY_2023_2_OR_NEWER
            : IAnalytic.IData
#endif
        {
            public int msDuration;
        }
    }
}
