using RabbitMQ.Client.Events;
using RabbitMQRecieverInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQConsumersAttach
{
    public class NewConsumer1 : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("NewConsumer1:" + msg);
        }
    }
}
