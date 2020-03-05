
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQEntity;
using RabbitMQUtils;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace RabbitMQUtils
{
    public class PublishesUtil
    {
        public static IEnumerable<IConfigurationSection> Publishes { get; set; }
        public static List<RabbitMQModel> RabbitMQModels { get; set; } = new List<RabbitMQModel>();
        public static RabbitMQModel AdminMQModel { get; set; } = GetRabbitMQModelByPath("PublishSettings:" + ConstValues.Admin);
        static PublishesUtil()
        {
            LoadRabbitMQInstances();
        }
        public static void LoadRabbitMQInstances()
        {
            Publishes = AppSetting.GetChildren("PublishSettings:Publishes");
            RabbitMQModels = new List<RabbitMQModel>();
            if (Publishes != null)
            {
                foreach (var publish in Publishes)
                {
                    if (publish == null) continue;
                    var rabbitMQModel = GetRabbitMQModel(publish);
                    RabbitMQModels.Add(rabbitMQModel);
                }
            }
        }
        public static RabbitMQModel GetRabbitMQModelByPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var configurationSection = AppSetting.GetConfigurationSection(path);
            return GetRabbitMQModel(configurationSection);
        }

        public static RabbitMQModel GetRabbitMQModel(IConfigurationSection publish)
        {
            if (publish == null) return null;
            RabbitMQModel rabbitMQModel = new RabbitMQModel();
            rabbitMQModel.PublishName = publish.GetSection("PublishName").Value;
            rabbitMQModel.ConsumerType = publish.GetSection("ConsumerType").Value;
            rabbitMQModel.Exchange = publish.GetSection("Exchange").Value;
            rabbitMQModel.HostName = publish.GetSection("HostName").Value;
            rabbitMQModel.Password = publish.GetSection("Password").Value;
            string sPort = publish.GetSection("Port").Value;
            int port = 0;
            int.TryParse(sPort, out port);
            rabbitMQModel.Port = port;
            rabbitMQModel.Queue = publish.GetSection("Queue").Value;
            rabbitMQModel.RoutingKey = publish.GetSection("RoutingKey").Value;
            rabbitMQModel.UserName = publish.GetSection("UserName").Value;
            rabbitMQModel.VirtualHost = publish.GetSection("VirtualHost").Value;
            return rabbitMQModel;
        }


    }
}
