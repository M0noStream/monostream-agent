using System;
using System.Collections.Generic;
using Confluent.Kafka;
using MonoStreamAgent.Common;

namespace MonoStreamAgent.Readers
{
    public class KafkaConsumer : IDataReader, IDisposable
    {
        private IConsumer<string, string> _consumer;
        private readonly int _consumeTimeoutMS;
        public KafkaConsumer(Source _sourceConfig)
        {
            _consumeTimeoutMS = _sourceConfig.ConsumeTimeoutMS;

            ConsumerConfig consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _sourceConfig.Cluster,
                GroupId = _sourceConfig.ConsumerGroup,
                EnableAutoCommit = _sourceConfig.AutoCommit
            };

            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(consumerConfig);

            _consumer = builder.Build();
            _consumer.Subscribe(_sourceConfig.SourceName);
        }

        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();
            ConsumeResult<string, string> consumeRes = null;
            
            while (consumeRes == null)
            {
                consumeRes = _consumer.Consume(_consumeTimeoutMS);
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
