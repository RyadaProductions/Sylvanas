using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Plugins.Abstractions;

namespace Sylvanas.Plugins.Abstractions.Database
{
    /// <summary>
    /// Represents the public API of a plugin supporting migrations.
    /// </summary>
    [PublicAPI]
    public interface IMigratablePlugin : IPluginDescriptor
    {
        /// <summary>
        /// Performs any migrations required by the plugin.
        /// </summary>
        /// <param name="serviceProvider">The available services.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [NotNull]
        Task<bool> MigratePluginAsync([NotNull] IServiceProvider serviceProvider);

        /// <summary>
        /// Determines whether the database has been created.
        /// </summary>
        /// <param name="serviceProvider">The available services.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [NotNull]
        Task<bool> IsDatabaseCreatedAsync([NotNull] IServiceProvider serviceProvider);
    }
}