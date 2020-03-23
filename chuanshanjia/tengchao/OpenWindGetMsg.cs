using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.CloseWindGetMsg;
using static tengchao.GetmsgProcessNeedFunc;
using static tengchao.PublicDefine;
using static tengchao.SearhWindFunc;
using static tengchao.FreePermission;
using Uploads;
namespace tengchao
{
    class OpenWindGetMsg
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        static systemsleep sl = new systemsleep();
        private MainWind mainWind;
        public OpenWindGetMsg(MainWind mainWind)
        {
            this.mainWind = mainWind;
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
                    ClickCreateNew();//创建新的点击（开始抓取）
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

        /// <summary>
        /// 出现菜单页面，点击关闭，返回值true，调用回退函数
        /// </summary>
        /// <returns></returns>
        public static bool ClickCaiDan()
        {
            bool _mouse_call = true;
            CommonFunc.WaitFindWind(() => TryClickCaidan());
            return _mouse_call;
        }
        /// <summary>
        /// 点击列表中某一项，点击出现非客户及车辆页面，都回退到第一步，成功的话继续点击车辆
        /// </summary>
        public static void GetClick(IntPtr bighwnd)
        {
            GlobalCounts = 0;
            GlobalChuChangCount = 0;
            GlobalXiuLiContent = "";
            bool _enter_page;
            try
            {
                if (!GlobalSystemStop)
                {
                    CommonFunc.CommonSleep("GetClick", sl.Moment);
                    GlobalTag = ConstTagNoop;
                    if (!GlobalSystemStop)
                    {
                        _enter_page = KaiDanClick();//首先判断是不是已开单或低权限用户
                        if (_enter_page)//（已开单或低权限用户）都点只显示
                        {
                            logg.Info("进入了错误页面--输入确认--，开始回退");
                        }
                        else
                        {//一般用户或正常进入
                            CommonFunc.CommonSleep("GetClick", sl.Moment);
                            _enter_page = ClickCaiDan();
                            logg.Info("开始点击车辆");
                            CommonFunc.CommonSleep("GetClick", sl.Moment);
                            ClickCheLiang(bighwnd);
                        }
                    }
                    else
                    {
                        if (GlobalKeyDown >= 1)
                        {
                            GlobalKeyDown--;
                        }
                        logg.Info("进入了错误页面--其他--，开始回退");
                        CommonFunc.CommonSleep("GetClick", sl.Longe);
                    }
                }
            }
            catch (Exception e)
            {
                logg.Info("CheLiangTag" + e.ToString());
                CommonFunc.SendBug("GetClick出错", "2", e.ToString(), "get_msg", "GetClick");
            }
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
                if (GlobalCheLiangXingHao.Contains("Mercedes")) {
                    GlobalCheLiangXingHao = GlobalCheLiangXingHao1.Replace("Mercedes", "").Replace("-","");
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
        //点击车辆信息
        public static void ClickCheLaingMsg(IntPtr maindHwnd2)//点击车辆
        {
            CommonFunc.WaitFindWind(() => TryGetCheLiangMsg(maindHwnd2));
            CommonFunc.CommonSleep("ClickCheLaingMsg", sl.Longe);
            GlobalEditNum = 0;
        }
        /// <summary>
        ///  切割字符串
        /// </summary>
        /// <param name="toSub">需要切割的字符串</param>
        /// <param name="startIndex">开始索引处</param>
        /// <param name="length">切割长度</param>
        /// <returns></returns>
        public static string SubString(string toSub, int startIndex, int length)
        {
            byte[] subbyte = System.Text.Encoding.Default.GetBytes(toSub);
            string _Sub = System.Text.Encoding.Default.GetString(subbyte, startIndex, length);
            return _Sub;
        }
        /// <summary>
        /// 点击工单后可能出现的状况，1-客户及车辆页面，2-wip被锁定，3-什么都没有出现
        /// </summary>
        public static void ClickCheLiang(IntPtr bighwnd)
        {
            CommonFunc.WaitFindWind(() => TryClickCheLiang(bighwnd));
        }
        // 对控件输入内容
        public static void fill_wip_num_totext(IntPtr hwnd, string text)
        {

            ControlText.XieRuMsgToHwnd(CommonFunc.GetIntptrFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26684), text);
        }
        //点击新建
        public static bool TryClickCreateNew()
        {
            string _WindowTitle = "";
            bool _istrue = false;
            if (!GlobalSystemStop)
            {
                IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);
                SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_BigHwnd);
                if (_BigHwnd != IntPtr.Zero)
                {
                    StringBuilder _title = new StringBuilder(102400);
                    GetWindowText(_BigHwnd, _title, _title.Capacity);//获取窗口标题
                    if (_title.ToString().Length > 5)
                    {
                        _WindowTitle = _title.ToString();
                    }
                    logg.Info("GlobalClickListLocation:" + GlobalClickListLocation);
                    if (!GlobalSystemStop)
                    {
                        bool _mouse_call=true;
                        //_mouse_call = MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _BigHwnd, 74, 23, 1000);//点击新建
                        GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalXinjianTag);
                        if (_mouse_call)
                        {

                            //单个抓取测试
                            //SearchWipNum(_BigHwnd, _WindowTitle, "27064");//传wip号
                            //GetKeHuAndOtherInfoInfo();
                            //return true;
                            //HistoryWip.HistoryList();
                            //return true;
                            logg.Info("点击新建完成");
                            if (!GlobalSystemStop)
                            {
                                GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalDaKaiTag);
                                //_mouse_call = MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _BigHwnd, 74, 74, 2000);//点击打开
                                if (_mouse_call)
                                {
                                    logg.Info("进来维修销售");
                                    logg.Info("点击打开");
                                    if (!GlobalSystemStop)
                                    {
                                        string colsetimes = OperateSql.GetDmsInfo("closetime"); //查看是否过了关闭时间
                                        if (DateTime.Now > DateTime.Parse(DateTime.Now.ToShortDateString() + " " + colsetimes + ":00"))  //23:30
                                        {//如果时间大于23；30  那么程序就自动退出
                                            Application.Exit();
                                            Application.ExitThread();
                                            System.Environment.Exit(System.Environment.ExitCode);
                                        }
                                        else
                                        {
                                            FindWipList(_BigHwnd, _WindowTitle);//查找列表
                                            SendMessage(_BigHwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);//窗体置顶
                                        }
                                    }
                                }
                                else
                                {
                                    logg.Info("准备开始回退");
                                }
                                global_liuchegtag = 1;
                                DefineVar.ZebraInt = global_liuchegtag;
                                logg.Info("GlobalWipNum是：" + GlobalWipNum + "@" + "global_last_wip是：" + GlobalLastWip + "@GlobalWipYuQi是：" + GlobalWipYuQi);
                            }
                        }
                        else
                        {
                            logg.Info("准备开始回退");
                        }
                        SetWindowPos(_BigHwnd, HWND_BOTTOM, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                        _istrue = true;
                    }
                }
                else
                {
                    CommonFunc.CommonSleep("clickCreateNew", 500);
                    _istrue = false;
                }
            }
            return _istrue;
        }
        /// <summary>
        /// 点击新建、打开，都成功后调用点击列表函数，有失败则回退
        /// </summary>
        public static void ClickCreateNew()
        {
            try
            {
                GlobalZhongWenPinPai = "奔驰";
                CommonFunc.WaitFindWind(() => TryClickCreateNew());
            }
            catch (Exception ex)
            {
                logg.Info("clickCreateNew" + ex.ToString());
                CommonFunc.SendBug("clickCreateNew出错", "2", ex.ToString(), "get_msg", "GetClick");
            }
        }
        //点击右键  _windowtitle标题 _bighwnd句柄 topadd一句宁为原点 距离高的偏移量 leftadd 以句柄为原点 距离左边的偏移量
        public static void CilikRight(string _WindowTitle, IntPtr _BigHwnd, int topadd, int leftadd) {//点击右键
            logg.Info("标题111"+_WindowTitle+"//"+topadd.ToString()+"//"+leftadd.ToString());
            MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _BigHwnd, topadd-20, leftadd, 2000);//点击报表
            CommonFunc.CommonSleep("CilikRight", 500);
            keybd_event(HistoryWip.vbKeyControl, 0, 0, 0);
            if (_BigHwnd != IntPtr.Zero)
            {
                ClickOneRight(_BigHwnd, topadd , leftadd, 2000);
            }
            CommonFunc.CommonSleep("FindWeixiu", 200);
            //松开按键ctrl
            keybd_event(vbKeyControl, 0, 2, 0);
            CommonFunc.CommonSleep("FindWeixiu", 1000);
        }
        public static void CilikRights(string _WindowTitle, IntPtr _BigHwnd, int topadd, int leftadd)
        {//点击右键
            logg.Info("标题111" + _WindowTitle + "//" + topadd.ToString() + "//" + leftadd.ToString());
            CommonFunc.CommonSleep("CilikRight", 500);
            keybd_event(HistoryWip.vbKeyControl, 0, 0, 0);
            if (_BigHwnd != IntPtr.Zero)
            {
                ClickOneRight(_BigHwnd, topadd, leftadd, 2000);
            }
            CommonFunc.CommonSleep("FindWeixiu", 200);
            //松开按键ctrl
            keybd_event(vbKeyControl, 0, 2, 0);
            CommonFunc.CommonSleep("FindWeixiu", 1000);
        }
        //点击右键 mainhwnd 句柄 topadd 以句柄为原点 距离高度的偏移量  leftadd 以句柄为原点  距离左边的偏移量
        public static void ClickOneRight(IntPtr mainhwnd, int topadd, int leftadd, int sleeptime)//点击鼠标右键
        {
            RECT rect = new RECT();
            GetWindowRect(mainhwnd, out rect);
            int Topclick = (rect.Top + topadd) * 65535 / GlobalWindowH; // 131  711
            int Leftclick = (rect.Left + leftadd) * 65535 / GlobalWindowW; // 481  1216
            SetCursorPos(Leftclick, Topclick);
            logg.Info("点击复制wip号搜索右键" + (Topclick ).ToString() + "#" + (Leftclick).ToString());
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, Leftclick, Topclick, 0, 0);
        }
        //复制list 
        public static List<int> GetCopyList() {//复制下来得list
            List<int> alist = new List<int>();
            try
            {
                int wips = 0;
                ClipboardAsync Clipboard2 = new ClipboardAsync();
                if (Clipboard2.ContainsText(TextDataFormat.Text))
                {
                    string txts = Clipboard2.GetText(TextDataFormat.Text);
                    logg.Info("复制下来的内容"+txts);
                    string[] sArray = txts.Split(new char[2] { '\r', '\n' });
                    foreach (var txt in sArray)
                    {
                        {
                            string[] aArray = txt.Split('	');
                            bool is_num = HistoryWip.IsNumeric(aArray[0]);
                            if (is_num && aArray[0].Length > 4)
                            {
                                int.TryParse(aArray[0], out wips);
                                alist.Add(wips);
                                if (!GlobalWipList.Contains(wips)) {
                                    GlobalWipList.Add(wips);
                                }
                            }
                        }
                    }
                }
                else
                {
                    logg.Info("ReadToMysql为空1");
                }
            }
            catch
            {
                logg.Info("ReadToMysql为空2");
            }
            return alist;
        }
        //复制wip号搜索出来得wip 参数为句柄
        public static void GetCopyWips(IntPtr _BigHwnd)
        {
            string _WindowTitle = "";
            CommonFunc.CommonSleep("GetCopyWip", 5000);
            int _NextWipNum = 0;
            //IntPtr _BigHwnd = FindWindow(null, "WIP号搜索");
            StringBuilder _title = new StringBuilder(102400);
            GetWindowText(_BigHwnd, _title, _title.Capacity);//获取窗口标题
            if (_title.ToString().Length > 5)
            {
                _WindowTitle = _title.ToString();
                logg.Info("获取窗口标题" + _WindowTitle);
            }
            if (_BigHwnd != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(_BigHwnd, out rect);
                int topadd = rect.Top;
                int leftadd = rect.Left;
                logg.Info("点击复制wip号搜索" + topadd.ToString() + "#" + leftadd.ToString());
                while (true)
                {
                    try
                    {
                        //291 640 
                        //519 670 
                        int _wiplistmin = 0;
                        CilikRight(_WindowTitle, _BigHwnd, 5, 5);
                        CommonFunc.CommonSleep("GetCopyWip", 300);
                        
                        List<int> _wiplists = GetCopyList();//wip号搜索页面
                        if (_wiplists.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            _wiplistmin = _wiplists.Min();
                        }
                        if (_wiplistmin.ToString() == _NextWipNum.ToString())
                        {
                            break;
                        }
                        _NextWipNum = _wiplistmin;
                        keybd_event(Keys.Next, 0, 0, 0);
                        CommonFunc.CommonSleep("GetCopyWip", 10);
                        keybd_event(Keys.Next, 0, KEYEVENTF_KEYUP, 0);
                        CommonFunc.CommonSleep("GetCopyWip", 300);
                    }
                    catch (Exception ex)
                    {
                        logg.Info("ClickWipListForScreen--" + ex.ToString());
                        break;
                    }
                }
            }
            keybd_event(Keys.Escape, 0, 0, 0);
            CommonFunc.CommonSleep("ClickWipListForScreen", 10);
            keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
        }

        public static void GetCopyWip(IntPtr _BigHwnd)//复制wip号搜索出来得wip
        {
            string _WindowTitle = "";
            string _PicPath2 = @"wip1.png";
            string _PicPath1 = @"wip2.png";

            CommonFunc.CommonSleep("GetCopyWip",5000);
            int _NextWipNum = 0;
            IntPtr _MaindHwnd = FindWindow(null, "WIP号搜索");
            StringBuilder _title = new StringBuilder(102400);
            GetWindowText(_MaindHwnd, _title, _title.Capacity);//获取窗口标题
            if (_title.ToString().Length > 5)
            {
                _WindowTitle = _title.ToString();
                logg.Info("获取窗口标题"+ _WindowTitle);
            }
            if (_MaindHwnd != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(_MaindHwnd, out rect);
                int topadd = rect.Top;
                int leftadd = rect.Left;

                GetWindowRect(_BigHwnd, out rect);
                int topadds = (rect.Bottom - rect.Top) / 8 * 7;
                int leftadds = (rect.Right - rect.Left) / 2;
                logg.Info("点击复制wip号搜索"+topadd.ToString()+"#"+leftadd.ToString());
                while (true)
                {
                    try
                    {
                        //291 640 
                        //519 670 
                        int _wiplistmin = 0;
                        CilikRight(_WindowTitle, _MaindHwnd, topadds+50, leftadds);
                        CommonFunc.CommonSleep("GetCopyWip", 300);
                        logg.Info("点击复制wip号搜索右键" + (topadds + 50).ToString() + "#" + (leftadds).ToString());
                        List<int> _wiplists = GetCopyList();//wip号搜索页面
                        if (_wiplists.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            _wiplistmin = _wiplists.Min();
                        }
                        if (_wiplistmin.ToString() == _NextWipNum.ToString())
                        {
                            break;
                        }
                        _NextWipNum = _wiplistmin;
                        keybd_event(Keys.Next, 0, 0, 0);
                        CommonFunc.CommonSleep("GetCopyWip", 10);
                        keybd_event(Keys.Next, 0, KEYEVENTF_KEYUP, 0);
                        CommonFunc.CommonSleep("GetCopyWip",300);
                    }
                    catch (Exception ex)
                    {
                        logg.Info("ClickWipListForScreen--" + ex.ToString());
                        break;
                    }
               }
            }
            keybd_event(Keys.Escape, 0, 0, 0);
            CommonFunc.CommonSleep("ClickWipListForScreen", 10);
            keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
        }
        //找到wip奥并进行搜索 bighwnd是句柄 title是标题
        public static bool TryFindWipList(IntPtr bighwnd, string window_title)//通过wip号搜索来查找wip
        {
            bool _istrue = true;
            bool _mouse_call;
            IntPtr _MaindHwnd = FindWindow(null, "WIP号搜索");//找到wip号搜索句柄
            SetWindowPos(_MaindHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_MaindHwnd);
            if (_MaindHwnd != IntPtr.Zero)
            {
                if (!GlobalSystemStop)
                {
                        GlobalEditNum = 0;
                        enumwindow(_MaindHwnd, ConstCreateDateTag);//双击创建日期
                        GlobalEditNum = 0;
                        _mouse_call = true;
                        if (_mouse_call)
                        {
                            CommonFunc.CommonSleep("copy",100);
                            logg.Info("key_down的值：" + GlobalKeyDown);
                            enumwindow(_MaindHwnd, ConstGlobalCopyTag);//点击复制
                           // GetCopyWip(_MaindHwnd);//复制wip号
                            // 截图处理wip号，并且生成1个列表， 需要抓取的wip号列表
                            string dayspider = OperateSql.GetDmsInfo("dayspider");//读取是否是专注模式
                            if (dayspider.Contains("true"))
                            {
                                    GetEnterWipList(bighwnd, window_title);
                                    GetOldWip(bighwnd, window_title,"true");
                                    GetHistory(bighwnd, window_title);
                            }
                            else {
                                // 上传已出场与历史工单重复，如果GlobalQueShiWipList（缺失工单列表）是空或者不是null 那么就有限抓取新入场工单  然后抓取完再抓取一个历史工单
                                if (GlobalQueShiWipList.Count != 0 && GlobalQueShiWipList != null)
                                {
                                    GetEnterWipList(bighwnd, window_title);
                                    GetOldWip(bighwnd, window_title, "true");
                                    GetHistory(bighwnd, window_title);
                                }
                                else
                                {
                                    //如果globalwiplist（新入场工单列表）不是空  那么就有限抓取新入场工单  然后抓取完再抓取一个历史工单
                                    if (GlobalWipList.Count != 0 && GlobalWipList != null)
                                    {
                                        GetEnterWipList(bighwnd, window_title);
                                        GetOldWip(bighwnd, window_title, "true");
                                    }
                                    else
                                    {//否则  就根据新入场工单列表的内的数量以及时间判断抓取方法 
                                        GetOldWip(bighwnd, window_title, "true");
                                        GetHistory(bighwnd, window_title);
                                    }
                                }
                            }
                            CommonFunc.CommonSleep("TryFindWipList", sl.Moment);
                        }
                }
                global_liuchegtag = 2;
                DefineVar.ZebraInt = global_liuchegtag;
                _istrue = true;
            }
            else
            {
                CommonFunc.CommonSleep("FindWipList", 1500);
                _istrue = false;
            }
            return _istrue;
        }
        //抓取刚入场的工单函数 bighwnd 是句柄 window_title是标题
        public static void GetEnterWipList(IntPtr bighwnd, string window_title)//上传新入场工单函数
        {
            GlobalIsNew = "true";
            while (true)
            {
                if (GlobalWipList.Count > 0 && GlobalWipList != null)//判断公共wiplist列表中是否有值  没有就跳出
                {
                    logg.Info("开始抓取");
                    string dt1 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    CommonFunc.InitGlobalVars();//清空各个变量得内容
                    string _day = DateTime.Now.ToString("dd");//获取当前时间
                    GlobalSongXiuTimeDay = _day;
                    if (GlobalSystemStop)
                    {
                        CommonFunc.CommonSleep("FindWipList", sl.Moment);
                        continue;
                    }
                    string minwip = GlobalWipList.Min().ToString();//获取新入场列表中最小的值
                    //int datawips= OperateSql.GetWipCountFromData(minwip);
                    int _WipCount = OperateSql.GetDataWipCount(minwip);//判断数据库里有没有
                    int oldcount = OperateSql.GetOldWipCount(minwip);
                    int historycount = InfoSql.GetCountFromDatatb(minwip);
                    int isinhistorydata = 1;
                    if (_WipCount == 0)
                    {//判断 如果data表里此条信息抓取成功了  那么就不抓取了
                        SearchWipNum(bighwnd, window_title, minwip);//传wip号
                        GetKeHuAndOtherInfoInfo();//获取用户信息，wip号，订单状态，客户不为空时获取修理项目
                        SaveData();//保存内容
                        string dt2 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        logg.Info("wip号" + GlobalWipNum + "抓取开始" + dt1 + "$" + "抓取结束" + dt2);
                        logg.Info("a是否"+GlobalIsFull.Equals("true").ToString()+"是否"+ CommonFunc.IsValidRecordName(GlobalKehu).ToString());
                    }
                    else {//如果没抓取成功 那么就判断是否人工已经上传了  没上传那么就放到抓取不成功里面等待抓取
                        logg.Info(GlobalWipList.Count.ToString()+"最全的列表数目");
                        int uploadcount=OperateSql.GetUploadRecordWipCount(minwip);
                        if (oldcount == 0 && uploadcount==0&& historycount==1) {
                            OperateSql.SaveOldWipnum(minwip, GlobalIsNew);
                        }
                    }
                    string dt3 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    logg.Info("wip号" + GlobalWipNum + "抓取开始" + dt1 + "$" + "上传结束" + dt3);
                    // 每次过程结束更新抓取上传数值、更新log以及窗体记录
                    MainWind.myTopMost.update_hwind_txt();
                    //最大化dms页面
                    int _wipnum = 0;
                    Int32.TryParse(minwip, out _wipnum);//最小值转换为int类型 
                    logg.Info("循环最小值是" + minwip);
                    //try {
                    for (int i = GlobalWipList.Count - 1; i >= 0; i--)
                    {
                        if (GlobalWipList[i] == _wipnum)
                        {
                            GlobalWipList.RemoveAt(i);//删除抓取过得wIP号 
                        }
                    }
                    if (GlobalWipList.Count <= 0)
                    {
                        break;   //跳出
                    }
                }
                else
                {
                    break;
                }
            }
        }

        //抓取不完整工单 bighwnd是句柄 window_title是标题 isnew是否是新工单
        protected static void GetOldWip(IntPtr bighwnd, string window_title,string isnew)
        {
            if (GlobalWipList.Count == 0)
            {
                string oldwip = OperateSql.oldwip(isnew);
                int _WipCount = OperateSql.GetDataWipCount(oldwip);//判断数据库里有没有
                if (!oldwip.Equals(string.Empty) && !oldwip.Equals(null) && _WipCount==0)
                {//判断 如果抓取成功列表没有此wip号  才会抓取
                    string dt1 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    SearchWipNum(bighwnd, window_title, oldwip);//传最小的歷史wip号
                    GetKeHuAndOtherInfoInfo();
                    SaveData();
                    string dt2 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    logg.Info("wip号" + GlobalWipNum + "抓取开始" + dt1 + "$" + "抓取结束" + dt2);
                    logg.Info("b是否" + GlobalIsFull.Equals("true").ToString() + "是否" + CommonFunc.IsValidRecordName(GlobalKehu).ToString());
                    string dt3 = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    logg.Info("wip号" + GlobalWipNum + "抓取开始" + dt1 + "$" + "上传结束" + dt3);
                    // 每次过程结束更新抓取上传数值、更新log以及窗体记录
                    MainWind.myTopMost.update_hwind_txt();
                    //最大化dms页面
                }
            }
            if (GetDataAnfHistoryMinWip().Count > 0)
            {
                int queshiwip = GetDataAnfHistoryMinWip().Min();
                if (!GlobalWipList.Contains(queshiwip))
                {
                    GlobalWipList.Add(queshiwip);
                }
            }
        }
        //获取历史工单 bighwnd是句柄 window_title是标题
        public static void GetHistory(IntPtr bighwnd, string window_title) {
            HistoryWip.HistoryList();
            if (GetDataAnfHistoryMinWip().Count > 0) {
                int queshiwip = GetDataAnfHistoryMinWip().Min();
                if (!GlobalWipList.Contains(queshiwip))
                {
                    GlobalWipList.Add(queshiwip);
                }
            }
        }
        //获取最小值  从历史表
        public static List<int> GetDataAnfHistoryMinWip()
        {
            List<int> spiderwiplist = new List<int>();
            List<int> wiplist = InfoSql.GetWipFromHistory();
            foreach (var wip in wiplist)
            {
                if (OperateSql.GetDataWipAndHistoryCount(wip.ToString()) < 1)
                {
                    spiderwiplist.Add(wip);
                }
            }
            return spiderwiplist;
        }
        /// <summary>
        /// 处理出一个可执行列表，然后进行处理
        /// 大循环的主要函数
        /// </summary>
        public static void FindWipList(IntPtr bighwnd, string window_title)
        {
            CommonFunc.WaitFindWind(() => TryFindWipList(bighwnd, window_title));
        }
        /// <summary>
        /// 搜索wip号并且回车，继续后续操作
        /// </summary>
        /// <param name="bighwnd">最大窗体句柄</param>
        /// <param name="window_title">窗体标题</param>
        /// <param name="wip_num">wip号</param>
        public static void SearchWipNum(IntPtr bighwnd, string window_title, string wip_num)
        {
            GlobalWipNum = wip_num;
            SetWindowPos(bighwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            if (!GlobalSystemStop)
            {
                bool _mouse_call=true;
               // _mouse_call = MouseClick.AddYanZhengClickOne(window_title, "ToolBar_Class", bighwnd, 70, 24, 2000);
                enumwindow(bighwnd, ConstGlobalXinjianTag);
                if (_mouse_call)
                {
                    GlobalJuBingCtrlidDict.Clear();
                    enumwindow(bighwnd, ConstWipTag);
                    IntPtr _WipControlHwnd = CommonFunc.GetIntptrFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26684);
                    RECT rect = new RECT();
                    GetWindowRect(_WipControlHwnd, out rect);
                    int Topclick = (rect.Top + 10) * 65535 / GlobalWindowH; // 133  165
                    int Leftclick = (rect.Left + 25) * 65535 / GlobalWindowW; // 129  140
                    SetCursorPos(Leftclick + 50, Topclick + 50);
                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE, Leftclick, Topclick, 0, 0);
                    mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, Leftclick, Topclick, 0, 0);
                    CommonFunc.CommonSleep("ClickOne", sl.Little);
                    logg.Info("逐个输入字符");
                    fill_wip_num_totext(_WipControlHwnd, "");
                    for (int i = 0; i < wip_num.Length; i++)
                    {
                        logg.Info(wip_num[i].ToString());
                        Keys _key = (Keys)(48 + int.Parse(wip_num[i].ToString()));
                        keybd_event(_key, 0, 0, 0);
                        keybd_event(_key, 0, KEYEVENTF_KEYUP, 0);
                        CommonFunc.CommonSleep("SearchWipNum", 100);
                    }
                    // 獲取歷史表所有的Wip號
                    List<string> _AllHisWip = OperateSql.GetAllHistoryWip();
                    if (_AllHisWip.Contains(GlobalWipNum))
                    {// 增加判斷，此條數據如果屬於歷史表，則刪除
                        OperateSql.DeleteHistoryWip(GlobalWipNum);
                    }
                    logg.Info("输入完成");
                    SendMessage(_WipControlHwnd, WM_CHAR, (IntPtr)VK_RETURN, IntPtr.Zero);//Enter
                    logg.Info("enter键操作完成");
                    CommonFunc.CommonSleep("SearchWipNum", sl.Longe);
                    IntPtr jixuwin = FindWindow(null, "按回车键继续");
                    if (jixuwin != IntPtr.Zero)
                    {
                        SetWindowPos(jixuwin, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                        SetForegroundWindow(jixuwin);
                        OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                        OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                        CommonFunc.CommonSleep("SearchWipNum", sl.Moment);
                    }
                    GlobalJuBingCtrlidDict.Clear();
                    GetClick(bighwnd);
                }
            }
        }
    }
}