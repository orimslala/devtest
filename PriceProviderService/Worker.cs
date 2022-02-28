using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceProviderService.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PriceProviderService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileReaderService _service;
        private IHubContext<PricesHub> _hub;
        public Worker(FileReaderService service, IHubContext<PricesHub> hub, ILogger<Worker> logger)
        {
            _service = service;
            _hub = hub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _service.Read();
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
