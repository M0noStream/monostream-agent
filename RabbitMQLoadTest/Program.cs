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
                Cluster = "50.17.169.206:5672",
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
            Console.WriteLine("Starting write messages");
            int maxMessages = 100000;
            int i;
            for (i = 1; i <= maxMessages; i++)
            {
                message.Data = $"Message number [{i}/{maxMessages}]";
                loadTest.Write(message);
            }
            Console.WriteLine($"Finished writing messages. Last message id is {i}");
            Console.ReadLine();
        }
    }
}