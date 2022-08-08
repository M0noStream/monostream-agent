using Confluent.Kafka;
using MonoStreamAgent.Common;
using System;

namespace MonoStreamAgent.Writers
{
    public class KafkaProducer : IDataWriter, IDisposable
    {
        private readonly string _topic;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(Destination _destinationConfig)
        {
            _topic = _destinationConfig.SourceName;
            ProducerConfig producerConfig = new ProducerConfig
            {
                BootstrapServers = _destinationConfig.Cluster
            };

            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public void Write(MonoDTO data)
        {
            _producer.Produce(_topic, new Message<Null, string> { Value = data.Data });
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
