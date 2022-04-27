using MonoStreamAgent.Common;
using RabbitMQ.Client;
using System;
using System.Text;

namespace MonoStreamAgent.Writers
{
    public class RabbitProducer : IDataWriter, IDisposable
    {
        private string _user = "guest";
        private string _password = "guest";
        private string _url = "localhost:5672";
        private string _queue = "my.cars";
        private IConnection _connection;
        private IModel _channel;

        public RabbitProducer()
        {
            ConnectionFactory factory = new ConnectionFactory()
                        { Uri = new Uri($"amqp://{_user}:{_password}@{_url}") };
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

            _channel.BasicPublish("", _queue, null, body);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
