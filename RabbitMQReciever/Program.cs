using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Utils;

namespace RabbitMQReciever
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Logger.Info("消息队列消费程序启动...");
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ComsumeRabbitMQHostedService>();
                });
            await builder.RunConsoleAsync();

        }
    }
}
