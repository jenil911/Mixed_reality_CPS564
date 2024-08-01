#if UNITY_2023_2_OR_NEWER
using UnityEngine.Analytics;

namespace Unity.Services.Tooling.Editor.Shared.Analytics
{
    interface ICommonAnalyticProvider
    {
        IAnalytic GetAnalytic(ICommonAnalytics.CommonEventPayload payload);
    }
}
#endif
