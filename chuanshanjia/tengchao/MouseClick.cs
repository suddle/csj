using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static tengchao.PublicDefine;

namespace tengchao
{
    class MouseClick
    {
        static systemsleep sl = new systemsleep();
        // 初始
        public const uint SWP_NOMOVE = 0x0002; //不调整窗体位置
        public const uint SWP_NOSIZE = 0x0001; //不调整窗体大小
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        ///////
        //////获取信息 
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);//设置此窗体为活动窗体

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);


        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        const int BM_CLICK = 0xF5;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        // 鼠标参数
        private static int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        private static int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        private static int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        private static int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标 
        //private static int MOUSEEVENTF_WHEEL = 0x800;//控制鼠标滚轮
        // 获取边框位置类
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public static Rectangle rects = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        public static int window_h = rects.Height;
        public static int window_w = rects.Width;

        // 以下是根据坐标获得的信息
        //获取窗口标题
        [DllImport("user32", SetLastError = true)]
        public static extern int GetWindowText(
            IntPtr hWnd,//窗口句柄
            StringBuilder lpString,//标题
            int nMaxCount //最大值
            );

        //获取类的名字
        [DllImport("user32.dll")]
        private static extern int GetClassName(
            IntPtr hWnd,//句柄
            StringBuilder lpString, //类名
            int nMaxCount //最大值
            );

        //根据坐标获取窗口句柄
        [DllImport("user32")]
        private static extern IntPtr WindowFromPoint(
        Point Point  //坐标
        );
        //单次点击事件
        /// <summary>
        /// 单次点击事件
        /// </summary>
        /// <param name="text">函数名</param>
        /// <param name="classname">类名</param>
        /// <param name="mainhwnd">句柄</param>
        /// <param name="topadd">距离高度</param>
        /// <param name="leftadd">距离左边</param>
        /// <param name="sleeptime">休眠时间</param>
        /// <returns></returns>
        public static bool AddYanZhengClickOne(string text, string classname, IntPtr mainhwnd, int topadd, int leftadd, int sleeptime)
        {
            bool mouse_call = true;
            if (!GlobalSystemStop)
            {
                string point_txt;
                string point_class;
                RECT rect = new RECT();
                GetWindowRect(mainhwnd, out rect);
                int Topclick = (rect.Top + topadd) * 65535 / window_h; // 131  711
                int Leftclick = (rect.Left + leftadd) * 65535 / window_w; // 481  1216
                int top = rect.Top + topadd;
                int left = rect.Left + leftadd;
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
                CommonFunc.CommonSleep("AddYanZhengClickOne", sl.Little);
                GetPointAttribute(out point_txt, out point_class);
                logg.Info(text+"##"+topadd+"##"+leftadd+"##"+classname);
                if (point_txt == text)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickOne", sleeptime);
                    return mouse_call;
                }
                else if (point_class == classname)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickOne", sleeptime);
                    return mouse_call;
                }

                else
                {
                    mouse_call = false;
                    return mouse_call;
                }
            }
            return mouse_call;
        }
        //双击事件
        public static bool AddYanZhengClickTwo(string text, string classname, IntPtr mainhwnd, int topadd, int leftadd, int sleeptime)
        {
            bool mouse_call = true;
            if (!GlobalSystemStop)
            {
                string point_txt;
                string point_class;
                RECT rect = new RECT();
                GetWindowRect(mainhwnd, out rect);
                int top = rect.Top + topadd;
                int left = rect.Left + leftadd;
                int Topclick = (rect.Top + topadd) * 65535 / window_h; // 131  711
                int Leftclick = (rect.Left + leftadd) * 65535 / window_w; // 481  1216
                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
                CommonFunc.CommonSleep("AddYanZhengClickTwo", sl.Little);
                GetPointAttribute(out point_txt, out point_class);
                if (point_txt == text)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickTwo", sl.Little);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickTwo", sleeptime);
                    return mouse_call;
                }
                else if (point_class == classname)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickTwo", sl.Little);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("AddYanZhengClickTwo", sleeptime);
                    return mouse_call;
                }
                else
                {
                    mouse_call = false;
                    return mouse_call;
                }
            }
            return mouse_call;
        }
        //
        private static void GetPointAttribute(out string point_txt, out string point_class)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            Point p = new Point(x, y);
            IntPtr formHandle = WindowFromPoint(p);//得到窗口句柄
            StringBuilder title = new StringBuilder(256);
            GetWindowText(formHandle, title, title.Capacity);//得到窗口的标题
            point_txt = title.ToString();
            StringBuilder className = new StringBuilder(256);
            GetClassName(formHandle, className, className.Capacity);//得到窗口的句柄
            point_class = className.ToString();
        }
        public static void JustClickOne()
        {
            int top = window_h / 2;
            int left = window_w / 2;
            int Topclick = (top) * 65535 / window_h; // 131  711
            int Leftclick = (left) * 65535 / window_w; // 481  1216
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
            CommonFunc.CommonSleep("JustClickOne", sl.Little);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
            CommonFunc.CommonSleep("JustClickOne", sl.Longe);
        }
    }
}
