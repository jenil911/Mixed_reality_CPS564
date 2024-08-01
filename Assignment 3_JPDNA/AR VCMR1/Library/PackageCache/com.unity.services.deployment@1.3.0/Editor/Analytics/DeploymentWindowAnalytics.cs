using System;
using UnityEditor;
using Hashing = Unity.Services.Deployment.Editor.Shared.Crypto.Hash;

#if UNITY_2023_2_OR_NEWER
using Unity.Services.Deployment.Editor.Analytics.Events;
using UnityEngine.Analytics;
#endif

namespace Unity.Services.Deployment.Editor.Analytics
{
    class DeploymentWindowAnalytics : IDeploymentWindowAnalytics
    {
        public const string EventNameDoubleClickItem = "deployment_doubleclickitem";
        public const string EventNameContextMenuOpened = "deployment_contextmenuopened";
        public const string EventNameContextMenuSelect = "deployment_contextmenuselect";
        public const int VersionDoubleClick = 1;
        public const int VersionContextMenuOpened = 1;
        public const int VersionContextMenuSelect = 1;

#if UNITY_2023_2_OR_NEWER
        readonly IAnalyticProvider m_AnalyticProvider;

        public DeploymentWindowAnalytics(IAnalyticProvider analyticProvider)
        {
            m_AnalyticProvider = analyticProvider;
        }

#else
        public DeploymentWindowAnalytics()
        {
            AnalyticsUtils.RegisterEventDefault(EventNameDoubleClickItem, VersionDoubleClick);
            AnalyticsUtils.RegisterEventDefault(EventNameContextMenuOpened, VersionContextMenuOpened);
            AnalyticsUtils.RegisterEventDefault(EventNameContextMenuSelect, VersionContextMenuSelect);
        }

#endif

        public void SendDoubleClickEvent(string itemPath)
        {
#if UNITY_2023_2_OR_NEWER
            AnalyticsUtils.SendEvent(
                m_AnalyticProvider.GetAnalytic<DeploymentWindowAnalyticEvent.DoubleClick>(
                    new ItemPathParams(itemPath)));
#else
            AnalyticsUtils.SendEvent(
                EventNameDoubleClickItem,
                new ItemPathParams(itemPath),
                VersionDoubleClick);
#endif
        }

        public void SendContextMenuOpenEvent(string itemPath)
        {
#if UNITY_2023_2_OR_NEWER
            AnalyticsUtils.SendEvent(
                m_AnalyticProvider.GetAnalytic<DeploymentWindowAnalyticEvent.ContextMenuOpened>(
                    new ItemPathParams(itemPath)));
#else
            AnalyticsUtils.SendEvent(
                EventNameContextMenuOpened,
                new ItemPathParams(itemPath),
                VersionContextMenuOpened);
#endif
        }

        public void SendContextMenuSelectEvent(string itemPath)
        {
#if UNITY_2023_2_OR_NEWER
            AnalyticsUtils.SendEvent(
                m_AnalyticProvider.GetAnalytic<DeploymentWindowAnalyticEvent.ContextMenuSelect>(
                    new ItemPathParams(itemPath)));
#else
            AnalyticsUtils.SendEvent(
                EventNameContextMenuSelect,
                new ItemPathParams(itemPath),
                VersionContextMenuSelect);
#endif
        }

        // Lowercase to match the naming schema
        [Serializable]
        public struct ItemPathParams
#if UNITY_2023_2_OR_NEWER
            : IAnalytic.IData
#endif
        {
            static readonly GUID k_NullGUID = new GUID();

            public string itemName;

            public ItemPathParams(string itemPath)
            {
                var assetGuid = AssetDatabase.GUIDFromAssetPath(itemPath);
                this.itemName = Hashing.SHA1(assetGuid == k_NullGUID ? itemPath : assetGuid.ToString());
            }
        }
    }
}
