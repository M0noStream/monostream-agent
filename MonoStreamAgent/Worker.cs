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
            if (_appData.IsTransacted)
            {
                InitTransaction(cancellationToken);
            }
            else
            {
                InitSource(cancellationToken);
                InitDestination(cancellationToken);
            }

            return base.StartAsync(cancellationToken);
        }

        private void InitSource(CancellationToken cancellationToken)
        {
            srcTask = Task.Factory.StartNew(() =>
            {
                IDataReader consumer = InitConsumer();
                int maxInnerQueueDepth = _appData.InnerQueueDepth;

                while (!cancellationToken.IsCancellationRequested)
                {
                    while (_messages.Count < maxInnerQueueDepth)
                    {
                        _messages.Enqueue(consumer.Read());
                    }
                    SpinWait.SpinUntil(() => _messages.Count < maxInnerQueueDepth);
                }
            }, cancellationToken);
        }

        private void InitDestination(CancellationToken cancellationToken)
        {
            dstTask = Task.Factory.StartNew(() =>
            {
                IDataWriter producer = InitProducer();

                while (!cancellationToken.IsCancellationRequested || _messages.Count > 0)
                {
                    while (_messages.TryDequeue(out MonoDTO message))
                    {
                        producer.Write(message);
                    }
                    SpinWait.SpinUntil(() => _messages.TryPeek(out MonoDTO tmp));
                }
            }, cancellationToken);
        }

        private void InitTransaction(CancellationToken cancellationToken)
        {
            var transactedTask = Task.Factory.StartNew(() =>
                {
                    IDataReader consumer = InitConsumer();
                    IDataWriter producer = InitProducer();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        producer.Write(consumer.Read());
                    }
                }, cancellationToken);
        }

        private IDataReader InitConsumer()
        {
            IDataReader consumer;
            switch (_appData.Source.TypeName)
            {
                case DataPlatformEnum.Kafka:
                    {
                        consumer = new KafkaConsumer(_appData.Source);
                        break;
                    }
                case DataPlatformEnum.RabbitMQ:
                    {
                        consumer = new RabbitConsumer(_appData.Source);
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown Data Platform");
            }

            return consumer;
        }

        private IDataWriter InitProducer()
        {
            IDataWriter producer;
            switch (_appData.Destination.TypeName)
            {
                case DataPlatformEnum.Kafka:
                    {
                        producer = new KafkaProducer(_appData.Destination);
                        break;
                    }
                case DataPlatformEnum.RabbitMQ:
                    {

                        producer = new RabbitProducer(_appData.Destination);
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown Data Platform");
            }

            return producer;
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
