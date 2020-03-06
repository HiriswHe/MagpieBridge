using RabbitMQ.Client;
using RabbitMQFactory;
using RabbitMQUtils;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQHelper
{
    public class RabbitMQSender
    {
        static RabbitMQInstance adminMQ = RabbitMQInstance.AdminMQ;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        public static void PublishToQueue(string publishName, string message, string queue = "", string exchange = "", string routingKey = "")
        {
            if (string.IsNullOrEmpty(publishName)) return;
            try
            {
                RabbitMQInstance instance = null;
                if (publishName == ConstValues.Admin)
                    instance = adminMQ;
                else
                    instance = RabbitMQInstance.Instance(publishName).FirstOrDefault();
                if (instance == null) return;
                if (string.IsNullOrEmpty(exchange)) exchange = instance.RabbitMQModel.Exchange;
                if (string.IsNullOrEmpty(queue)) queue = instance.RabbitMQModel.Queue;
                if (string.IsNullOrEmpty(routingKey)) routingKey = instance.RabbitMQModel.RoutingKey;
                var channel = instance.Channel;
                channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue, exchange, routingKey, null);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;//持久化
                channel.BasicPublish(exchange: exchange,routingKey: routingKey,basicProperties: properties,body: body);
                Console.WriteLine("Send Message:"+message);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static void PublishToAdmin(string publishName, string message, string exchange = "", string queue = "", string routingKey = "")
        {
            if (string.IsNullOrEmpty(publishName) || string.IsNullOrEmpty(message)) return;
            RabbitMQInstance instance = null;
            if (publishName == ConstValues.Admin)
                instance = adminMQ;
            else
                instance = RabbitMQInstance.Instance(publishName).FirstOrDefault();
            if (instance == null) return;            
            PublishToQueue(ConstValues.Admin, publishName + ConstValues.PublishNameSepareter + message);
        }
    }
}
