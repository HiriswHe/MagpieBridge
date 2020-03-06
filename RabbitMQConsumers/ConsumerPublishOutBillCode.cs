using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;

namespace RabbitMQConsumers
{
    public class ConsumerPublishOutBillCode : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerPublishOutBillCode:" + msg);
        }
    }
}
