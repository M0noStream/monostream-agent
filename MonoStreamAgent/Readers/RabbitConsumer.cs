using System;
using System.Text;
using Microsoft.Extensions.Logging;
using MonoStreamAgent.Common;
using RabbitMQ.Client;

namespace MonoStreamAgent.Readers
{
    public class RabbitConsumer : IDataReader, IDisposable
    {
        private readonly string _queue;
        private readonly bool _autoAck;
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<Worker> _logger;

        public RabbitConsumer(ILogger<Worker> logger,Source _sourceConfig)
        {
            _logger = logger;

            _logger.LogInformation("[RabbitConsumer] Starting RabbitConsumer");
            _queue = _sourceConfig.SourceName;
            _autoAck = _sourceConfig.AutoCommit;

            ConnectionFactory factory = new ConnectionFactory()
            { Uri = new Uri($"amqp://{_sourceConfig.Username}:{_sourceConfig.Password}@{_sourceConfig.Cluster}") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.QueueDeclare(_queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();
            BasicGetResult consumeRes = null;

            while (consumeRes == null)
            {
                consumeRes = _channel.BasicGet(_queue, _autoAck);
            }

            res.SourceType = DataPlatformEnum.RabbitMQ;
            res.Data = Encoding.UTF8.GetString(consumeRes.Body.ToArray());

            return res;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
