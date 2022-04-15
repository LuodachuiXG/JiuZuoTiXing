using System;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using JiuZuoTiXing.util;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace JiuZuoTiXing.pages;

public partial class MainPage : Page
{
    private readonly IniUtil Ini;
    
    

    // 主页面 MainWindow 实例
    private readonly MainWindow _mainWindow;

    public MainPage(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        Ini = mainWindow.Ini;
        
        InitializeComponent();
        
        // 设置 ComboBox 数据
        CbReminderTime.ItemsSource = _mainWindow.ReminderTimeItems;
        CbStopReminderTime.ItemsSource = _mainWindow.StopReminderTimeItems;
        CbReminderMode.ItemsSource = _mainWindow.ComboBoxItemsItems;

        Init();
    }

    /**
     * 初始化数据
     */
    private void Init()
    {
        CbReminderTime.SelectedIndex = _mainWindow.GetReminderTimeSelectIndex();
        CbStopReminderTime.SelectedIndex = _mainWindow.GetStopReminderTimeSelectIndex();
        CbReminderMode.SelectedIndex = _mainWindow.GetReminderModeSelectIndex();
    }
    
    /**
     * 执行操作按钮单击事件
     */
    private void BtnStart_OnClick(object sender, RoutedEventArgs e)
    {
        var stopReminderTime = int.Parse(CbStopReminderTime.SelectedValue.ToString()!);
        var reminderTime = int.Parse(CbReminderTime.SelectedValue.ToString()!);
        
        // 鼠标无操作重置时间不能大于等于提醒时间
        if (stopReminderTime >= reminderTime)
        {
            Growl.Error("鼠标无操作重置时间不能超过提醒时间");
            return;
        }
        
        // 将提醒时间和无操作重置的时间以及通知方式存储到 MainWindow 变量中
        _mainWindow.SetReminderTime(reminderTime, stopReminderTime);
        _mainWindow.SetReminderMode(CbReminderMode.SelectedIndex);
        
        _mainWindow.SetReminderTimeSelectIndex(CbReminderTime.SelectedIndex);
        _mainWindow.SetStopReminderTimeSelectIndex(CbStopReminderTime.SelectedIndex);
        _mainWindow.SetReminderModeSelectIndex(CbReminderMode.SelectedIndex);
        
        
        // 存储到配置文件中
        Ini.WriteInt(Common.NodeMain, Common.MainReminderTime, CbReminderTime.SelectedIndex);
        Ini.WriteInt(Common.NodeMain, Common.MainStopReminderTime, CbStopReminderTime.SelectedIndex);
        Ini.WriteInt(Common.NodeMain, Common.MainReminderMode, CbReminderMode.SelectedIndex);
        
        // 跳转计时页面
        _mainWindow.NavigateToTimingPage();
    }
    
}