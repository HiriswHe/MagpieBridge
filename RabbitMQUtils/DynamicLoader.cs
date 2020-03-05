using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace RabbitMQUtils
{
    public class DynamicLoader
    {
        static Timer timer = new Timer(new TimerCallback(Runing),null,0,ConstValues.ReloadSettingFileRefreshTime);
        static string settingFilePath = Environment.CurrentDirectory + "\\appsettings.json";
        static DateTime lastModifyTime = DateTime.Now;
        public static bool IsLoading = false;
        public static bool IsFinished = false;
        static DynamicLoader()
        {
            
        }

        static void Runing(object parameter)
        {
            if(!IsLoading)
                IsLoading = IsSettingFileModified();
            if (IsLoading&&!IsFinished)
            {
                PublishesUtil.LoadRabbitMQInstances();
                AssemblyLoader.LoadDll();
                IsFinished = true;
            }
        }

        static bool IsSettingFileModified()
        {
            bool result = false;

            FileInfo fileInfo = new FileInfo(settingFilePath);
            DateTime currentModifedTime = fileInfo.LastWriteTime;
            result = currentModifedTime > lastModifyTime;
            if(result)
                lastModifyTime = currentModifedTime;
            return result;
        }
    }
}
