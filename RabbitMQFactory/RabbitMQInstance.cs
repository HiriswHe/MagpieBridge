using RabbitMQ.Client;
using RabbitMQEntity;
using RabbitMQUtils;
using System.Collections.Generic;
using System.Threading;

namespace RabbitMQFactory
{
    /// <summary>
    /// 
    /// </summary>

    public sealed class RabbitMQInstance
    {        
        public IConnection Connection { get; set; }
        public ConnectionFactory Factory { get; set; }
        public IModel Channel { get; set; } 
        public RabbitMQModel RabbitMQModel { get; set; }        
        private readonly static object _synObj=new object();
        public static Dictionary<string,List<RabbitMQInstance>> RabbitMQInstances { get; set; } = new Dictionary<string,List<RabbitMQInstance>>();
        public static RabbitMQInstance AdminMQ { get; set; } = GetRabbitMQInstance(PublishesUtil.AdminMQModel);
        static Timer timer = new Timer((e) => { Calling(e); }, null, 0, (ConstValues.ReloadSettingFileRefreshTime - 500)); 
        static bool needReSet = false;
        public static bool IsFinished = false;
        static void Calling(object parameter)
        {
            if (DynamicLoader.IsLoading&&!IsFinished)
                needReSet = true;
            if (needReSet && DynamicLoader.IsFinished)
            {
                var models = PublishesUtil.RabbitMQModels;
                GenerateInstances(models);
                needReSet = false;
                IsFinished = true;
            }
        }

        private RabbitMQInstance(ConnectionFactory _factory, IConnection _connection,  RabbitMQModel _rabbitMQModel)
        {
            this.Connection = _connection;
            this.Factory = _factory;
            this.Channel = Connection.CreateModel();
            this.RabbitMQModel = _rabbitMQModel;
        }

        static RabbitMQInstance()
        {
            var models = PublishesUtil.RabbitMQModels;
            GenerateInstances(models);
        }

        public static void GenerateInstances(List<RabbitMQModel> rabbitMQModels)
        {
            RabbitMQInstances = new Dictionary<string, List<RabbitMQInstance>>();
            //if (RabbitMQInstances == null || RabbitMQInstances.Count == 0)
            {
                if (rabbitMQModels == null || rabbitMQModels.Count == 0) return;
                foreach (var rabbitMQModel in rabbitMQModels)
                {
                    var instance = GetRabbitMQInstance(rabbitMQModel);
                    if (!RabbitMQInstances.ContainsKey(rabbitMQModel.PublishName))
                    {
                        RabbitMQInstances.Add(rabbitMQModel.PublishName, new List<RabbitMQInstance> { instance });
                    }
                    else
                        RabbitMQInstances[rabbitMQModel.PublishName].Add(instance);
                }
            }
        }

        public static RabbitMQInstance GetRabbitMQInstance(RabbitMQModel rabbitMQModel)
        {
            if (rabbitMQModel == null) return null;
            var _factory = new ConnectionFactory
            {
                HostName = rabbitMQModel.HostName,
                Port = rabbitMQModel.Port,
                VirtualHost = rabbitMQModel.VirtualHost,
                UserName = rabbitMQModel.UserName,
                Password = rabbitMQModel.Password
            };
            var _connection = _factory.CreateConnection();
            var _rabbitMQModel = rabbitMQModel;
            var _channel = _connection.CreateModel();
            var instance = new RabbitMQInstance(_factory, _connection, _rabbitMQModel);
            _channel.ExchangeDeclare(rabbitMQModel.Exchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            _channel.QueueDeclare(rabbitMQModel.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(rabbitMQModel.Queue, rabbitMQModel.Exchange, rabbitMQModel.RoutingKey, null);
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>        
        public static List<RabbitMQInstance> Instance(string publishName)
        {
            if (string.IsNullOrEmpty(publishName)) return null;
            List<RabbitMQInstance> _rabbitMQInstances = null;
            if (!RabbitMQInstances.ContainsKey(publishName)) return new List<RabbitMQInstance>(0);
            if(RabbitMQInstances.ContainsKey(publishName))
                _rabbitMQInstances = RabbitMQInstances[publishName];
            return _rabbitMQInstances;
        }         
        
    }
}
