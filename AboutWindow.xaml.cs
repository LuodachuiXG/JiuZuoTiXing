using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JiuZuoTiXing.util;
using Window = HandyControl.Controls.Window;

namespace JiuZuoTiXing;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        Title = "关于";
    }


    /**
     * 点击 HandyControl 打开 Github 页面
     */
    private void OnHandyClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        Tool.OpenUrl("https://github.com/NaBian/HandyControl");
    }

    /**
     * 点击版权信息打开官网
     */
    private void OnCopyRightClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        Tool.OpenUrl("https://luodachui.cn");
    }
}