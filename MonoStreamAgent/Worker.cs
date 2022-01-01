using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoStreamAgent.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MonoStreamAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Mayan test");
            KafkaConnector kafka = new KafkaConnector();

            kafka.Consume();
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}
