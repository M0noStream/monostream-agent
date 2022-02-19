using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoStreamAgent.Common;
using MonoStreamAgent.Readers;
using MonoStreamAgent.Writers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MonoStreamAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ConcurrentQueue<MonoDTO> _messages;
        private Task srcTask;
        private Task dstTask;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _messages = new ConcurrentQueue<MonoDTO>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            int maxInnerQueueDepth = 100;
            srcTask = Task.Factory.StartNew(() => 
            {
                KafkaConsumer srcKafka = new KafkaConsumer();
                while (!cancellationToken.IsCancellationRequested)
                {
                    while (_messages.Count < maxInnerQueueDepth)
                    {
                        _messages.Enqueue(srcKafka.Read());
                    }
                    Task.Delay(10, cancellationToken);
                }
            }, cancellationToken);

            dstTask = Task.Factory.StartNew(() =>
            {
                RabbitProducer rabbit = new RabbitProducer();
                while (!cancellationToken.IsCancellationRequested || _messages.Count > 0)
                {
                    while (_messages.TryDequeue(out MonoDTO message))
                    {
                        rabbit.Write(message);
                    }
                    Task.Delay(10, cancellationToken);
                }
            }, cancellationToken);
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[ExecuteAsync] Not sure why we need it, but i must override this func");
            await Task.Delay(10, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            srcTask.Wait();
            dstTask.Wait();
            return base.StopAsync(cancellationToken);
        }
    }
}
