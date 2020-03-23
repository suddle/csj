using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.PublicDefine;
using System.Collections.Specialized;
namespace tengchao
{
    class HistoryWip
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public static IDataObject iData = Clipboard.GetDataObject();
        protected static IntPtr BaoBiaoIntper = IntPtr.Zero;
        /// <summary>
        /// 获取历史表总入口函数
        /// </summary>
        public static void HistoryList()
        {
            IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);//找到大窗体句柄
            SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_BigHwnd);//置顶
            ClickNew(_BigHwnd);//点击新建
            ClickLook(_BigHwnd);//点击查看
            ClickBaobiao(_BigHwnd);//点击报表
            CommonFunc.CommonSleep("HistoryList", 1500);
            ClickBaobiaoXuanze();//点击报表选择
            FindWeixiu();//查找维修窗体
            CommonFunc.CommonSleep("HistoryList", 500);
            ReadToMysql();
            QuitWind();
            CommonFunc.CommonSleep("HistoryList", 500);
            QuitBaoBiao();
        }
        /// <summary>
        /// 点击新建
        /// </summary>
        /// <param name="_BigHwnd">句柄</param>
        protected static void ClickNew(IntPtr _BigHwnd)
        {
            GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalXinjianTag);
        }
        /// <summary>
        /// 点击查看
        /// </summary>
        /// <param name="_BigHwnd">句柄</param>
        protected static void ClickLook(IntPtr _BigHwnd)
        {//点击查看   仅仅查看
            string _WindowTitle = "";
            if (!GlobalSystemStop)//判断是否暂停
            {
                if (_BigHwnd != IntPtr.Zero)
                {
                    StringBuilder title = new StringBuilder(102400);
                    GetWindowText(_BigHwnd, title, title.Capacity);//获取窗口标题
                    if (title.ToString().Length > 5)
                    {
                        _WindowTitle = title.ToString();
                    }//获取窗体标题
                    logg.Info("GlobalClickListLocation:" + GlobalClickListLocation);
                    bool _mouse_call;
                    logg.Info("点击查看开始");
                    _mouse_call = MouseClick.AddYanZhengClickOne(_WindowTitle, "KCMLMasterForm_32", _BigHwnd, 40, 316, 2000);//点击查看
                    CommonFunc.CommonSleep("ClickLook", 500);
                    SetCursorPos(40, 100);
                    CommonFunc.CommonSleep("ClickLook", 500);
                    keybd_event(Keys.Down, 0, 0, 0);
                    CommonFunc.CommonSleep("ClickLook", 500);
                    keybd_event(Keys.Enter, 0, 0, 0);
                    CommonFunc.CommonSleep("ClickLook", 500);
                }
            }
        }
        /// <summary>
        /// 点击报表
        /// </summary>
        /// <param name="_BigHwnd">句柄</param>
        protected static void ClickBaobiao(IntPtr _BigHwnd)
        {//点击报表
            //点击内容
            string _WindowTitle = "";
            if (!GlobalSystemStop)//判断是否暂停
            {
                if (_BigHwnd != IntPtr.Zero)
                {
                    StringBuilder title = new StringBuilder(102400);
                    GetWindowText(_BigHwnd, title, title.Capacity);//获取窗口标题
                    if (title.ToString().Length > 5)
                    {
                        _WindowTitle = title.ToString();
                    }//获取窗体标题
                    logg.Info("GlobalClickListLocation:" + GlobalClickListLocation);
                    bool _mouse_call;
                    logg.Info("点击报表");
                    CommonFunc.CommonSleep("ClickLook", 1000);
                    _mouse_call = MouseClick.AddYanZhengClickOne(_WindowTitle, "KCMLMasterForm_32", _BigHwnd, 40, 366, 2000);//点击报表
                    CommonFunc.CommonSleep("ClickLook", 500);
                    keybd_event(Keys.Down, 0, 0, 0);
                    CommonFunc.CommonSleep("ClickLook", 300);
                    keybd_event(Keys.Enter, 0, 0, 0);
                }
            }
        }
        /// <summary>
        /// 点击报表选择
        /// </summary>
        protected static void ClickBaobiaoXuanze()
        {//点击报表选择
            string _WindowTitle = "";
            IntPtr _maindHwnd = FindWindow(null, "报表选择");
            BaoBiaoIntper = _maindHwnd;
            SetWindowPos(_maindHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_maindHwnd);
            CommonFunc.CommonSleep("ClickLook", 1000);
            if (_maindHwnd != IntPtr.Zero)
            {
                StringBuilder title = new StringBuilder(102400);
                GetWindowText(_maindHwnd, title, title.Capacity);//获取窗口标题
                if (title.ToString().Length > 3)
                {
                    _WindowTitle = title.ToString();
                }//获取窗体标题
                MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _maindHwnd, 36, 97, 2000);//点击销售
                CommonFunc.CommonSleep("ClickLook", 500);
                KeyDownBaoBiaoXuanze();
            }
        }
        /// <summary>
        /// 点击推出报表
        /// </summary>
        protected static void QuitBaoBiao()
        {//退出报表
            string _WindowTitle = "";
            if (BaoBiaoIntper != IntPtr.Zero)
            {
                GetmsgProcessNeedFunc.enumwindow(BaoBiaoIntper, ConstGlobalTuiChuBaoBiaoTag);
            }
        }
        /// <summary>
        /// 点击选择某一项目报表
        /// </summary>
        protected static void KeyDownBaoBiaoXuanze()
        {
            CommonFunc.CommonSleep("ClickLook", 1000);
            keybd_event(Keys.Down, 0, 0, 0);
            CommonFunc.CommonSleep("ClickLook", 200);
            keybd_event(Keys.Down, 0, 0, 0);
            CommonFunc.CommonSleep("ClickLook", 200);
            keybd_event(Keys.Down, 0, 0, 0);
            CommonFunc.CommonSleep("ClickLook", 500);
            keybd_event(Keys.Enter, 0, 0, 0);
            CommonFunc.CommonSleep("ClickLook", 500);
        }
        /// <summary>
        /// 点击查找维修
        /// </summary>
        protected static void FindWeixiu()
        {
            string _WindowTitle = "";
            CommonFunc.CommonSleep("FindWeixiu", 1000);
            IntPtr _BigHwnd = FindWindow(null, "维修销售综合查询报表");
            SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_BigHwnd);//置顶
            if (_BigHwnd != IntPtr.Zero)
            {
                GlobalTag = ConstGlobalDateTag;
                GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalDateTag);
                CommonFunc.CommonSleep("FindWeixiu", 500);
                GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalCopyWeiXiuTag);
            }
        }
        /// <summary>
        /// 复制wip
        /// </summary>
        /// <param name="_BigHwnd">句柄</param>
        /// <param name="topoffset">距离高度的良</param>
        /// <param name="leftoffset">距离左边的量</param>
        public static void ClickCopyWip(IntPtr _BigHwnd, int topoffset, int leftoffset)
        {//复制wip
            //模拟按下ctrl键
            keybd_event(vbKeyControl, 0, 0, 0);
            if (_BigHwnd != IntPtr.Zero)
            {
                ClickOneRight(_BigHwnd, topoffset, leftoffset, 2000);
            }
            CommonFunc.CommonSleep("FindWeixiu", 200);
            //松开按键ctrl
            keybd_event(vbKeyControl, 0, 2, 0);
            CommonFunc.CommonSleep("ClickCopyWip", 500);
        }
        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        /// <summary>
        /// 保存粘贴板内容到数据库
        /// </summary>
        protected static void ReadToMysql()//保存粘贴板内容到数据库
        {
            try
            {
                int wips = 0;
                ClipboardAsync Clipboard2 = new ClipboardAsync();
                if (Clipboard2.ContainsText(TextDataFormat.Text))
                {
                    string txts = Clipboard2.GetText(TextDataFormat.Text);
                    logg.Info("复制下来内容了" + txts);
                    string[] sArray = txts.Split(new char[2] { '\r', '\n' });
                    foreach (var txt in sArray)
                    {
                        {
                            string[] aArray = txt.Split('	');
                            bool is_num = IsNumeric(aArray[0]);
                            if (is_num && aArray[0].Length > 4)
                            {
                                int.TryParse(aArray[0], out wips);
                                OperateSql.SaveHistoryWipnum(aArray[0], aArray[1]);
                            }
                        }
                    }
                }
                else
                {
                    logg.Info("ReadToMysql为空");
                }
            }
            catch
            {
                logg.Info("ReadToMysql为空");
            }
        }
        /// <summary>
        /// 输入内容
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <param name="enters">输入的内容</param>
        /// <param name="addpix">增加的像素</param>
        public static void CsjKeyEnter(IntPtr hWnd, string enters, int addpix)
        {//逐步输入内容 hwnd句柄 enters输入的内容 addpix增加的像素
            RECT rect = new RECT();
            GetWindowRect(hWnd, out rect);
            int Topclick = (rect.Top + addpix) * 65535 / GlobalWindowH; // 133  165
            int Leftclick = (rect.Left + addpix) * 65535 / GlobalWindowW; // 129  140
            SetCursorPos(Leftclick, Topclick);
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
            for (int i = 0; i < enters.Length; i++)
            {
                logg.Info(enters[i].ToString());
                Keys _key = (Keys)(48 + int.Parse(enters[i].ToString()));
                keybd_event(_key, 0, 0, 0);
                keybd_event(_key, 0, KEYEVENTF_KEYUP, 0);
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// 点击鼠标右键
        /// </summary>
        /// <param name="mainhwnd">句柄</param>
        /// <param name="topoffset">距离高度</param>
        /// <param name="leftoffset">距离左边</param>
        /// <param name="sleeptime">时间</param>
        public static void ClickOneRight(IntPtr mainhwnd, int topoffset, int leftoffset, int sleeptime)//点击鼠标右键
        {
            RECT rect = new RECT();
            GetWindowRect(mainhwnd, out rect);
            int Topclick = (rect.Top + topoffset) * 65535 / GlobalWindowH; // 131  711
            int Leftclick = (rect.Left + leftoffset) * 65535 / GlobalWindowW; // 481  1216
            SetCursorPos(Leftclick, Topclick);
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, Leftclick, Topclick, 0, 0);
        }
        /// <summary>
        /// 推出
        /// </summary>
        protected static void QuitWind()
        {//推出窗体  esc键
            CommonFunc.CommonSleep("QuitWind", 500);
            keybd_event(vbKeyEscape, 0, 0, 0);
            CommonFunc.CommonSleep("QuitWind", 200);
            //松开按键ctrl
            keybd_event(vbKeyEscape, 0, 2, 0);
            CommonFunc.CommonSleep("QuitWind", 1000);

        }
    }
}
