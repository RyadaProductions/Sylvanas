using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours;
using Remora.Behaviours.Services;
using Remora.Discord.Hosted;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Services;
using Remora.Results;

#pragma warning disable SA1118 // Parameter spans multiple lines, big strings

// ReSharper disable RedundantDefaultMemberInitializer - suppressions for indirectly initialized properties.
namespace Sylvanas
{
    /// <summary>
    /// Main service for the bot itself. Handles high-level functionality.
    /// </summary>
    public class SylvanasBotService : HostedDiscordBotService<SylvanasBotService>
    {
        private readonly DiscordSocketClient _client;
        private readonly BehaviourService _behaviours;
        private readonly PluginService _pluginService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SylvanasBotService"/> class.
        /// </summary>
        /// <param name="discordClient">The Discord client.</param>
        /// <param name="commandService">The command service.</param>
        /// <param name="behaviourService">The behaviour service.</param>
        /// <param name="pluginService">The plugin service.</param>
        /// <param name="hostConfiguration">The host configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="services">The available services.</param>
        public SylvanasBotService
        (
            DiscordSocketClient discordClient,
            CommandService commandService,
            BehaviourService behaviourService,
            PluginService pluginService,
            IConfiguration hostConfiguration,
            IHostEnvironment hostEnvironment,
            ILogger<SylvanasBotService> log,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services
        )
            : base(discordClient, pluginService, behaviourService, hostConfiguration, hostEnvironment, log, applicationLifetime, services)
        {
            _client = discordClient;
            _behaviours = behaviourService;
            _pluginService = pluginService;

            _client.Log += OnDiscordLogEvent;
            commandService.Log += OnDiscordLogEvent;
        }

        protected override async Task<RetrieveEntityResult<string>> GetTokenAsync()
        {
            var contentService = Services.GetRequiredService<ContentService>();

            var getTokenResult = await contentService.GetBotTokenAsync();
            if (!getTokenResult.IsSuccess)
            {
                return RetrieveEntityResult<string>.FromError(getTokenResult.ErrorReason);
            }
            return RetrieveEntityResult<string>.FromSuccess(getTokenResult.Entity);
        }
    }
}