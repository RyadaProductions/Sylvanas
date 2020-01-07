using System;
using System.Threading.Tasks;
using Sylvanas.Plugins.JumboEmotes;
using Sylvanas.Plugins.JumboEmotes.CommandModules;
using Discord.Commands;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;

[assembly: RemoraPlugin(typeof(JumboEmotesPlugin))]

namespace Sylvanas.Plugins.JumboEmotes
{
    /// <summary>
    /// Describes the JumboEmotes plugin.
    /// </summary>
    [PublicAPI]
    public sealed class JumboEmotesPlugin : PluginDescriptor
    {
        /// <inheritdoc />
        public override string Name => "JumboEmotes";

        /// <inheritdoc />
        public override string Description => "Provides a command for jumbofying emotes.";

        /// <inheritdoc />
        public override async Task<bool> InitializeAsync(IServiceProvider serviceProvider)
        {
            var commands = serviceProvider.GetRequiredService<CommandService>();
            await commands.AddModuleAsync<JumboCommands>(serviceProvider);

            return true;
        }
    }
}