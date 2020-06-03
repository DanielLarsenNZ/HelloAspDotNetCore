using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace HelloAspDotNetCore
{
    // https://andrewlock.net/exploring-istartupfilter-in-asp-net-core/
    public class DelayStartupFilter : IStartupFilter
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DelayStartupFilter> _log;

        public DelayStartupFilter(IConfiguration config, ILogger<DelayStartupFilter> logger)
        {
            _config = config;
            _log = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            if (!string.IsNullOrEmpty(_config["StartupDelaySeconds"]) && int.TryParse(_config["StartupDelaySeconds"], out int seconds))
            {
                _log.LogDebug($"Starting startup delay of {seconds} seconds");
                Thread.Sleep(seconds * 1000);
                _log.LogDebug($"Finished startup delay of {seconds} seconds");
            }

            return next;
        }
    }
}
