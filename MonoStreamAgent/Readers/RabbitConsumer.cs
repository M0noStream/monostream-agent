using System;
using System.Text;
using MonoStreamAgent.Common;
using RabbitMQ.Client;

namespace MonoStreamAgent.Readers
{
    public class RabbitConsumer : IDataReader, IDisposable
    {
        private string _user = "guest";
        private string _password = "guest";
        private string _url = "localhost:5672";
        private string _queue = "my.cars";
        private IConnection _connection;
        private IModel _channel;

        public RabbitConsumer()
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

        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();
            BasicGetResult consumeRes = null;

            while (consumeRes == null)
            {
                consumeRes = _channel.BasicGet(_queue, true);
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
