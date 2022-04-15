using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using JiuZuoTiXing.util;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = HandyControl.Controls.Window;

namespace JiuZuoTiXing;

public partial class SettingsWindow : Window
{
    private readonly IniUtil _ini = Common.GetIniUtil();

    private readonly SettingPropertyGridModel _settingModel = new ();
    public SettingsWindow()
    {
        InitializeComponent();
        
        // 读取配置文件中的数据
        var autoStart = _ini.ReadString(Common.NodeMain, Common.MainAutoStart, "N");
        var closeMode = _ini.ReadString(Common.NodeClose, Common.CloseMode, Common.CloseMinimize);
        _settingModel.开机自启 = autoStart.Equals("Y");
        _settingModel.窗口关闭方式 = closeMode.Equals(Common.CloseMinimize) ? CloseMode.最小化到托盘 : CloseMode.关闭程序;
        
        PropertyGrid.SelectedObject = _settingModel;
    }

    private void OnWindowClosing(object? sender, CancelEventArgs cancelEventArgs)
    {
        // 设置开机自启
        if (_settingModel.开机自启)
        {
            if (!Tool.AutoStart())
            {
                MessageBox.Error("开机自启设置失败！请尝试使用管理员权限运行。");
            }
            else
            {
                _ini.WriteString(Common.NodeMain, Common.MainAutoStart, "Y");
            }
        }
        else
        {
            if (!Tool.AutoStart(false))
            {
                MessageBox.Error("开机自启设置失败！请尝试使用管理员权限运行。");
            }
            else
            {
                _ini.WriteString(Common.NodeMain, Common.MainAutoStart, "N");
            }
        }

        // 设置窗口关闭方式
        var closeMode = _settingModel.窗口关闭方式 == CloseMode.最小化到托盘 ? Common.CloseMinimize : Common.CloseFinish;
        _ini.WriteString(Common.NodeClose, Common.CloseMode, closeMode);
    }
    
}

public class SettingPropertyGridModel
{
    [Category("主要设置")]
    public bool 开机自启 { get; set; }
    
    [Category("主要设置")]
    public CloseMode 窗口关闭方式 { get; set; }
}

public enum CloseMode
{
    最小化到托盘,
    关闭程序
}