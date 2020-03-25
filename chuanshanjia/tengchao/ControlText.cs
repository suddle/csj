using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static tengchao.CallWin32Api;
using static tengchao.PublicDefine;

namespace tengchao
{
    class ControlText
    {
        const int BM_CLICK = 0xF5;//点击
        const int WM_CLOSE = 0x10;//关闭
        const int WM_KEYDOWN = 0x0100;//普通按键按下
        const int WM_GETTEXT = 0x000D;//获取txt
        const int WM_GETTEXTLENGTH = 0x000E;//获取txt长度
        const int WM_SETTEXT = 0x000C;
        const int WM_COPY = 0x0301;
        const int WM_COPYDATA = 0x004A;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public delegate bool CallBack(IntPtr hwnd, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

        //获取类的名字 
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]

        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage1(IntPtr hwnd, uint wMsg, int wParam, string lParam);

        // combobox参数
        const int CB_SELECTSTRING = 0x014D;
        const int CB_SHOWDROPDOWN = 0x014F;
        const int VK_RETURN = 0x0D;
        const int CB_GETDROPPEDSTATE = 0x0157;
        /// <summary>
        /// 把数据写入到对应的控件，hwnd是该控件的句柄，txt是需要写入的数据
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <param name="txt">需要写入的数据，string类型</param>
        public static void XieRuMsgToHwnd(IntPtr hWnd, string txt)
        {
            logg.Info("发送内容"+txt+"到控件"+hWnd.ToString());
            try
            {
                SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, txt);
            }
            catch (Exception e)
            {
                logg.Info("XieRuMsgToHwnd:"+e.ToString());
                CommonFunc.SendBug("XieRuMsgToHwnd错误", "2", e.ToString(), "controltext", "XieRuMsgToHwnd");
            }
        }
        /// <summary>
        /// 选择combobox对应的文本，hwnd是控件对应的句柄，txt是我们需要传的值，但是此值必须是combobox列表中包含的某一个值
        /// </summary>
        /// <param name="hwnd">控件句柄</param>
        /// <param name="txt">需要写入到combobox的值，string类型</param>
        public static void ChooseComboBoxText(IntPtr hwnd, string txt)
        {
            if (txt == "京")
            {
                txt = "京".ToString();
            }
            CommonFunc.CommonSleep("ChooseComboBoxText", 200);
            SendMessage(hwnd, CB_SHOWDROPDOWN, 1, 0);//打开列表
            CommonFunc.CommonSleep("ChooseComboBoxText", 20);
            //combobox要选择的内容是txt
            SendMessage1(hwnd, CB_SELECTSTRING, 0, txt);//选择
            CommonFunc.CommonSleep("ChooseComboBoxText", 20);
            SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0);//响应回车键
            CommonFunc.CommonSleep("ChooseComboBoxText", 20);
            SendMessage(hwnd, CB_SHOWDROPDOWN, 0, 0); //关闭列表  
            CommonFunc.CommonSleep("ChooseComboBoxText", 20);
        }
        /// <summary>
        /// 获取坐标
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="Top">距离高</param>
        /// <param name="Left">距离左边</param>
        public static void GetexTopLeft(IntPtr hwnd, out int Top, out int Left)
        {
            RECT rect = new RECT();
            GetWindowRect(hwnd, out rect);
            Top = rect.Top;
            Left = rect.Left;
        }
        /// <summary>
        /// 根据坐标获取标题
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="top">距离高</param>
        /// <param name="left">距离左边</param>
        /// <returns></returns>
        public static string TopLeftGetText(IntPtr hwnd, int top, int left)
        {
            RECT rect = new RECT();
            GetWindowRect(hwnd, out rect);
            if (top == rect.Top && left == rect.Left)
            {
                const int buffer_size = 10240000;
                StringBuilder buffer = new StringBuilder(buffer_size);
                SendMessage(hwnd, WM_GETTEXT, buffer_size, buffer);
                return buffer.ToString();
            }
            else {
                return "";
            }
        }
        /// <summary>
        /// 匹配颜色
        /// </summary>
        /// <param name="color_string"></param>
        /// <param name="_color"></param>
        public static void FindColor(string color_string, out string _color)
        {
            //红橙黄绿青蓝紫灰粉黑白棕
            _color = "";
            string replace_jinshuqi_color_string = color_string.Replace("金属漆", "");
            for(int i = replace_jinshuqi_color_string.Length-1; i >= 0; i--)
            {
                Match match1 = Regex.Match(replace_jinshuqi_color_string[i].ToString(), @"[白灰黄粉红紫绿蓝棕黑银墨橄金米橙褐]{1}");
                if (match1.Success)
                {
                    _color = match1.Value;
                    if (_color.Equals("银"))
                    {
                        _color = "灰";
                    }
                    else if (_color.Equals("墨"))
                    {
                        _color = "黑";
                    }
                    else if(_color.Equals("金"))
                    {
                        _color = "黄";
                    }
                    else if(_color.Equals("米"))
                    {
                        _color = "灰";
                    }
                    else if(_color.Equals("橙"))
                    {
                        _color = "黄";
                    }
                    else if(_color.Equals("褐"))
                    {
                        _color = "棕";
                    }
                    else if(_color.Equals("橄"))
                    {
                        _color = "绿";
                    }
                    break;
                }
            }
        }
    }
}
