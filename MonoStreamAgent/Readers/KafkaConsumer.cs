using System;
using System.Collections.Generic;
using Confluent.Kafka;
using MonoStreamAgent.Common;

namespace MonoStreamAgent.Readers
{
    public class KafkaConsumer : IDataReader, IDisposable
    {
        private IConsumer<string, string> _consumer;
        public KafkaConsumer()
        {
            ClientConfig kConfig = new ClientConfig(new Dictionary<string, string>
            {
                {"bootstrap.servers", "localhost:9092"}
            });

            ConsumerConfig consumerConfig = new ConsumerConfig(kConfig)
            {
                GroupId = "M0noStream-Kafka-Rabbit",
                EnableAutoCommit = false
            };

            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(consumerConfig);

            _consumer = builder.Build();
            _consumer.Subscribe("sample-messages");
        }

        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();
            ConsumeResult<string, string> consumeRes = null;
            
            while (consumeRes == null)
            {
                consumeRes = _consumer.Consume(10000);
            }

            res.SourceType = DataPlatformEnum.Kafka;
            res.Data = consumeRes.Message.Value;

            return res;
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}
