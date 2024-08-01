namespace Unity.Services.Deployment.Editor.Analytics
{
    interface IDeploymentWindowAnalytics
    {
        void SendDoubleClickEvent(string itemPath);
        void SendContextMenuOpenEvent(string itemPath);
        void SendContextMenuSelectEvent(string itemPath);
    }
}
