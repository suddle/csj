using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.CloseWindGetMsg;
using static tengchao.GetmsgProcessNeedFunc;
using static tengchao.PublicDefine;
namespace tengchao
{
    class SearhWindFunc
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        static systemsleep sl = new systemsleep();
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <returns></returns>
        public static bool TryClickCaidan()//点击菜单
        {
            bool _istrue = false;
            IntPtr _CaiDanHwnd = FindWindow(null, "选择菜单中选项");
            SetWindowPos(_CaiDanHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_CaiDanHwnd);
            if (_CaiDanHwnd != IntPtr.Zero)
            {
                _istrue = true;
                bool _mouse_call = MouseClick.AddYanZhengClickTwo("", "ListBox_Class", _CaiDanHwnd, 120, 100, 2000);
            }
            else
            {
                CommonFunc.CommonSleep("ClickCaiDan", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="maindHwnd2"></param>
        /// <returns></returns>
        public static bool TryGetCheLiangMsg(IntPtr maindHwnd2)//获取车辆信息
        {
            bool _istrue = false;
            if (maindHwnd2 != IntPtr.Zero)
            {
                _istrue = true;
                TryClickCrm(maindHwnd2);
                GetCrmMsg();
            }
            else
            {
                CommonFunc.CommonSleep("ClickCheLaingMsg", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 点击车辆
        /// </summary>
        /// <returns></returns>
        public static bool TryClickCheLiang(IntPtr bighwnd)//点击车辆
        {
            string _kehutitle = "";
            bool _istrue = false;
            IntPtr _MaindHwnds = FindWindow("KCMLMasterForm_32", null);
            if (_MaindHwnds != IntPtr.Zero) {
                string _WindowTitle = "";
                StringBuilder _title = new StringBuilder(102400);
                GetWindowText(_MaindHwnds, _title, _title.Capacity);//获取窗口标题
                if (_title.ToString().Length > 1)
                {
                    _WindowTitle = _title.ToString();
                }
                if (_WindowTitle.Contains("客户及车辆"))
                {
                    _kehutitle = _WindowTitle;
                }
            }
            if (_kehutitle.Contains("客户及车辆"))
            {
                if (!GlobalSystemStop)
                {
                    IntPtr _MaindHwnd1 = FindWindow(null, _kehutitle);
                    SetWindowPos(_MaindHwnd1, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_MaindHwnd1);
                    if (_MaindHwnd1 != IntPtr.Zero)
                    {
                        if (!GlobalSystemStop)
                        {
                            logg.Info("开始查找车辆按钮。。。");
                            enumwindow(_MaindHwnd1, ConstCheLiangTag);
                            global_liuchegtag = 4;
                            DefineVar.ZebraInt = global_liuchegtag;
                            _istrue = true;
                        }
                    }
                    else
                    {
                        CommonFunc.CommonSleep("ClickCheLiang", sl.Moment);
                        if (!GlobalSystemStop)
                        {
                            IntPtr _MaindHwnd2 = FindWindow(null, _kehutitle);
                            SetWindowPos(_MaindHwnd2, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                            SetForegroundWindow(_MaindHwnd2);
                            if (_MaindHwnd2 != IntPtr.Zero)
                            {
                                logg.Info("开始查找车辆按钮。。。");
                                CommonFunc.CommonSleep("ClickCheLiang", sl.Little);
                                enumwindow(_MaindHwnd1, ConstCheLiangTag);
                                global_liuchegtag = 4;
                                DefineVar.ZebraInt = global_liuchegtag;
                                _istrue = true;
                            }
                            else
                            {
                                if (!GlobalSystemStop)
                                {
                                    AppareWiplockPage();
                                }
                                global_liuchegtag = 4;
                                _istrue = true;
                            }
                        }
                    }
                }
            }
            else {
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                CommonFunc.CommonSleep("CloseFuJia", 10);
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// wip锁定页面，找到取消按钮
        /// </summary>
        public static void AppareWiplockPage()
        {
            CommonFunc.WaitFindWind(() => TryClickLockWip());
        }
        /// <summary>
        /// 点击锁定的wip号
        /// </summary>
        /// <returns></returns>
        public static bool TryClickLockWip()//点击锁定wip号
        {
            bool _istrue = false;
            IntPtr _MaindHwnd11 = FindWindow("KCMLMasterForm_32", null);
            if (_MaindHwnd11 != IntPtr.Zero)
            {
                enumwindow(_MaindHwnd11, ConstGlobalSuoDIngWipTag);
                _istrue = true;
            }
            else
            {
                CommonFunc.CommonSleep("AppareWiplockPage", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 出现输入确认页面，点击退出，并回退到第一步
        /// </summary>
        /// <returns></returns>
        public static bool KaiDanClick()
        {
            bool _mouse_call;
            bool _is_true = true;
            try
            {
                CommonFunc.CommonSleep("KaiDanClick", sl.Little);
                int _zebracount = 0;
                while (_zebracount < 3)
                {
                    _zebracount++;
                    IntPtr _KaiDanHwnd = FindWindow(null, "输入确认");
                    SetWindowPos(_KaiDanHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_KaiDanHwnd);
                    if (_KaiDanHwnd != IntPtr.Zero)
                    {
                        _mouse_call = MouseClick.AddYanZhengClickTwo("只显示", "KCMLButton_32", _KaiDanHwnd, 150, 300, 500);
                        _is_true = true;
                        break;
                    }
                    else
                    {
                        _is_true = false;
                    }
                }
                return _is_true;
            }
            catch (Exception e)
            {
                logg.Debug("CheLiangTag点击列表后出错，记录，此时应该重头开始。。。报错信息是：" + e.ToString());
                CommonFunc.SendBug("CheLiangTag点击列表后出错", "2", e.ToString(), "get_msg", "KaiDanClick");
                return false;
            }
        }
        /// <summary>
        /// 调用dms入口函数
        /// </summary>
        public static void GetMsg() //获取窗口句柄 启动抓取获取去信息
        {
            try
            {
                CommonFunc.CommonSleep("GetMsg", sl.Little);
                CommonFunc.InitGlobalVars();//调用各个公共变量
                if (!GlobalSystemStop)
                {
                    OpenWindGetMsg.ClickCreateNew();//创建新的点击（开始抓取）
                }
                CommonFunc.CommonSleep("GetMsg", sl.Little);
            }
            catch (Exception ex)
            {
                logg.Error("getmsg初始调用时出错了，报错信息是：" + ex.ToString());
                CommonFunc.SendBug("getmsg初始调用时出错了", "2", ex.ToString(), "get_msg", "GetMsg");
            }
        }
        /// <summary>
        /// 点击车辆，成功后出现crm页面，失败后回退
        /// </summary>
        /// <param name="hWnd"></param>
        public static void CheLiangTag(IntPtr hWnd)
        {
            try
            {
                bool _mouse_call;
                if (!GlobalSystemStop)
                {
                    _mouse_call = MouseClick.AddYanZhengClickOne("", "ToolBar_Class", hWnd, 15, 140, 3000);
                    // 获取退出时得点击坐标
                    logg.Info("点击车辆按键结束");
                    CommonFunc.CommonSleep("CheLiangTag", sl.Longe);
                    if (_mouse_call)
                    {
                        if (!GlobalSystemStop)
                        {
                            IntPtr _HuiCheHwnd = FindWindow("#32770", "按回车键继续");
                            SetWindowPos(_HuiCheHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                            SetForegroundWindow(_HuiCheHwnd);
                            if (_HuiCheHwnd != IntPtr.Zero)
                            {
                                CommonFunc.CommonSleep("CheLiangTag", sl.Little);
                                keybd_event(Keys.Enter, 0, 0, 0);
                                CommonFunc.CommonSleep("CheLiangTag", 10);
                                keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                                CommonFunc.CommonSleep("CheLiangTag", 15000);
                            }
                        }
                        GlobalTag = ConstTagNoop;
                        GetCheLiangInfo();
                        global_liuchegtag = 5;
                        DefineVar.ZebraInt = global_liuchegtag;
                    }
                    else
                    {
                        logg.Info("准备回退");
                    }
                }
            }
            catch (Exception e)
            {
                logg.Info("CheLiangTag" + e.ToString());
                CommonFunc.SendBug("CheLiangTag", "2", e.ToString(), "get_msg", "CheLiangTag");
            }
        }
        //打开crm获取信息 
        public static void GetCrmMsg()
        {
            GlobalTag = ConstTagNoop;
            GlobalEditNum = 0;
            GlobalChePaiHao1 = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 26987);
            GlobalCheLiangXingHao1 = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27004);
            GlobalZhongWenPinPai1 = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 26996);
            GlobalCheShenYanSe1 = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27007);
            GlobalCheJiaHao = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27892);
            GlobalGuoChanCheJiaHao = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27051);
            GlobalKehu = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27210);
            GlobalGongSiName = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBing1CtrlidDict, 27206);
            logg.Info(GlobalGuoChanCheJiaHao + "美版车架号");
            logg.Info(GlobalCheJiaHao + "国产车架号");
            if (GlobalZhongWenPinPai1 == "M")
            {
                GlobalZhongWenPinPai = "奔驰";
            }
            else { GlobalZhongWenPinPai = "其他"; }
            Match match4 = Regex.Match(GlobalCheLiangXingHao1, @"(?!\ d+ $)[ \d a-z A-Z]+");
            if (match4.Success)
            {
                GlobalCheLiangXingHao = match4.Value;
                if (GlobalCheLiangXingHao.Contains("Mercedes"))
                {
                    GlobalCheLiangXingHao = GlobalCheLiangXingHao1.Replace("Mercedes", "").Replace("-", "");
                }
            }
            else
            {
                GlobalCheLiangXingHao = GlobalCheLiangXingHao1;
            }
            ControlText.FindColor(GlobalCheShenYanSe1, out GlobalCheShenYanSe);
            if (GlobalCheShenYanSe != "")
            {
                logg.Info(GlobalCheShenYanSe);
            }
            else
            {
                logg.Error("么有匹配到颜色，抓取到的颜色是：" + GlobalCheShenYanSe1);
            }
            Match match = Regex.Match(GlobalChePaiHao1, @"[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}");
            if (match.Success)
            {
                GlobalChePaiHao = match.Value;
                GlobalChePaiQianZhui = OpenWindGetMsg.SubString(GlobalChePaiHao, 0, 2);
                GlobalChePaiHouZhui = OpenWindGetMsg.SubString(GlobalChePaiHao, 2, 6);
            }
            logg.Info("抓取到的车牌号是：" + GlobalChePaiHao1);
            logg.Info("匹配到的车牌号是：" + GlobalChePaiHao);
            logg.Info("抓取到的车身颜色是：" + GlobalCheShenYanSe1);
            logg.Info("匹配到的车身颜色是：" + GlobalCheShenYanSe);
            logg.Info("抓取到的车辆型号是：" + GlobalCheLiangXingHao1);
            logg.Info("匹配到的车辆型号是：" + GlobalCheLiangXingHao);
            logg.Info("抓取到的中文品牌是：" + GlobalZhongWenPinPai1);
            logg.Info("匹配到的中文品牌是：" + GlobalZhongWenPinPai);
            if (!String.IsNullOrEmpty(GlobalKehu) && GlobalKehu.Length > 2)
            {
                GlobalXingMing = GlobalKehu.Substring(0, GlobalKehu.Length - 2);
            }
            else
            {
                GlobalXingMing = "";
            }
            if (GlobalKehu.Contains("女士") || GlobalKehu.Contains("小姐"))
            {
                GlobalXingBie = "女";
            }
            if (GlobalKehu.Contains("先生") || GlobalKehu.Contains("阁下"))
            {
                GlobalXingBie = "男";
            }
            logg.Info("抓取到的公司名是：" + GlobalGongSiName);
            logg.Info("抓取到的客户是：" + GlobalKehu);
            GlobalEditNum = 0;
        }
        //点击crm 传参是句柄
        public static void TryClickCrm(IntPtr maindHwnd2)
        {
            CommonFunc.CommonSleep("ClickCheLaingMsg", sl.Moment);
            GlobalJuBing1CtrlidDict.Clear();
            EnumWindowsCallBack((IntPtr)maindHwnd2, 0);
            GlobalTag = ConstTagNoop;
            GlobalEditNum = 0;
            GlobalTag = ConstCrmTag1;
            EnumWindowsCallBack((IntPtr)maindHwnd2, 0);
            GlobalTag = ConstTagNoop;
            logg.Info("点击信息二完成");
            GlobalEditNum = 0;
            CommonFunc.CommonSleep("ClickCheLaingMsg", sl.Moment);
            GlobalTag = ConstXgclTag;
            EnumWindowsCallBack((IntPtr)maindHwnd2, 0);
            GlobalTag = ConstTagNoop;
            logg.Info("点击相关车辆完成");
            GlobalEditNum = 0;
            CommonFunc.CommonSleep("ClickCheLaingMsg", sl.Little);
            GlobalTag = ConstCrmTag3;
            EnumWindowsCallBack((IntPtr)maindHwnd2, 0);
        }
        /// 获取工单进场时间
        /// </summary>
        /// <param name="hWnd">工单时间控件句柄</param>
        public static void GetChuChangTime(IntPtr hWnd)
        {
            const int buffer_size = 10240000;
            StringBuilder buffer = new StringBuilder(buffer_size);
            SendMessage(hWnd, WM_GETTEXT, buffer_size, buffer);
            if (GlobalEditNum == 12)
            {
                Match match8 = Regex.Match(buffer.ToString(), @"\d+");
                if (match8.Success && buffer.ToString().StartsWith("2"))
                {
                    logg.Info(buffer.ToString() + "送修时间内容");
                    string songxiutime = GeShiHuaTime(buffer.ToString());
                    logg.Info(songxiutime + "songxiutime送修时间内容");
                    songxiutime = songxiutime.Replace("/", "-");
                    if (songxiutime.Contains("-"))
                    {
                        string[] sArray = songxiutime.Split('-');
                        if (sArray.Length >= 2)
                        {
                            for (int i = 0; i < sArray.Length; i++)
                            {
                                if (i == 0)
                                {
                                    logg.Info(sArray[i] + "songxiutime里遍历");
                                    GlobalSongXiuTimeYear = sArray[i];
                                    logg.Debug("获取送修时间，内容是：" + GlobalSongXiuTimeYear);
                                }
                                if (i == 1)
                                {
                                    GlobalSongXiuTimeMonth = sArray[i];
                                    if (GlobalSongXiuTimeMonth.Length == 1)
                                    {
                                        GlobalSongXiuTimeMonth = "0" + GlobalSongXiuTimeMonth;
                                        logg.Debug("获取送修时间，内容是：" + GlobalSongXiuTimeMonth);
                                    }
                                }
                                if (i == 2)
                                {
                                    GlobalSongXiuTimeDay = sArray[i];
                                    logg.Debug("获取送修时间，内容是：" + GlobalSongXiuTimeDay);
                                }
                            }
                        }
                        else
                        {
                            GlobalSongXiuTimeYear = "";
                            GlobalSongXiuTimeMonth = "";
                            GlobalSongXiuTimeDay = "";
                        }
                    }
                    else
                    {
                        GlobalSongXiuTimeYear = "";
                        GlobalSongXiuTimeMonth = "";
                        GlobalSongXiuTimeDay = "";
                    }
                }
                else
                {
                    GlobalSongXiuTimeYear = "";
                    GlobalSongXiuTimeMonth = "";
                    GlobalSongXiuTimeDay = "";
                }
            }
            if (GlobalEditNum == 13)
            {
                if (buffer.ToString().Contains("."))
                {
                    string[] sArray = buffer.ToString().Split('.');
                    if (sArray.Length >= 1)
                    {
                        GlobalSongXiuTimeHour = sArray[0];
                        logg.Debug("获取送修时间，内容是：" + GlobalSongXiuTimeHour);
                        GlobalSongXiuTimeMinute = sArray[1];
                        logg.Debug("获取送修时间，内容是：" + GlobalSongXiuTimeMinute);
                    }
                    else
                    {
                        GlobalSongXiuTimeHour = "";
                        GlobalSongXiuTimeMinute = "";
                    }
                }
                else
                {
                    GlobalSongXiuTimeHour = "";
                    GlobalSongXiuTimeMinute = "";
                }
            }
            GlobalEditNum++;
        }
        /// <summary>
        /// 转换时间，抓到的不是规则的，需要转换
        /// </summary>
        /// <param name="endday"></param>
        /// <returns></returns>
        public static string GeShiHuaTime(string endday)
        {
            string endtime = "";
            if (!String.IsNullOrEmpty(endday) && endday.Length != 0)
            {
                try
                {
                    Match match = Regex.Match(endday, @"\d+");
                    if (endday != "" && match.Success)
                    {
                        string dateString = "20190101";
                        DateTime dts = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        var startday = int.Parse("2458485");
                        var enddays = int.Parse(endday);
                        int end = enddays - startday;
                        DateTime dt = dts.AddDays(end).Date;
                        endtime = string.Format("{0:d}", dt);
                    }
                    return endtime;
                }
                catch (Exception ex)
                {
                    logg.Info(ex.ToString() + "格式化时间");
                    CommonFunc.SendBug("geshihau出错", "2", ex.ToString(), "getmsg_process_needfunc", "GeShiHuaTime");
                    return "没数据";
                }
            }
            else
            {
                logg.Info("格式化时间为空");
                return "没数据";
            }
        }
    }
}
