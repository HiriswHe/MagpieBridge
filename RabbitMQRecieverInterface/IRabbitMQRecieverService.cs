using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQRecieverInterface
{
    public interface IRabbitMQRecieverService
    {
        void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg);
    }
}
