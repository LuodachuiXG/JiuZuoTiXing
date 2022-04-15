using System.Runtime.InteropServices;

namespace JiuZuoTiXing.util;

public class Mouse
{
    /// <summary>   
    /// 获取鼠标的坐标   
    /// </summary>   
    /// <param name="lpPoint">传址参数，坐标point类型</param>
    /// <param name="pt"></param>
    /// <returns>获取成功返回真</returns>   
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetCursorPos(out Point pt);

    ///<summary>   
    /// 设置鼠标的坐标   
    /// </summary>   
    /// <param name="x">横坐标</param>   
    /// <param name="y">纵坐标</param>   
    [DllImport("User32")]
    public static extern void SetCursorPos(int x, int y);
    public struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}