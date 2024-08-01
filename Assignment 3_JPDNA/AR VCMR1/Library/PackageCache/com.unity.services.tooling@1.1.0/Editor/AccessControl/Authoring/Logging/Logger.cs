namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Logging
{
    class Logger : ILogger
    {
        public void LogError(object message)
        {
            Shared.Logging.Logger.LogError(message);
        }

        public void LogWarning(object message)
        {
            Shared.Logging.Logger.LogWarning(message);
        }

        public void LogInfo(object message)
        {
            Shared.Logging.Logger.Log(message);
        }

        public void LogVerbose(object message)
        {
            Shared.Logging.Logger.LogVerbose(message);
        }
    }
}
