using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sylvanas
{
    /// <summary>
    /// Acts as a base class for hosted bots.
    /// </summary>
    /// <typeparam name="TBotService">The implementing bot service.</typeparam>
    public abstract class HostedBotService<TBotService> : IHostedService
        where TBotService : HostedBotService<TBotService>
    {
        /// <summary>
        /// Gets the available services.
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Gets the application lifetime.
        /// </summary>
        protected IHostApplicationLifetime Lifetime { get; }

        /// <summary>
        /// Gets the logging instance for this service.
        /// </summary>
        protected ILogger<TBotService> Log { get; }

        /// <summary>
        /// Gets the host environment for this service.
        /// </summary>
        protected IHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the host configuration for this service.
        /// </summary>
        protected IConfiguration HostConfiguration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedBotService{TBotService}"/> class.
        /// </summary>
        /// <param name="hostConfiguration">The host configuration.</param>
        /// <param name="hostEnvironment">The host environment.</param>
        /// <param name="log">The logging instance.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="services">The available services.</param>
        protected HostedBotService
        (
            IConfiguration hostConfiguration,
            IHostEnvironment hostEnvironment,
            ILogger<TBotService> log,
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider services
        )
        {
            HostConfiguration = hostConfiguration;
            Log = log;
            Lifetime = applicationLifetime;
            Services = services;
            HostEnvironment = hostEnvironment;
        }

        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}