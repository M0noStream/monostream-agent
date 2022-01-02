using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MonoStreamAgent.Connectors
{
    public class KafkaConnector
    {
        public void Consume()
        {
            ClientConfig kConfig = new ClientConfig(new Dictionary<string, string>
            {
                {"bootstrap.servers", "localhost:29092"}
            });

            ConsumerConfig consumerConfig = new ConsumerConfig(kConfig)
            {
                GroupId = "mayan",
                //AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(consumerConfig);

            using IConsumer<string, string> consumer = builder.Build();
            consumer.Subscribe("my.cars");

            ConsumeResult<string, string> res = null;
            
            while (res == null)
            {
                res = consumer.Consume(1);
            }

            Console.WriteLine(res.Message.Value);
        }
    }
}
