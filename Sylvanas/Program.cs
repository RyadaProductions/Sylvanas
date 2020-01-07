using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Behaviours.Services;
using Remora.Plugins.Services;
using Sylvanas.Core.Services;
using Sylvanas.Plugins.JumboEmotes;

namespace Sylvanas
{
    internal class Program
    {
        public static async Task Main()
        {
            // Configure logging
            const string configurationName = "Sylvanas.log4net.config";
            var logConfig = new XmlDocument();
            await using (var configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(configurationName))
            {
                if (configStream is null)
                {
                    throw new InvalidOperationException("The log4net configuration stream could not be found.");
                }

                logConfig.Load(configStream);
            }

            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(Hierarchy));
            XmlConfigurator.Configure(repo, logConfig["log4net"]);

            var hostBuilder = Host.CreateDefaultBuilder()
                .UseSystemd()
                .ConfigureServices(services =>
                {
                    var pluginService = new PluginService();

                    services
                        .AddHostedService<SylvanasBotService>()
                        .AddSingleton(pluginService)
                        .AddSingleton
                        (
                            provider => new DiscordSocketClient(new DiscordSocketConfig { MessageCacheSize = 100 })
                        )
                        .AddSingleton<IDiscordClient>(s => s.GetRequiredService<DiscordSocketClient>())
                        .AddSingleton<BaseSocketClient>(s => s.GetRequiredService<DiscordSocketClient>())
                        .AddSingleton<CommandService>()
                        .AddSingleton<BehaviourService>()
                        .AddSingleton<ContentService>()
                        //.AddSingleton<DiscordService>()
                        .AddSingleton<UserFeedbackService>()
                        // .AddSingleton<InteractivityService>()
                        .AddSingleton<DelayedActionService>()
                        // .AddSingleton<SchemaAwareDbContextService>()
                        // .AddSingleton<ContextConfigurationService>()
                        .AddSingleton(FileSystemFactory.CreateContentFileSystem())
                        .AddSingleton<Random>();

                    var plugins = pluginService.LoadAvailablePlugins();
                    foreach (var plugin in plugins)
                    {
                        plugin.ConfigureServices(services);
                    }
                })
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();

                    l.AddLog4Net()
                        .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning)
                        .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
                        .AddFilter("Microsoft.EntityFrameworkCore.Migrations", LogLevel.Warning);
                });

            var host = hostBuilder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Running on {RuntimeInformation.FrameworkDescription}");

            await host.RunAsync();
        }
    }
}