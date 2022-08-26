using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using HandyControl.Controls;
using JiuZuoTiXing.pages;
using JiuZuoTiXing.util;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = HandyControl.Controls.Window;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using System.Drawing;

namespace JiuZuoTiXing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly IniUtil Ini = Common.GetIniUtil();

        public readonly string[] ReminderTimeItems = { "30", "35", "40", "45", "50", "55", "60", "70", "80", "90", "100" };
        public readonly string[] StopReminderTimeItems = { "1", "2", "3", "5", "10", "15", "20", "30", "40", "50" };
        public readonly string[] ComboBoxItemsItems = { "软件通知（推荐）", "系统通知" };

        // 提醒时间和鼠标无操作重置时间的分钟和秒
        private int _reminderMinute = 0;
        private int _stopReminderMinute = 0;
        private int _reminderSecond = 0;
        private int _stopReminderSecond = 0;

        // 提醒时间 / 无操作时间 / 通知模式 的 ComboBox 选中 Index
        private int _reminderTimeSelectIndex = 6;
        private int _stopReminderTimeSelectIndex = 3;
        private int _reminderModeSelectIndex = 0;

        // MainPage 是否自动执行标识
        private bool _autoStart = false;


        // 通知方式 (0 == 软件通知，1 == 系统通知)
        private int _reminderMode = 0;


        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        /**
         * 初始化数据
         */
        private void Init()
        {
            // 判断配件文件夹是否存在
            if (!Directory.Exists(Common.Path))
            {
                Directory.CreateDirectory(Common.Path);
            }

            // 判断配置文件中的提醒时间等参数是否正确
            var reminderTime = Convert.ToInt32(Ini.ReadString(Common.NodeMain, Common.MainReminderTime, "6"));
            var stopReminderTime = Convert.ToInt32(Ini.ReadString(Common.NodeMain, Common.MainStopReminderTime, "3"));
            var reminderMode = Convert.ToInt32(Ini.ReadString(Common.NodeMain, Common.MainReminderMode, "0"));
            if ((reminderTime is < 0 or > 10) || (stopReminderTime is < 0 or > 9) || (reminderMode is < 0 or > 1))
            {
                // 配置文件被篡改，保持 ComboBox 默认数据，并修改配置文件错误数据
                MessageBox.Fatal("请不要修改配置文件，否则可能导致软件出现未知错误", "错误");
                Ini.WriteString(Common.NodeMain, Common.MainReminderTime, "6");
                Ini.WriteString(Common.NodeMain, Common.MainStopReminderTime, "3");
                Ini.WriteString(Common.NodeMain, Common.MainReminderMode, "0");
            }
            else
            {
                // 数据正确，存入变量
                _reminderTimeSelectIndex = reminderTime;
                _stopReminderTimeSelectIndex = stopReminderTime;
                _reminderModeSelectIndex = reminderMode;
            }

            // 判断是否启用了开机自启
            var autoStart = Ini.ReadString(Common.NodeMain, Common.MainAutoStart, "N");
            if (autoStart.Equals("Y"))
            {
                // 启用了开机自启，启动程序后自动执行并最小化到托盘
                _reminderMinute = Convert.ToInt32(ReminderTimeItems[_reminderTimeSelectIndex]);
                _reminderSecond = _reminderMinute * 60;
                _stopReminderMinute = Convert.ToInt32(StopReminderTimeItems[_stopReminderTimeSelectIndex]);
                _stopReminderSecond = _stopReminderMinute * 60;
                _reminderMode = _reminderModeSelectIndex;
                NavigateToTimingPage();

                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
                //this.Hide();
                ShowInTaskbar = false;

                Growl.SuccessGlobal("久坐提醒已自动最小化运行");
            }
            else
            {
                // 未启用开机自启，默认跳转 MainPage
                NavigateToMainPage();
            }
        }


        /**
         * 设置提醒时间和无操作重置时间变量
         */
        public void SetReminderTime(int reminderMinute, int stopReminderMinute)
        {
            _reminderMinute = reminderMinute;
            _stopReminderMinute = stopReminderMinute;
            _reminderSecond = _reminderMinute * 60;
            _stopReminderSecond = _stopReminderMinute * 60;
        }

        /**
         * 获取提醒时间（秒）
         */
        public int GetReminderSecond()
        {
            return _reminderSecond;
        }

        /**
         * 获取提醒时间的 ComboBox 选项 Index
         */
        public int GetReminderTimeSelectIndex()
        {
            return _reminderTimeSelectIndex;
        }

        /**
         * 设置提醒时间的 ComboBox 选项 Index
         */
        public void SetReminderTimeSelectIndex(int index)
        {
            _reminderTimeSelectIndex = index;
        }

        /**
         * 获取无操作时间（秒）
         */
        public int GetStopReminderSecond()
        {
            return _stopReminderSecond;
        }

        /**
         * 获取无操作时间的 ComboBox 选项 Index
         */
        public int GetStopReminderTimeSelectIndex()
        {
            return _stopReminderTimeSelectIndex;
        }

        /**
         * 设置提醒时间的 ComboBox 选项 Index
         */
        public void SetStopReminderTimeSelectIndex(int index)
        {
            _stopReminderTimeSelectIndex = index;
        }

        /**
         * 设置通知方式
         */
        public void SetReminderMode(int reminderMode)
        {
            _reminderMode = reminderMode;
        }

        /**
         * 获取通知方式
         */
        public int GetReminderMode()
        {
            return _reminderMode;
        }

        /**
         * 获取通知方式的 ComboBox 选项 Index
         */
        public int GetReminderModeSelectIndex()
        {
            return _reminderModeSelectIndex;
        }

        /**
         * 设置通知方式的 ComboBox 选项 Index
         */
        public void SetReminderModeSelectIndex(int index)
        {
            _reminderModeSelectIndex = index;
        }

        /**
         * 告知 MainPage 页面是否自动执行，并且只有第一次验证才为 true
         */
        public bool IsAutoStart()
        {
            if (_autoStart)
            {
                _autoStart = false;
                return true;
            }
            return false;
        }

        /**
         * Frame 跳转到 MainPage 页面
         */
        public void NavigateToMainPage()
        {
            // 因为 TimingPage 可能会修改标题，所以这里默认重置一下
            Title = "久坐提醒";
            MainWindowFrame.Navigate(new MainPage(this));
        }

        /**
         * Frame 跳转到 TimingPage 页面
         */
        public void NavigateToTimingPage()
        {
            MainWindowFrame.Navigate(new TimingPage(this));
        }


        /**
         * MainWindows 窗口关闭事件
         */
        private void OnWindowClosing(object? sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = true;
            var closeMode = Ini.ReadString(Common.NodeClose, Common.CloseMode, "");

            // 判断配置文件中是否有关闭窗口的配置文件，或配置参数是否错误。此判断一般在第一次使用软件时才会触发。
            if (closeMode.Length == 0 ||
                (!closeMode.Equals(Common.CloseFinish) && !closeMode.Equals(Common.CloseMinimize)))
            {
                // 告知用户关闭窗口默认是最小化，可以在设置里修改
                Growl.Ask("默认关闭窗口是最小化到托盘，可在设置修改。\n\n取消 = 改为直接关闭程序\n确定 = 接受最小化到托盘", isConfirmed =>
                {
                    if (isConfirmed)
                    {
                        // 将关闭窗口的模式改为最小化
                        Ini.WriteString(Common.NodeClose, Common.CloseMode, Common.CloseMinimize);
                        //WindowState = WindowState.Minimized;
                        this.Hide();
                        ShowInTaskbar = false;
                    }
                    else
                    {
                        // 将关闭窗口的模式改为直接关闭程序，并关闭程序
                        Ini.WriteString(Common.NodeClose, Common.CloseMode, Common.CloseFinish);
                        Application.Current.Shutdown();
                    }
                    return true;
                });
                return;
            }

            // 判断当前的关闭模式是最小化到托盘还是关闭程序
            if (closeMode.Equals(Common.CloseMinimize))
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /**
         * MenuItem “设置” 选项点击事件
         */
        private void OnSettingItemClick(object sender, RoutedEventArgs routedEventArgs)
        {
            new SettingsWindow().Show();
        }

        /**
         * MenuItem “关于” 选项点击事件
         */
        private void OnAboutItemClick(object sender, RoutedEventArgs routedEventArgs)
        {
            new AboutWindow().Show();
        }

        /**
         * MenuItem “检查更新” 选项点击事件
         */
        private void OnCheckUpdateItemClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Tool.OpenUrl("https://app.luodachui.cn/#jiuzuotixing");
        }

        /**
         * 托盘图标单击/托盘菜单显示程序单击 事件
         */
        private void OnNotifyIconClick(object sender, RoutedEventArgs routedEventArgs)
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }


    }
}