#if UNITY_2023_2_OR_NEWER
using UnityEngine.Analytics;

namespace Unity.Services.Tooling.Editor.Shared.Analytics
{
    class CommonAnalyticProvider : ICommonAnalyticProvider
    {
        public IAnalytic GetAnalytic(ICommonAnalytics.CommonEventPayload payload)
        {
            return new CommonAnalyticEvent(payload);
        }
    }
}
#endif
