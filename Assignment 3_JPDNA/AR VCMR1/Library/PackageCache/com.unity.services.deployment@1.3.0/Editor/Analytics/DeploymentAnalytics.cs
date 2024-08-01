using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.Deployment.Editor.Shared.Analytics;
using Unity.Services.DeploymentApi.Editor;

#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
using UnityEngine.Analytics;
#endif

namespace Unity.Services.Deployment.Editor.Analytics
{
    class DeploymentAnalytics : IDeploymentAnalytics
    {
        public const string EventNameDeploy = "deployment_deployed";
        const string k_EventNameDDefDeployed = "deployment_definition_deployed";
        public const int VersionDeploy = 2;

        readonly IEnvironmentsApi m_EnvironmentService;
        readonly ICommonAnalytics m_CommonAnalytics;
#if UNITY_2023_2_OR_NEWER
        readonly IAnalyticProvider m_AnalyticProvider;
#endif

        public DeploymentAnalytics(
            IEnvironmentsApi environmentService,
            ICommonAnalytics commonAnalytics
#if UNITY_2023_2_OR_NEWER
            ,
            IAnalyticProvider analyticProvider
#endif
        )
        {
            m_EnvironmentService = environmentService;
            m_CommonAnalytics = commonAnalytics;
#if UNITY_2023_2_OR_NEWER
            m_AnalyticProvider = analyticProvider;
#else
            AnalyticsUtils.RegisterEventDefault(EventNameDeploy, VersionDeploy);
#endif
        }

        public IDeploymentAnalytics.IDeployEvent BeginDeploy(IReadOnlyDictionary<string, List<IDeploymentItem>> itemsPerProvider, string source)
        {
            return new DeployEvent(
                m_EnvironmentService.ActiveEnvironmentId,
                itemsPerProvider,
                source
#if UNITY_2023_2_OR_NEWER
                ,
                m_AnalyticProvider
#endif
            );
        }

        public void SendDeploymentDefinitionDeployedEvent(int itemsNumber)
        {
            m_CommonAnalytics.Send(new ICommonAnalytics.CommonEventPayload()
            {
                action = k_EventNameDDefDeployed,
                count = itemsNumber
            });
        }

        class DeployEvent : IDeploymentAnalytics.IDeployEvent
        {
            readonly IReadOnlyDictionary<string, List<IDeploymentItem>> m_ItemsPerProvider;
            readonly Stopwatch m_Stopwatch = new();
            readonly Guid? m_Environment;
            readonly string m_Source;
#if UNITY_2023_2_OR_NEWER
            readonly IAnalyticProvider m_AnalyticProvider;
#endif

            public DeployEvent(
                Guid? environment,
                IReadOnlyDictionary<string, List<IDeploymentItem>> itemsPerProvider,
                string source
#if UNITY_2023_2_OR_NEWER
                ,
                IAnalyticProvider analyticProvider
#endif
            )
            {
                m_Environment = environment;
                m_ItemsPerProvider = itemsPerProvider;
                m_Source = source;
                m_Stopwatch.Start();
#if UNITY_2023_2_OR_NEWER
                m_AnalyticProvider = analyticProvider;
#endif
            }

            public void SendSuccess()
            {
#if UNITY_2023_2_OR_NEWER
                AnalyticsUtils.SendEvent(
                    m_AnalyticProvider.GetAnalytic<DeploymentAnalyticEvent>(CreateDeployEvent("success")));
#else
                AnalyticsUtils.SendEvent(
                    EventNameDeploy, CreateDeployEvent("success"), VersionDeploy);
#endif
            }

            public void SendFailure(Exception exception)
            {
#if UNITY_2023_2_OR_NEWER
                AnalyticsUtils.SendEvent(
                    m_AnalyticProvider.GetAnalytic<DeploymentAnalyticEvent>(CreateDeployEvent("failure")));
#else
                AnalyticsUtils.SendEvent(
                    EventNameDeploy, CreateDeployEvent("failure", exception), VersionDeploy);
#endif
            }

            DeployEventPayload CreateDeployEvent(string status, Exception exception = null)
            {
                return new DeployEventPayload
                {
                    status = status,
                    environment = m_Environment?.ToString(),
                    duration = m_Stopwatch.ElapsedMilliseconds,
                    exception = exception?.GetType().ToString(),
                    source = m_Source,
                    providers = m_ItemsPerProvider.Keys.Select(name =>
                        new DeployEventPayload.Provider
                        {
                            name = name,
                            item_count = m_ItemsPerProvider[name].Count,
                            failure_count = m_ItemsPerProvider[name].Count(i => i.Status.MessageSeverity == SeverityLevel.Error)
                        }
                        ).ToList()
                };
            }
        }

        [Serializable]
        // Naming exception to the standard in order to match the schema
        // ReSharper disable InconsistentNaming
        internal struct DeployEventPayload
#if UNITY_2023_2_OR_NEWER
            : IAnalytic.IData
#endif
        {
            public string environment;
            public string status;
            public string exception;
            public string source;
            public long duration;
            public List<Provider> providers;

            [Serializable]
            public struct Provider
            {
                public string name;
                public int item_count;
                public int failure_count;
            }
        }
        // ReSharper restore InconsistentNaming
    }
}
