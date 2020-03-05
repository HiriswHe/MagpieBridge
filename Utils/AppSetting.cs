using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSetting
    {
        private static readonly object objLock = new object();
        private static AppSetting instance = null;

        private IConfigurationRoot Config { get; }

        private AppSetting()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Config = builder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AppSetting GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new AppSetting();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConfig(string name)
        {
            return GetInstance().Config.GetSection(name).Value;
        }

        public static IEnumerable<IConfigurationSection> GetChildren(string name)
        {
            return GetInstance().Config.GetSection(name).GetChildren();
        }

        public static IConfigurationSection GetConfigurationSection(string name)
        {
            return GetInstance().Config.GetSection(name);
        }
    }
}
