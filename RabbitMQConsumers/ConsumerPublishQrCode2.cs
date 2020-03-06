using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;

namespace RabbitMQConsumers
{
    public class ConsumerPublishQrCode2 : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerPublishQrCode2:" + msg);
        }
    }
}
