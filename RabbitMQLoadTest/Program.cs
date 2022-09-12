using MonoStreamAgent;
using MonoStreamAgent.Common;
using MonoStreamAgent.Writers;

namespace RabbitMQLoadTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Destination destConf = new()
            {
                TypeName = DataPlatformEnum.RabbitMQ,
                Cluster = "100.27.24.141:5672",
                Exchange = "",
                Username = "admin",
                Password = "admin",
                SourceName = "first.queue",
                Vhost = "/"
            };
            RabbitProducer loadTest = new(destConf);

            MonoDTO message = new()
            {
                SourceType = DataPlatformEnum.RabbitMQ
            };

            int maxMessages = 100000;
            for (int i = 0; i < maxMessages; i++)
            {
                message.Data = $"Message number [{i}/{maxMessages}]";
                loadTest.Write(message);
            }

            Console.ReadLine();
        }
    }
}