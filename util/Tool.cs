using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using Microsoft.Win32;

namespace JiuZuoTiXing.util
{
    class Tool
    {
        public static bool AutoStart(bool isAuto = true)
        {
            //获取程序执行路径..
            var starUpPath = AppDomain.CurrentDomain.BaseDirectory + Process.GetCurrentProcess().ProcessName + ".exe";
            var local = Registry.LocalMachine;
            var run = local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            try
            {
                if (isAuto)
                {
                    run.SetValue("久坐提醒", starUpPath);
                    local.Close();
                    run.Close();
                } 
                else
                {
                    run.DeleteValue("久坐提醒", false);
                    local.Close();
                    run.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 把秒转为类似于99小时59分钟59秒的文本
        /// </summary>
        /// <param name="second">秒</param>
        /// <param name="supplementZero">是否在时间上增补0</param>
        /// <returns></returns>
        public static string SecondToTime(int second, bool supplementZero = true)
        {
            var day = second / 86400;
            var hour = second / 3600 - day * 24;
            var minute = second / 60 - hour * 60 - day * 1440;
            var mSecond = second - minute * 60 - hour * 3600 - day * 86400;

            // 为了使小时超过 24 小时，需要加上上面减去的 天数 * 24 小时
            hour = hour + day * 24;

            string str;
            if (supplementZero)
                str = (hour.ToString().Length < 2 ? "0" + hour : hour.ToString()) + "时" +
                    (minute.ToString().Length < 2 ? "0" + minute : minute.ToString()) + "分" +
                    (mSecond.ToString().Length < 2 ? "0" + mSecond : mSecond.ToString()) + "秒";
            else
                str = day + "天" + hour + "时" + minute + "分" + mSecond + "秒";
            return str;
        }

        /**
         * 将秒按照时分秒格式化成 {2, 3, 5, 9, 5, 9} 数组格式，以供翻页时钟 FlipClock 使用
         */
        public static List<int> SecondToList(int second)
        {
            var day = second / 86400;
            var hour = second / 3600 - day * 24;
            var minute = second / 60 - hour * 60 - day * 1440;
            var mSecond = second - minute * 60 - hour * 3600 - day * 86400;
            
            // 为了使小时超过 24 小时，需要加上上面减去的 天数 * 24 小时
            hour = hour + day * 24;

            var list = new List<int>(){0, 0, 0, 0, 0, 0};
            if (hour.ToString().Length < 2)
            {
                list[0] = 0;
                list[1] = hour;
            }
            else
            {
                list[0] = int.Parse(hour.ToString().Substring(0, 1));
                list[1] = int.Parse(hour.ToString().Substring(1, 1));
            }
            
            if (minute.ToString().Length < 2)
            {
                list[2] = 0;
                list[3] = minute;
            }
            else
            {
                list[2] = int.Parse(minute.ToString().Substring(0, 1));
                list[3] = int.Parse(minute.ToString().Substring(1, 1));
            }
            
            if (mSecond.ToString().Length < 2)
            {
                list[4] = 0;
                list[5] = mSecond;
            }
            else
            {
                list[4] = int.Parse(mSecond.ToString().Substring(0, 1));
                list[5] = int.Parse(mSecond.ToString().Substring(1, 1));
            }
            return list;
        }
        
        /**
         * 用浏览器打开 Url
         */
        public static void OpenUrl(string url)
        {
            Process.Start("explorer.exe", url);
        }
    }
}
