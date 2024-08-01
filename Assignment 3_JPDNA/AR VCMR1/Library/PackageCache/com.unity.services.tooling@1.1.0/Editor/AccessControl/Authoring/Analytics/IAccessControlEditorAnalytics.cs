namespace Unity.Services.Tooling.Editor.AccessControl.Authoring.Analytics
{
    interface IAccessControlEditorAnalytics
    {
        public void SendEvent(
            string action,
            string context = default,
            long duration = default,
            string exception = default);
    }
}
