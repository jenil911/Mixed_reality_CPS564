// WARNING: Auto generated code. Modifications will be lost!
using System;
using System.Diagnostics;

namespace Unity.Services.Deployment.Editor.Shared.Analytics
{
    class AnalyticsTimer : IDisposable
    {
        readonly Stopwatch m_Stopwatch;
        readonly Action<int> m_DurationHandler;

        public AnalyticsTimer(Action<int> durationHandler)
        {
            m_Stopwatch = new Stopwatch();
            m_DurationHandler = durationHandler;
            m_Stopwatch.Start();
        }

        public void Dispose()
        {
            m_Stopwatch.Stop();
            m_DurationHandler((int)m_Stopwatch.ElapsedMilliseconds);
        }
    }
}
