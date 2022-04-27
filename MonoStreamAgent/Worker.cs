using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonoStreamAgent.Common;
using MonoStreamAgent.Readers;
using MonoStreamAgent.Writers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MonoStreamAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppData _appData;
        private ConcurrentQueue<MonoDTO> _messages;
        private Task srcTask;
        private Task dstTask;

        public Worker(ILogger<Worker> logger,
                      IOptions<AppData> options)
        {
            _appData = options.Value;
            _logger = logger;
            _messages = new ConcurrentQueue<MonoDTO>();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            InitSource(cancellationToken);
            InitDestination(cancellationToken);

            return base.StartAsync(cancellationToken);
        }

        private void InitSource(CancellationToken cancellationToken)
        {
            int maxInnerQueueDepth = 100;
            srcTask = Task.Factory.StartNew(() =>
            {
                IDataReader consumer;

                switch (_appData.Source.TypeName)
                {
                    case DataPlatformEnum.Kafka:
                        {
                            consumer = new KafkaConsumer();
                            break;
                        }
                    case DataPlatformEnum.RabbitMQ:
                        {

                            consumer = new RabbitConsumer();
                            break;
                        }
                    default:
                        throw new ArgumentException("Unknown Data Platform");
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    while (_messages.Count < maxInnerQueueDepth)
                    {
                        _messages.Enqueue(consumer.Read());
                    }
                    Task.Delay(10, cancellationToken);
                }
            }, cancellationToken);

        }

        private void InitDestination(CancellationToken cancellationToken)
        {
            dstTask = Task.Factory.StartNew(() =>
            {
                IDataWriter producer;

                switch (_appData.Destination.TypeName)
                {
                    case DataPlatformEnum.Kafka:
                        {
                            producer = new KafkaProducer();
                            break;
                        }
                    case DataPlatformEnum.RabbitMQ:
                        {

                            producer = new RabbitProducer();
                            break;
                        }
                    default:
                        throw new ArgumentException("Unknown Data Platform");
                }

                while (!cancellationToken.IsCancellationRequested || _messages.Count > 0)
                {
                    while (_messages.TryDequeue(out MonoDTO message))
                    {
                        producer.Write(message);
                    }
                    Task.Delay(10, cancellationToken);
                }
            }, cancellationToken);
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
