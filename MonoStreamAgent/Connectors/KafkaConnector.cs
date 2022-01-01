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
        Dictionary<string, string> consumerConfig = new Dictionary<string, string>
            {
                {"bootstrap.servers", "127.0.0.1:9093"},
                {"group.id", "mayan"}
            };
        
        public void Consume()
        {
            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(consumerConfig);

            IConsumer<string, string> consumer = builder.Build();

            consumer.Subscribe("test");

            ConsumeResult<string, string> res = consumer.Consume(7000);

            if (res != null)
            {
                Console.WriteLine(res);
            }
        }
    }
}
