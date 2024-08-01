namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// This interface represents a container for the current selected environment.
    /// </summary>
    public interface IEnvironmentProvider
    {
        /// <summary>
        /// Environment Id of a currently selected environment.
        /// </summary>
        string Current { get; }
    }
}
