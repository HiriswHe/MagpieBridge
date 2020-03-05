using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQFactory;
using RabbitMQHelper;
using RabbitMQRecieverInterface;
using RabbitMQUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace RabbitMQReciever
{
    public class ComsumeRabbitMQHostedService : BackgroundService 
    {
        static Timer timer = new Timer((e) => { Calling(e); }, null, 0, (ConstValues.ReloadSettingFileRefreshTime-500));
        private static IConnection _connection;
        private static IModel _channel;
        static readonly ushort prefetch = 30;
        static bool needReSet = false;
        //public IRabbitMQRecieverService RabbitMQRecieverServiceImpl { get; set; } //= new ProduceCheckLostDataConsumerImpllDao();//Update LostData
            //Update MissingData = new ProduceCheckConsumerImplDao();//Change Instance
        public ComsumeRabbitMQHostedService()
        {            
            InitRabbitMQ(adminMQ);
        }
        static Dictionary<string,List<RabbitMQInstance>> rabbitMQInstances = RabbitMQInstance.RabbitMQInstances;
        RabbitMQInstance adminMQ = RabbitMQInstance.AdminMQ;
        static ComsumeRabbitMQHostedService()
        {
            
        }

        static void Calling(object parameter)
        {
            if (DynamicLoader.IsLoading)
                needReSet = true;
            if (needReSet && RabbitMQInstance.IsFinished)
            {
                if (rabbitMQInstances != null)
                    foreach (var rabbitMQ in rabbitMQInstances)
                    {
                        BindConsumer(rabbitMQ.Value);
                    }
                needReSet = false;
                DynamicLoader.IsLoading = false;
                DynamicLoader.IsFinished = false;
                RabbitMQInstance.IsFinished = false;
            }
        }

        private static void InitRabbitMQ(RabbitMQInstance rabbitMQInstance)
        {
            if (rabbitMQInstance == null) throw new Exception("rabbitMQInstance Is Null");
            _connection = rabbitMQInstance.Factory.CreateConnection();
            _channel = _connection.CreateModel();
            var sExchange = rabbitMQInstance.RabbitMQModel?.Exchange;
            var sQueue = rabbitMQInstance.RabbitMQModel?.Queue;
            var sRoutingKey = rabbitMQInstance.RabbitMQModel?.RoutingKey;
            _channel.ExchangeDeclare(sExchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            _channel.QueueDeclare(sQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(sQueue, sExchange, sRoutingKey, null);
            _channel.BasicQos(0, prefetch, false);
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                if (rabbitMQInstances != null)
                    foreach (var rabbitMQ in rabbitMQInstances)
                    {
                        BindConsumer(rabbitMQ.Value);
                    }
                BindConsumer(new List<RabbitMQInstance> { adminMQ });
            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message);

            }
            await Task.CompletedTask;

        }

        private static void BindConsumer(List<RabbitMQInstance> rabbitMQInstances)
        {
            if (rabbitMQInstances == null|| rabbitMQInstances.Count==0) return;
            foreach(var rabbitMQInstance in rabbitMQInstances)
            {
                if (rabbitMQInstance == null) continue;
                var _channel = rabbitMQInstance.Connection.CreateModel();
                if (_channel == null || _channel.IsClosed)
                {
                    InitRabbitMQ(rabbitMQInstance);
                }
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {
                    var msg = System.Text.Encoding.UTF8.GetString(ea.Body);
                    if (msg == null) return;
                    if (string.IsNullOrEmpty(msg)) return;
                    string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    Logger.Info("HandleMessage:" + dateTime);
                    try
                    {
                        while (DynamicLoader.IsLoading)
                        {
                            Thread.Sleep(ConstValues.ReLoadSettingFileSleepTime);
                        }
                        IRabbitMQRecieverService rabbitMQRecieverServiceImpl = AssemblyLoader.AttachServices.FirstOrDefault(w => w.GetType().AssemblyQualifiedName == rabbitMQInstance.RabbitMQModel?.ConsumerType);
                        if (rabbitMQRecieverServiceImpl == null)
                        {
                            rabbitMQRecieverServiceImpl = Activator.CreateInstance(Type.GetType(rabbitMQInstance.RabbitMQModel?.ConsumerType)) as IRabbitMQRecieverService;
                        }
                        rabbitMQRecieverServiceImpl?.HandleMessage(ch, ea, msg);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                        Logger.Error("ContentErrorStart:" + dateTime + ":" + msg);
                        Logger.Error("ContentErrorFinished:" + dateTime);
                        RabbitMQSender.PublishToAdmin(rabbitMQInstance.RabbitMQModel?.PublishName, msg);
                    }
                    Logger.Info("Current Dealt:" + dateTime);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);//手动应答
            };
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerConsumerCancelled;
                _channel.BasicConsume(queue: rabbitMQInstance.RabbitMQModel?.Queue, autoAck: false, consumer: consumer); //手动应答
            }
        }
        
        private static void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private static void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private static void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private static void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private static void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
