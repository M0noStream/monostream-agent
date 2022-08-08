using MonoStreamAgent.Common;
using RabbitMQ.Client;
using System;
using System.Text;

namespace MonoStreamAgent.Writers
{
    public class RabbitProducer : IDataWriter, IDisposable
    {
        private readonly string _queue;
        private readonly string _exchange;

        private IConnection _connection;
        private IModel _channel;

        public RabbitProducer(Destination _destinationConfig)
        {
            _queue = _destinationConfig.SourceName;
            _exchange = _destinationConfig.Exchange;

            ConnectionFactory factory = new ConnectionFactory()
                        { Uri = new Uri($"amqp://{_destinationConfig.Username}:{_destinationConfig.Password}@{_destinationConfig.Cluster}") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void Write(MonoDTO data)
        {
            // RabbitMQ can't publish string as a message
            // Parsing to byte[]
            byte[] body = Encoding.UTF8.GetBytes(data.Data);

            _channel.BasicPublish(_exchange, _queue, null, body);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
