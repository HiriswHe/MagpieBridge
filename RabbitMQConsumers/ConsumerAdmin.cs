using RabbitMQ.Client.Events;
using RabbitMQHelper;
using RabbitMQRecieverInterface;
using RabbitMQUtils;
using System;

namespace RabbitMQConsumers
{
    public class ConsumerAdmin : IRabbitMQRecieverService
    {
        public void HandleMessage(object ch, BasicDeliverEventArgs ea, string msg)
        {
            Console.WriteLine("ConsumerAdmin:"+msg);
            if (msg == ConstValues.ReloadRabbitMQInstanceWholeCommand)
            {

            }
            else
            {
                int iNotation = msg.IndexOf(ConstValues.PublishNameSepareter);
                if (iNotation < 0) return;
                var publishName = msg.Substring(0, iNotation);
                var content = msg.Substring(iNotation+ConstValues.PublishNameSepareter.Length);
                RabbitMQSender.PublishToQueue(publishName, content);
            }
        }
    }
}
