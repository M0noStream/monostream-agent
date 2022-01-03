using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using MonoStreamAgent.Common;

namespace MonoStreamAgent.Readers
{
    public class KafkaConsumer : IDataReader
    {
        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();

            ClientConfig kConfig = new ClientConfig(new Dictionary<string, string>
            {
                {"bootstrap.servers", "localhost:29092"}
            });

            ConsumerConfig consumerConfig = new ConsumerConfig(kConfig)
            {
                GroupId = "mayan",
                EnableAutoCommit = false
            };

            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(consumerConfig);

            using IConsumer<string, string> consumer = builder.Build();
            consumer.Subscribe("my.cars");

            ConsumeResult<string, string> consumeRes = null;
            
            while (consumeRes == null)
            {
                consumeRes = consumer.Consume(1);
            }

            res.SourceType = DataPlatformEnum.Kafka;
            res.data = consumeRes.Message.Value;

            return res;
        }
    }
}
