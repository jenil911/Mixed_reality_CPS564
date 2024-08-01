namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// Class containing extension methods to make it easier to manipulate the deployment status of an item.
    /// </summary>
    public static class DeploymentItemExtensions
    {
        /// <summary>
        /// Extension method to set the detail of the Deployment Item status.
        /// </summary>
        /// <param name="self">The target of the invocation.</param>
        /// <param name="detail">The detail to set.</param>
        public static void SetStatusDetail(this IDeploymentItem self, string detail)
        {
            var originalStatus = self.Status;
            self.Status = new DeploymentStatus(originalStatus.Message, detail, originalStatus.MessageSeverity);
        }

        /// <summary>
        /// Extension method to set the detail of the Deployment Item description.
        /// </summary>
        /// <param name="self">The target of the invocation.</param>
        /// <param name="description">The description to set.</param>
        public static void SetStatusDescription(this IDeploymentItem self, string description)
        {
            var originalStatus = self.Status;
            self.Status = new DeploymentStatus(description, originalStatus.MessageDetail, originalStatus.MessageSeverity);
        }

        /// <summary>
        /// Extension method to set the detail of the Deployment Item severityLevel.
        /// </summary>
        /// <param name="self">The target of the invocation.</param>
        /// <param name="severityLevel">The severityLevel to set.</param>
        public static void SetStatusSeverity(this IDeploymentItem self, SeverityLevel severityLevel)
        {
            var originalStatus = self.Status;
            self.Status = new DeploymentStatus(originalStatus.MessageDetail, originalStatus.Message, severityLevel);
        }
    }
}
