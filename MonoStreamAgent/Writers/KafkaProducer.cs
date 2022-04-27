using Confluent.Kafka;
using MonoStreamAgent.Common;
using System;

namespace MonoStreamAgent.Writers
{
    public class KafkaProducer : IDataWriter, IDisposable
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer()
        {
            ProducerConfig producerConfig = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public void Write(MonoDTO data)
        {
            _producer.Produce("sample-messages", new Message<Null, string> { Value = data.Data });
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
