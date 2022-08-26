using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HandyControl.Controls;
using HandyControl.Data;
using JiuZuoTiXing.util;
using Mouse = JiuZuoTiXing.util.Mouse;
namespace JiuZuoTiXing.pages;

public partial class TimingPage : Page
{
    private readonly MainWindow _mainWindow;

    // 设置的提醒时间和鼠标无操作时间
    private int _reminderSecond = 0;
    private readonly int _stopReminderSecond = 0;

    // 定时器
    private readonly DispatcherTimer _timer;

    // 坐姿时间记录（秒）
    private int _timeRecord = 0;
    // 鼠标停止移动时间记录（秒）
    private int _mouseStopMoveTimeRecord = 0;
    // 停止记录坐姿时间标识
    private bool _stopReminderFlag = false;

    // 鼠标最后 X Y 位置
    private int _mouseLastX = 0;
    private int _mouseLastY = 0;


    public TimingPage(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _reminderSecond = _mainWindow.GetReminderSecond();
        _stopReminderSecond = _mainWindow.GetStopReminderSecond();

        _timer = new DispatcherTimer();
        // 时间间隔 1 秒
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_OnTick;
        _timer.Start();

        // 停止翻页时钟根据系统时间自动响应
        FlipClock.Dispose();

        // 先手动触发一下 Timer 的事件，防止进入此页面时组件有一秒的顿
        Timer_OnTick(null, null!);
    }


    private void Timer_OnTick(object? sender, EventArgs e)
    {
        /* 检测鼠标是否未移动并记录鼠标无操作时间 */
        var p = new Mouse.Point();
        Mouse.GetCursorPos(out p);
        var x = p.X;
        var y = p.Y;
        if (x == _mouseLastX && y == _mouseLastY)
        {
            // 鼠标现在相比上一秒位置没有变化，无操作时间 +1
            _mouseStopMoveTimeRecord++;

            // 判断鼠标无操作时间是否达到了重置计时的时间
            if (_mouseStopMoveTimeRecord >= _stopReminderSecond)
            {
                // 达到重置计时时间，重置计时器
                _timeRecord = 0;
                // 设置标识，告知下面代码停止记录坐姿时间
                _stopReminderFlag = true;
            }
        }
        else
        {
            // 鼠标现在相比上一秒位置已经变化，鼠标无操作时间归零
            _mouseStopMoveTimeRecord = 0;
            _stopReminderFlag = false;
        }

        // 将当前鼠标 X Y 位置存到变量
        _mouseLastX = x;
        _mouseLastY = y;

        /* 记录坐姿时间 */
        // 通过上面的标识来判断当前是否进行坐姿时间记录
        if (!_stopReminderFlag)
        {
            _timeRecord++;
        }


        /* 设置界面上一些数据展示 */
        // 根据鼠标无操作时间是否大于 0 来修改一些标签的文字提示
        if (_mouseStopMoveTimeRecord > 0)
        {
            LbTips.Content = "鼠标无操作 (" + Tool.SecondToTime(_mouseStopMoveTimeRecord) + ")";
            var style = (Style)this.FindResource("LabelPrimary");
            LbTips.Style = style;
        }
        else
        {
            LbTips.Content = "鼠标正在移动";
            var style = (Style)this.FindResource("LabelWarning");
            LbTips.Style = style;
        }

        // 设置波浪进度条进度
        Wpb.Value = Convert.ToDouble(_timeRecord) / Convert.ToDouble(_reminderSecond) * 100;

        // 设置翻页时钟
        FlipClock.NumberList = Tool.SecondToList(_timeRecord);


        /* 判断坐姿时间是否已经达到提醒时间 */
        if (_timeRecord >= _reminderSecond)
        {
            // 先暂停计时器
            _timer.Stop();
            // 判断是 软件通知 还是 系统通知
            switch (_mainWindow.GetReminderMode())
            {
                // 软件通知
                case 0:
                    LbTips.Content = "你已经坐了 " + _timeRecord / 60 + " 分钟啦";
                    var style = (Style)this.FindResource("LabelDanger");
                    LbTips.Style = style;
                    Growl.AskGlobal("已经坐了 " +
                              _timeRecord / 60 + " 分钟了，起来活动一下吧。\n\n确定 = 重新计时\n取消 = 再延迟 "
                              + _reminderSecond / 60 + " 分钟后提醒", isConfirmed =>
                    {
                        if (isConfirmed)
                        {
                            // 重新计时
                            _timeRecord = 0;
                            _mouseStopMoveTimeRecord = 0;
                        }
                        else
                        {
                            // 将提醒时间 + 一倍，已经计录时间不变
                            _reminderSecond += _reminderSecond;
                            _mainWindow.Title = "久坐提醒" + "（久坐太长时间对身体不好哦）";
                        }
                        _timer.Start();
                        return true;
                    });
                    break;
                // 系统通知
                case 1:
                    //_mainWindow.NotifyIcon.ShowBalloonTip("该起来活动一下啦",
                    //    "你已经久坐 " + _timeRecord / 60 + " 分钟了，起来活动一下吧。", NotifyIconInfoType.Info);
                    _timeRecord = 0;
                    _mouseStopMoveTimeRecord = 0;
                    _timer.Start();
                    break;
            }
        }

    }

    private void OnStopReminderBtnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        _timeRecord = 0;
        _mouseStopMoveTimeRecord = 0;
        FlipClock.Dispose();
        _mainWindow.NavigateToMainPage();
    }
}