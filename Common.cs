using System;
using JiuZuoTiXing.util;

namespace JiuZuoTiXing
{
    class Common
    {
        // 数据存放地址
        public static readonly string Path = $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\JiuZuoTiXing\\";
        
        // 配置文件节点
        public static readonly string NodeMain = "Main";
        public static readonly string NodeClose = "CloseSetting";
        public static readonly string NodeReminder = "ReminderSetting";

        // 主设置项
        public static readonly string MainReminderTime = "reminder_Time";
        public static readonly string MainStopReminderTime = "stop_Reminder_Time";
        public static readonly string MainReminderMode = "reminder_Mode";
        public static readonly string MainAutoStart = "auto_Starting";

        // 关闭设置项
        public static readonly string CloseMode = "mode";
        public static readonly string CloseMinimize = "minimize";
        public static readonly string CloseFinish = "finish";
        
        // 全局设置配置文件IniUtil类
        public static  IniUtil GetIniUtil()
        {
            return new IniUtil(Path, "config.ini");
        }

    }
}
