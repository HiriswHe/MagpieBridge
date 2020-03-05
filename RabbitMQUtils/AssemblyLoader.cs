using RabbitMQRecieverInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RabbitMQUtils
{
    public class AssemblyLoader
    {
        static string Attach =Environment.CurrentDirectory + "\\Attach";
        public static List<IRabbitMQRecieverService> AttachServices = new List<IRabbitMQRecieverService>();
        static AssemblyLoader()
        {
            LoadDll();
        }
        public static List<IRabbitMQRecieverService> LoadDll()
        {
            AttachServices = new List<IRabbitMQRecieverService>();
            List<IRabbitMQRecieverService> result = new List<IRabbitMQRecieverService>();
            string[] files = Directory.GetFiles(Attach, "*.dll");
            foreach (var file in files)
            {
                Assembly assembly = Assembly.LoadFile(file);
                if (assembly == null) continue;
                var serviceTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IRabbitMQRecieverService)));
                if (serviceTypes != null && serviceTypes.Count() > 0)
                {
                    foreach (var type in serviceTypes)
                    {
                        IRabbitMQRecieverService rabbitMQRecieverService = Activator.CreateInstance(type) as IRabbitMQRecieverService;
                        if(rabbitMQRecieverService!=null&& !result.Exists(w=>w.GetType()==rabbitMQRecieverService.GetType()))
                            result.Add(rabbitMQRecieverService);
                    }
                }
            }
            AttachServices = result;
            return result;
        }
    }
}
