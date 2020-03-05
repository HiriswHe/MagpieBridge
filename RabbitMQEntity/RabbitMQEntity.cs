using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQEntity
{
    public class RabbitMQModel
    {
        public string PublishName { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public string ConsumerType { get; set; }
    }
}
