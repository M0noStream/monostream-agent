using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoStreamAgent.Common;
using MonoStreamAgent.Readers;
using MonoStreamAgent.Writers;
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
            KafkaConsumer kafka = new KafkaConsumer();
            RabbitProducer rabbit = new RabbitProducer();

            while (!stoppingToken.IsCancellationRequested)
            {
                MonoDTO data = kafka.Read();
                _logger.LogDebug($"Finished Reading from Kafka - {data}");

                rabbit.Write(data);
                _logger.LogDebug($"Finished Writing to Rabbit - {data}");

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
