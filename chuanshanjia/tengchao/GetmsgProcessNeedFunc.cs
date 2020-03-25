using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.PublicDefine;

namespace tengchao
{
    class GetmsgProcessNeedFunc
    {
        static systemsleep sl = new systemsleep();
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern int GetDlgCtrlID(IntPtr hwnd, out int controlid);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern IntPtr EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, int lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        public static EnumWindowsProc callBackEnumChildWindows = new EnumWindowsProc(EnumChildWindowsCallBack);
        public static EnumWindowsProc callBackEnumWindows = new EnumWindowsProc(EnumWindowsCallBack);
        public const byte vbKeyTab = 0x9;        // TAB 键
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <param name="source">经过Unicode编码的字符串</param>
        /// <returns>正常字符串</returns>
        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
        /// <summary>
        /// 遍历窗体函数
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static bool EnumWindowsCallBack(IntPtr hwnd, int lParam)
        {
            try
            {
                EnumChildWindows(hwnd, callBackEnumChildWindows, 0);
            }
            catch (Exception e)
            {
                logg.Error(e.ToString());
                CommonFunc.SendBug("遍历窗体出错", "2", e.ToString(), "getmsg_process_needfunc", "EnumWindowsCallBack"); 
            }
            return true;
        }
        /// <summary>
        /// 枚举控件，进行判断后执行相关操作
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        static int exit_count = 0;
        static int button_count = 0;
        public static bool EnumChildWindowsCallBack(IntPtr hWnd, int lParam)
        {
            StringBuilder title = new StringBuilder(256);
            GetWindowText(hWnd, title, title.Capacity);// 得到窗口标题
            StringBuilder className = new StringBuilder(256);
            GetClassName(hWnd, className, className.Capacity);//得到窗口的类名
            const int buffer_size = 10240000;
            StringBuilder buffer = new StringBuilder(buffer_size);
            SendMessage(hWnd, WM_GETTEXT, buffer_size, buffer);
            string classname = className.ToString();
            int controlid;
            GetDlgCtrlID(hWnd, out controlid);// 获取控件id
            if (classname == "ThunderRT6ComboBox" && ConstPanDuanChePaiTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    logg.Info("控件顺序是：" + GlobalComboBoxTag.ToString() + "内容是：" + buffer.ToString());
                    if (GlobalComboBoxTag == 3)
                    {
                        if (buffer.ToString() == "京V")
                        {
                            logg.Info("车牌前缀是京V");
                            ControlText.ChooseComboBoxText(hWnd, Unicode2String("\u4eac"));
                        }
                    }
                    GlobalComboBoxTag++;
                }
            }
            else if (classname == "ListBox_Class" && ConstCreateDateTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    logg.Info("进入点击创建日期");
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = (rect.Bottom - rect.Top) / 8 * 7;
                    int leftadd = (rect.Right - rect.Left) / 2;
                    logg.Info("点击查看创建日期得点击" + (topadd).ToString() + "#" + (leftadd).ToString());
                    MouseClick.AddYanZhengClickTwo("", "ListBox_Class", hWnd, topadd, leftadd, sl.HhreeSecond);
                }
            }
            else if (classname == "KCMLGridPad_32" && ConstGlobalCopyTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    OpenWindGetMsg.GetCopyWips(hWnd);
                }
            }
            else if (classname == "ToolBar_Class" && ConstGlobalXinjianTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    string _WindowTitle = "";
                    StringBuilder _title = new StringBuilder(102400);
                    GetWindowText(hWnd, _title, _title.Capacity);//获取窗口标题
                    if (_title.ToString().Length > 5)
                    {
                        _WindowTitle = _title.ToString();
                    }
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = rect.Top;
                    int leftadd = rect.Left;
                    logg.Info(topadd.ToString() + "$$$$" + leftadd.ToString());
                    MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", hWnd, 10, 10, 1000);//点击新建
                }
            }
            else if (classname == "ToolBar_Class" && ConstGlobalTuiChuBaoBiaoTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    string _WindowTitle = "";
                    StringBuilder _title = new StringBuilder(102400);
                    GetWindowText(hWnd, _title, _title.Capacity);//获取窗口标题
                    if (_title.ToString().Length > 5)
                    {
                        _WindowTitle = _title.ToString();
                    }
                    MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", hWnd, 10, 10, 2000);//点击报表
                    CommonFunc.CommonSleep("QuitBaoBiao", 200);
                    MouseClick.AddYanZhengClickTwo(_WindowTitle, "ToolBar_Class", hWnd, 10, 10, 2000);//点击报表
                }
            }
            else if (classname == "KCMLGridPad_32" && ConstGlobalCopyWeiXiuTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    string _WindowTitle = "";
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = rect.Top;
                    int leftadd = rect.Left;
                    CommonFunc.CommonSleep("FindWeixiu", 13000);
                    MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", hWnd, topadd, leftadd + 10, 2000);//点击维修
                    CommonFunc.CommonSleep("FindWeixiu", 500);
                    HistoryWip.ClickCopyWip(hWnd, 6, 10);
                    CommonFunc.CommonSleep("FindWeixiu", 500);
                }
            }
            else if (classname == "ToolBar_Class" && ConstGlobalDaKaiTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    string _WindowTitle = "";
                    StringBuilder _title = new StringBuilder(102400);
                    GetWindowText(hWnd, _title, _title.Capacity);//获取窗口标题
                    if (_title.ToString().Length > 5)
                    {
                        _WindowTitle = _title.ToString();
                    }
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = rect.Top;
                    int leftadd = rect.Left;
                    logg.Info(topadd.ToString() + "$$$$" + leftadd.ToString());
                    MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", hWnd, 10, 80, 1000);//点击打开
                }
            }
            else if (classname == "KTabEars" && ConstGlobalOrderDateTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int top;
                    int left;
                    ControlText.GetexTopLeft(hWnd, out top, out left);
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    logg.Info(controlid.ToString()+"打印获取时间id");
                    SetWindowPos(hWnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(hWnd);
                    GlobalEditNum = 0;
                    logg.Info("点击账单。。。");
                    MouseClick.AddYanZhengClickOne("", "KTabEars", hWnd, 10, 105, 1500);// 105 89
                }
            }
            else if (classname == "Edit" && ConstDmsTag == GlobalTag)//登陆dms时候  输入用户名和密码 
            {
                if (!GlobalSystemStop)
                {
                    if (exit_count == 9)
                    {
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, "");
                        CommonFunc.CommonSleep("const_DMS_TAG", sl.Little);
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, "53.90.146.141");
                    }
                    if (exit_count == 12)
                    {
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, "");
                        CommonFunc.CommonSleep("const_DMS_TAG", sl.Little);
                        SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, "n1w12");
                    }
                }
            }
            else if (classname == "KCMLGridPad_32" && ConstGlobalSettleTag == GlobalTag)//登陆dms时候  输入用户名和密码 
            {
                if (!GlobalSystemStop)
                {
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBing5CtrlidDict[(int)controlid1] = hWnd.ToInt32();
                }
            }
            else if (classname == "Button" && ConstDmsTag == GlobalTag)//登陆dms时候 点击确定按钮
            {
                if (!GlobalSystemStop)
                {
                    if (button_count == 17)
                    {
                        SendMessage(hWnd, BM_CLICK, 0, 0);
                    }
                    CommonFunc.CommonSleep("const_DMS_TAG", sl.Longe);
                }
            }
            else if (classname == "KCMLGridPad_32" && ConstPicTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBing4CtrlidDict[(int)controlid1] = hWnd.ToInt32();
                }
            }
            // 列表页面-点击列表中的某一项
            else if (classname == "KCMLDBEdit_32" && ConstWipTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBingCtrlidDict[(int)controlid1] = hWnd.ToInt32();
                }
            }
            // 点击-车辆
            else if (classname == "ToolBar_Class" && ConstCheLiangTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    CommonFunc.CommonSleep("enum-const_CHELIANG_Tag", 500);
                    SearhWindFunc.CheLiangTag(hWnd);
                }
            }
            // 关闭wip锁定页面
            else if (classname == "KCMLButton_32" && ConstGlobalSuoDIngWipTag == GlobalTag)//KCMLDlgWndClass_32
            {
                if (!GlobalSystemStop)
                {
                    string _WindowTitle = "";
                    StringBuilder _title = new StringBuilder(102400);
                    GetWindowText(hWnd, _title, _title.Capacity);//获取窗口标题
                    if (_title.ToString().Length > 1)
                    {
                        _WindowTitle = _title.ToString();
                    }
                    if (_WindowTitle.Equals("取消"))
                    {
                        OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                        CommonFunc.CommonSleep("CloseFuJia", 10);
                        OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                    }
                    logg.Info("锁定wip好"+ _WindowTitle);
                }
            }
            // 匹配车辆信息
            else if (ConstCrmTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int top;
                    int left;
                    ControlText.GetexTopLeft(hWnd, out top, out left);
                }
            }
            // 点击crm页面的信息二
            else if (classname == "KTabEars" && ConstCrmTag1 == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    CommonFunc.CrmTag1(hWnd);
                }
            }
            //// 获取底盘号
            else if ((classname == "KCMLDBEdit_32" || classname == "Edit") && ConstCrmTag2 == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    string text2 = ControlText.TopLeftGetText(hWnd, 555, 957);
                    Match match6 = Regex.Match(text2, @"[a-zA-Z0-9]{17}");
                    if (match6.Success)
                    {
                        GlobalCheJiaHao = match6.Value;
                        logg.Info("获取车架号：" + GlobalCheJiaHao);
                    }
                }
            }
            //// 获取公司名以及客户姓名
            else if (ConstCrmTag3 == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int top;
                    int left;
                    ControlText.GetexTopLeft(hWnd, out top, out left);
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBing1CtrlidDict[(int)controlid1] = hWnd.ToInt32();
                    GlobalEditNum++;
                }
            }
            // 点击crm页面的相关车辆
            else if (classname == "KTabEars" && ConstXgclTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    CommonFunc.XgclTag(hWnd);
                }
            }
            else if (ConstConcatTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int top;
                    int left;
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBingCtrlidDict[(int)controlid1] = hWnd.ToInt32();
                    ControlText.GetexTopLeft(hWnd, out top, out left);
                    GlobalEditNum++;
                }
            }
            //获取预约出厂 预约进场
            else if (classname == "KCMLDBEdit_32" && ConstChuChangTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    SearhWindFunc.GetChuChangTime(hWnd);
                }
            }
            //获取修理项目信息
            else if (classname == "KCMLEdit32" && ConstXiuLiTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    try
                    {
                        const int buffer_size2 = 10240000;
                        StringBuilder buffer2 = new StringBuilder(buffer_size2);
                        SendMessage(hWnd, WM_GETTEXT, buffer_size2, buffer2);
                        if (buffer2.ToString() != "")
                        {
                            
                            GlobalXiuLiCount += 1;
                            if (GlobalXiuLiCount == 2)
                            {
                                if (GlobalXiuLiContent.Contains(buffer2.ToString()) == false)
                                {
                                    GlobalXiuLiContent += buffer2.ToString().Replace("'", "").Replace('"', ' ');
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logg.Info(ex);
                        CommonFunc.SendBug("获取修理项目信息", "2", ex.ToString(), "getmsg_process_needfunc", "EnumWindowsCallBack");
                    }
                }
            }
            else if (classname == "ListBox_Class" && ConstClickDateTag == GlobalTag)//点击创建日期
            {
                if (!GlobalSystemStop)
                {
                    RECT fx;
                    GetWindowRect(hWnd, out fx);
                    ClientToScreen(out fx);
                    CommonFunc.CommonSleep("const_CLICK_DATE_TAG", sl.Little);
                    SetCursorPos((fx.Left + 60) * 65535 / GlobalWindowW + 20, (fx.Top + 120) * 65535 / GlobalWindowH + 20);
                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, (fx.Left + 60) * 65535 / GlobalWindowW, (fx.Top + 120) * 65535 / GlobalWindowH, 0, 0);
                    CommonFunc.CommonSleep("const_CLICK_DATE_TAG", sl.Little);
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (fx.Left + 60) * 65535 / GlobalWindowW, (fx.Top + 120) * 65535 / GlobalWindowH, 0, 0);
                    CommonFunc.CommonSleep("const_CLICK_DATE_TAG", sl.Little);
                }
            }
            else if (classname == "KCMLDBEdit_32" && ConstSendDataTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    SendKeys.SendWait(GlobalTenDayBefore);
                }
            }
            else if (ConstXieRuTag == GlobalTag)
            {//把当前的窗体下所有句柄遍历  存储到字典中
                if (!GlobalSystemStop)
                {
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBing2CtrlidDict[(int)controlid1] = hWnd.ToInt32();
                    GlobalEditNum++;
                }
            }
            else if (ConstOldUserTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    GlobalJuBing3CtrlidDict[(int)controlid1] = hWnd.ToInt32();
                    GlobalEditNum++;

                }
            }
            else if (classname == "ThunderRT6TextBox" && ConstQuCheTimeTag == GlobalTag)//取车时间
            {
                if (!GlobalSystemStop)
                {
                    if (GlobalTextBoxTag == 16)
                    {
                        ControlText.XieRuMsgToHwnd(hWnd, GlobalQuCheTimeHour);
                    }
                    if (GlobalTextBoxTag == 17)
                    {
                        ControlText.XieRuMsgToHwnd(hWnd, GlobalQuCheTimeMinute);
                    }
                    GlobalTextBoxTag++;
                }
            }
            else if (classname == "ThunderRT6TextBox" && ConstOldUserInfoTag == GlobalTag)//老用户
            {
                if (!GlobalSystemStop)
                {
                    if (GlobalTextBoxTag == 22 && buffer.ToString() != "")
                    {
                        GlobalIsOldUser = true;
                    }
                    GlobalTextBoxTag++;
                }
            }
            else if (classname == "ThunderRT6TextBox" && ConstPanDuanTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    if (GlobalTextBoxTag == 1)
                    {
                        ControlText.XieRuMsgToHwnd(hWnd, "");
                        CommonFunc.CommonSleep("const_Panduan_Tag", sl.Little);
                        ControlText.XieRuMsgToHwnd(hWnd, GlobalChePaiHouZhui);
                        CommonFunc.CommonSleep("const_Panduan_Tag", sl.Longe);
                        GlobalPanDuanBiaoZhi = 0;
                    }
                    GlobalTextBoxTag++;
                }
            }
            else if (classname == "ThunderRT6TextBox" && ConstFindInfoTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    // 增加时间的获取，用来判断工单是否上传
                    if (GlobalTextBoxTag == 27)
                    {
                        GlobalSongXiuYuePanDuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 26)
                    {
                        GlobalSongXiuDayPanDuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 25)
                    {
                        GlobalSongXiuHourPanDuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 24)
                    {
                        GlobalSongXiuMinutePanDuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 30)
                    {
                        GlobalFirstWeiXiuRenYuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 9)
                    {
                        GlobalWeiXiuPanDuan = buffer.ToString();
                    }
                    if (GlobalTextBoxTag == 18)
                    {
                        GlobalQuChePanDuan = buffer.ToString();
                    }
                    GlobalTextBoxTag++;
                }
            }
            else if (classname == "ThunderRT6TextBox" && ConstGawTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    if (GlobalEditNum == 1)
                    {
                        ControlText.XieRuMsgToHwnd(hWnd, "111");
                    }
                    if (GlobalEditNum == 2)
                    {
                        ControlText.XieRuMsgToHwnd(hWnd, "111");
                    }
                    GlobalEditNum++;
                }

            } //// 获取公司名以及客户姓名
            else if (classname == "KCMLDBEdit_32" && ConstGlobalDateTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = rect.Top;
                    int leftadd = rect.Left;
                    int controlid1 = GetWindowLong(hWnd, -12);// 获取控件id
                    string yestoday= DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    if (controlid1.Equals(26695)) {
                        HistoryWip.CsjKeyEnter(hWnd, yestoday, 8);
                        CommonFunc.CommonSleep("ConstGlobalDateTag", 200);

                        keybd_event(vbKeyTab, 0, 0, 0);//按下tab键
                        CommonFunc.CommonSleep("ConstGlobalDateTag", 50);
                        keybd_event(vbKeyTab, 0, 2, 0);
                        CommonFunc.CommonSleep("ConstGlobalDateTag", 50);//抬起tab键

                        OpenWindGetMsg.keybd_event(Keys.Enter, 0, 0, 0);//按下enter键
                        CommonFunc.CommonSleep("ConstGlobalDateTag", 10);
                        OpenWindGetMsg.keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);//抬起enter键
                    }
                }
            }
            else if (classname == "KCMLGridPad_32" && ConstGlobalWeiXiuXiamgMuTag == GlobalTag)
            {
                if (!GlobalSystemStop)
                {
                    RECT rect = new RECT();
                    GetWindowRect(hWnd, out rect);
                    int topadd = rect.Top;
                    int leftadd = rect.Left;
                    if (leftadd < 20) {
                        logg.Info("维修项目坐标" + topadd.ToString() + "@@" + leftadd.ToString());
                        CommonFunc.CommonSleep("GetXiuLiXiangMuInfo", 500);
                        OpenWindGetMsg.CilikRights("GetXiuLiXiangMuInfo", hWnd, 10, 24);
                        CloseWindGetMsg.ReadWeiXiu();
                        CommonFunc.CommonSleep("GetXiuLiXiangMuInfo", 300);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 枚举控件，找到后对其中某些控件进行操作
        /// </summary>
        /// <param name="jubing"></param>
        /// <param name="biaoji"></param>
        // 枚举控件函数-代码优化
        public static void enumwindow(IntPtr jubing, int biaoji)
        {
            GlobalTag = biaoji;
            CommonFunc.CommonSleep("enumwindow", sl.Little);
            EnumWindowsCallBack((IntPtr)jubing, 0);
            GlobalTag = ConstTagNoop;
        }
        
    }
}
