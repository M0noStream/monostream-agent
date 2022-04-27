using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using MonoStreamAgent.Common;

namespace MonoStreamAgent.Readers
{
    public class RabbitConsumer : IDataReader, IDisposable
    {
        private IConsumer<string, string> _consumer;
        public RabbitConsumer()
        {
            
        }

        public MonoDTO Read()
        {
            MonoDTO res = new MonoDTO();
            

            return res;
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}
