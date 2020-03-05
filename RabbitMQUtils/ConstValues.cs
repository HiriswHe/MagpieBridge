using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace RabbitMQUtils
{
    public class ConstValues
    {
        public static string Admin { get; set; } = "Admin";
        public static string PublishNameSepareter { get; set; } = ">>>";
        public static string ReloadRabbitMQInstanceCommand { get; set; } = "@@@";
        private static string sReloadSettingFileRefreshTime { get; set; } = AppSetting.GetConfig("PublishSettings:ReloadSettingFileRefreshTime");
        private static string sReLoadSettingFileSleepTime { get; set; } = AppSetting.GetConfig("PublishSettings:ReLoadSettingFileSleepTime");
        public static int ReloadSettingFileRefreshTime { get; set; }
        public static int ReLoadSettingFileSleepTime { get; set; }
        public static string ReloadRabbitMQInstanceWholeCommand { get; set; } = Admin + PublishNameSepareter + ReloadRabbitMQInstanceCommand;

        static ConstValues()
        {
            int temp;
            int.TryParse(sReloadSettingFileRefreshTime, out temp);
            ReloadSettingFileRefreshTime = temp;
            int.TryParse(sReLoadSettingFileSleepTime, out temp);
            ReLoadSettingFileSleepTime = temp;
        }
    }
}
