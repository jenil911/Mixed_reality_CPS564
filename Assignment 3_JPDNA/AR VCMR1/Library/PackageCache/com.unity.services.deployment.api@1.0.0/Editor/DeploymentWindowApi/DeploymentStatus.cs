using System;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// A struct that represents the deployment status description of an item, relative
    /// to its 'live' counterpart, and the latest deployment.
    /// This should NOT be used to represent the validity of the item itself,
    /// or other internal validations, use AssetState for that purpose.
    /// </summary>
    [Serializable]
    public struct DeploymentStatus
    {
        //n.b.: Do not mark as readonly. Required for reload domain persistence
        string m_Message;
        string m_MessageDetail;
        SeverityLevel m_MessageSeverity;

        /// <summary>
        /// A status to represent an item that is up to date with the remote.
        /// </summary>
        public static readonly DeploymentStatus UpToDate = new DeploymentStatus("Up to date", string.Empty, SeverityLevel.Success);
        /// <summary>
        /// A status to represent an item that was modified locally.
        /// </summary>
        public static readonly DeploymentStatus ModifiedLocally = new DeploymentStatus("Modified locally, deploy to update", string.Empty, SeverityLevel.Warning);
        /// <summary>
        ///A status to represent an item that failed to deploy.
        /// </summary>
        public static readonly DeploymentStatus FailedToDeploy = new DeploymentStatus("Failed to deploy", string.Empty, SeverityLevel.Error);
        /// <summary>
        /// An empty status.
        /// </summary>
        public static readonly DeploymentStatus Empty = new DeploymentStatus(string.Empty, string.Empty, SeverityLevel.None);

        /// <summary>
        /// Message associated with a deployment result
        /// </summary>
        public string Message => m_Message;

        /// <summary>
        /// Details associated with a deployment result.
        /// </summary>
        public string MessageDetail => m_MessageDetail;

        /// <summary>
        /// Severity of the deployment message level.
        /// </summary>
        public SeverityLevel MessageSeverity => m_MessageSeverity;

        /// <summary>
        /// Creates a DeploymentStatus struct
        /// </summary>
        /// <param name="message">The DeploymentStatus message if any</param>
        /// <param name="messageDetail">Details regarding the message</param>
        /// <param name="messageSeverity">The severity of the message</param>
        public DeploymentStatus(
            string message = null,
            string messageDetail = null,
            SeverityLevel messageSeverity = SeverityLevel.None)
        {
            m_Message = message;
            m_MessageDetail = messageDetail;
            m_MessageSeverity = messageSeverity;
        }
    }
}
