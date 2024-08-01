using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.DeploymentApi.Editor
{
    /// <summary>
    /// Represents a command to execute on a deployment item.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Represents command's name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Defines the method to be called when a command is invoked.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <param name="cancellationToken">Cancellation token to cancel command invocation</param>
        /// <returns>A task representing the invocation result</returns>
        public abstract Task ExecuteAsync(IEnumerable<IDeploymentItem> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// The method to determine whether the command is enabled in the context menu.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <returns>A bool representing the enablement status</returns>
        public virtual bool IsEnabled(IEnumerable<IDeploymentItem> items) => true;

        /// <summary>
        /// The method to determine whether the command is visible in the context menu.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <returns>A bool representing the visibility status</returns>
        public virtual bool IsVisible(IEnumerable<IDeploymentItem> items) => true;
    }

    /// <summary>
    /// Represents a command to execute on a deployment item.
    /// </summary>
    /// <typeparam name="T">Deployment Item Type</typeparam>
    public abstract class Command<T> : Command where T : IDeploymentItem
    {
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="items">The target items of the invocation.</param>
        /// <param name="cancellationToken">Cancellation token to cancel command invocation.</param>
        /// <returns>A task representing invocation result</returns>
        public abstract Task ExecuteAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Defines the method to be called when a command is invoked.
        /// </summary>
        /// <param name="items">The target items of the invocation.</param>
        /// <returns>A task representing the invocation result.</returns>
        public virtual bool IsEnabled(IEnumerable<T> items) => true;

        /// <summary>
        /// The method to determine whether the command is visible in the context menu.
        /// </summary>
        /// <param name="items">The target items of the invocation.</param>
        /// <returns>A bool representing the visibility status.</returns>
        public virtual bool IsVisible(IEnumerable<T> items) => true;

        /// <summary>
        /// Defines the method to be called when a command is invoked.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <param name="cancellationToken">Cancellation token to cancel command invocation</param>
        /// <returns>A task representing the invocation result</returns>
        public sealed override Task ExecuteAsync(IEnumerable<IDeploymentItem> items, CancellationToken cancellationToken = default)
        {
            return ExecuteAsync(items.Cast<T>(), cancellationToken);
        }

        /// <summary>
        /// The method to determine whether the command is enabled in the context menu.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <returns>A bool representing the enablement status</returns>
        public sealed override bool IsEnabled(IEnumerable<IDeploymentItem> items) => IsEnabled(items.Cast<T>());

        /// <summary>
        /// The method to determine whether the command is visible in the context menu.
        /// </summary>
        /// <param name="items">The target items of the invocation</param>
        /// <returns>A bool representing the visibility status</returns>
        public sealed override bool IsVisible(IEnumerable<IDeploymentItem> items) => IsVisible(items.Cast<T>());
    }
}
