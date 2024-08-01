namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// Interface to provide the type of a deployment item.
    /// </summary>
    public interface ITypedItem
    {
        /// <summary>
        /// Represents the type of the deployment item.
        /// </summary>
        string Type { get; }
    }
}
