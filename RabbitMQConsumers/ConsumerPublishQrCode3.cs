using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;

namespace RabbitMQConsumers
{
    public class ConsumerPublish3 : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerPublish3:" + msg);
        }
    }
}
