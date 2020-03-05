using RabbitMQHelper;

namespace MagpieBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            string memory1 = "JoerYang2018";
            string memory2 = "JoerYang2019";
            string missing = "JoerYang2020";
            RabbitMQSender.PublishToAdmin("Publish1", memory1);
            RabbitMQSender.PublishToAdmin("Publish1", memory1);
            RabbitMQSender.PublishToAdmin("Publish2", memory2);
            RabbitMQSender.PublishToAdmin("PublishNew1", missing);
        }
    }
}
