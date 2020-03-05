using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;

namespace RabbitMQConsumers
{
    public class ConsumerPublish1 : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerPublish1:"+msg);
        }
    }
}
