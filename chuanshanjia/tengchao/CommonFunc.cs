using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.PublicDefine;

namespace tengchao
{
    class CommonFunc
    {
        // 实例化休眠时间类
        static systemsleep sl = new systemsleep();
        /// <summary>
        /// 初始化全局变量
        /// </summary>
        public static void InitGlobalVars()
        {
            logg.Info("清空各个变量");
            if (!GlobalWipNum.Equals("0") ) {
                int global_last_wip;
                int.TryParse(GlobalWipNum, out global_last_wip);
                int end_data = global_last_wip - 1;
                GlobalWipYuQi = end_data.ToString();
            }
            GlobalOrderState = "";
            GlobalShiFouQuChe = 1;
            GlobalGongSiName = "";
            GlobalXingMing = "";
            GlobalKehu = "";
            GlobalShouIiHaoMa1 = "";
            GlobalShouIiHaoMa = "";
            GlobalZhengJianHaoMa = "";
            GlobalHuJiDiZhi = "";
            GlobalCheShenYanSe1 = "";
            GlobalCheShenYanSe = "";
            GlobalChePaiHao = "";
            GlobalChePaiHao1 = "";
            GlobalCheJiaHao = "";
            GlobalGuoChanCheJiaHao = "";
            GlobalCheLiangXingHao1 = "";
            GlobalCheLiangXingHao = "";
            GlobalXiuLiPersonDaiHao = "";
            GlobalXiuLiRenYuan = "";
            GlobalXiuLiContent = "";
            GlobalDengJiShiJian = "";
            GlobalChePaiQianZhui = "";
            GlobalChePaiHouZhui = "";
            GlobalQuCheTimeYear = "";
            GlobalSongXiuTimeMonth = "";
            GlobalSongXiuTimeDay = "";
            GlobalQuCheTimeHour = "";
            GlobalQuCheTimeMinute = "";
            GlobalSongXiuTimeYear = "";
            GlobalSongXiuTimeMonth = "";
            GlobalSongXiuTimeDay = "";
            GlobalQuCheTimeHour = "";
            GlobalSongXiuTimeMinute = "";
            GlobalXingBie = "";
            GlobalZhengJianLeiXing = "身份证";
            GlobalZhongWenPinPai1 = "";
            GlobalZhongWenPinPai = "奔驰";
            GlobalHaoPaiZhongLei = "小型汽车（蓝底白字）";
            GlobalCheLiangLeiXing = "轿车";
            GlobalWipNum = "0";
            GlobalZebraDataType = "";
            GlobalIsFull = "false";
            GlobalJuBingCtrlidDict.Clear();
            GlobalJuBing1CtrlidDict.Clear();
            GlobalJuBing2CtrlidDict.Clear();
            GlobalJuBing3CtrlidDict.Clear();
            GlobalJuBing4CtrlidDict.Clear();
            GlobalJuBing5CtrlidDict.Clear();
            GlobalSongXiuYuePanDuan = "";
            GlobalSongXiuDayPanDuan = "";
            GlobalSongXiuHourPanDuan = "";
            GlobalSongXiuMinutePanDuan = "";
            GlobalWeiXiuPanDuan = "";
            GlobalCanWeiXiu = 1;
            GlobalSongXiuYuePanDuan = "";
            GlobalFirstWeiXiuRenYuan = "";
            GlobalQuChePanDuan = "";
            GlobalFirstRepairItem = "";
        }
        /// <summary>
        //获取登录句柄id
        /// </summary>
        /// <returns></returns>
        public static string GetDengLuUserId() {
            logg.Info("获取登录句柄id");
            string _DengLuUserId = "";
            IntPtr _bighwnd = FindWindow("KCMLMasterForm_32", null);
            SetWindowPos(_bighwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_bighwnd);
            GlobalEditNum = 0;
            GetmsgProcessNeedFunc.enumwindow(_bighwnd, ConstConcatTag);
            GlobalEditNum = 0;
            _DengLuUserId = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27033);//27033 27050
            GlobalJuBingCtrlidDict.Clear();
            return _DengLuUserId;
        }
        /// <summary>
        /// 点击信息二
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns></returns>
        public static bool CrmTag1(IntPtr hWnd)
        {
            logg.Info("点击信息二");
            bool _mouse_call;
            try
            {
                _mouse_call = MouseClick.AddYanZhengClickOne("", "KTabEars", hWnd, 10, 71, 2000); //71  58
                RECT rect = new RECT();
                GetWindowRect(hWnd, out rect);
                int Topclick = (rect.Top + 10) * 65535 / GlobalWindowH; // 131  711
                int Leftclick = (rect.Left + 71) * 65535 / GlobalWindowW; // 481  1216
                int top = rect.Top + 10;
                int left = rect.Left + 71;
                logg.Info(top.ToString()+"信息二"+left.ToString());
                return _mouse_call;
            }
            catch (Exception e)
            {
                logg.Info("CrmTag1" + e.ToString());
                SendBug("CrmTag1错误", "2", e.ToString(), "commonfunc", "CrmTag1");
                _mouse_call = false;
                return _mouse_call;
            }
        }
        
        /// <summary>
        ///  点击相关车辆
        /// </summary>
        /// <param name="hWnd"></param>
        public static void XgclTag(IntPtr hWnd)
        {
            logg.Info("点击相关车辆");
            try
            {
                MouseClick.AddYanZhengClickOne("", "KTabEars", hWnd, 10, 335, 1000);//335 264
            }
            catch (Exception e)
            {
                logg.Debug("XgclTag点击相关车辆时报错了，报错信息是：" + e.ToString());
                SendBug("XgclTag点击相关车辆时报错了", "2", e.ToString(), "commonfunc", "XgclTag");
            }
        }
        /*根据用户名判断是否是合理的记录，包含先生，女士，公司，小姐，阁下任一一个都可以*/
        /// <summary>
        /// 判断用户合理性
        /// </summary>
        /// <param name="name">传入用户名</param>
        /// <returns></returns>
        public static bool IsValidRecordName(string name)
        {
            logg.Info("判断用户名是否合理");
            string[] NameArrayA = { "先生", "女士", "公司", "小姐", "阁下"};
            int LengthOfNA = NameArrayA.Length;
            for(int i=0;i<LengthOfNA;i++)
            {
                if(name.Contains(NameArrayA[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 根据句柄获取txt
        /// </summary>
        /// <param name="dictcj">句柄和类的字典</param>
        /// <param name="controlid">id</param>
        /// <returns></returns>
        /*根据id和句柄的对应关系找到相应id的句柄，然后根据句柄获取text*/
        public static string GettextFromCtrlidAndJubing(Dictionary<int, int> dictcj, int controlid)
        {
            string _NeedText = "";
            int buffer_size = 10240;
            StringBuilder buffer = new StringBuilder(buffer_size);
            IntPtr _ControlHwnd = IntPtr.Zero;
            if (dictcj.ContainsKey(controlid))
            {
                _ControlHwnd = (IntPtr)dictcj[controlid];
                if (_ControlHwnd != IntPtr.Zero)
                {
                    SendMessage(_ControlHwnd, WM_GETTEXT, buffer_size, buffer);
                    _NeedText = buffer.ToString();
                    logg.Info("chuandi--huoquzhi:" + buffer.ToString());
                }
            }
            else
            {
                logg.Info("没有此id:" + controlid.ToString() +"的句柄");
            }
            return _NeedText;
        }
        /// <summary>
        /// 获取句柄
        /// </summary>
        /// <param name="dictcj">通过传入的字典</param>
        /// <param name="controlid">传入id</param>
        /// <returns></returns>
        /*根据id和句柄的对应关系找到相应id的句柄*/
        public static IntPtr GetIntptrFromCtrlidAndJubing(Dictionary<int, int> dictcj, int controlid)
        {
            int buffer_size = 10240;
            StringBuilder buffer = new StringBuilder(buffer_size);
            IntPtr _ControlHwnd = IntPtr.Zero;
            if (dictcj.ContainsKey(controlid))
            {
                _ControlHwnd = (IntPtr)dictcj[controlid];
            }
            else
            {
                _ControlHwnd = IntPtr.Zero;
                logg.Info("没有此id:" + controlid.ToString() + "的句柄");
            }
            return _ControlHwnd;
        }
         /// <summary>
         /// 休眠函数
         /// </summary>
         /// <param name="funcname">函数名</param>
         /// <param name="sleeptime">休眠时长</param>
        public static void CommonSleep(string funcname,int sleeptime)
        {
            logg.Info("sleeplog:\t"+GlobalKeyDown+"\t"+funcname+"\t"+ GlobalTag + "\t"+sleeptime);
            Thread.Sleep(sleeptime);
        }
        /// <summary>
        /// 程序开始时的初始化页面
        /// </summary>
        public static void BacktoChuShiPage()
        {
            IntPtr _SelfHwnd = FindWindow("WindowsForms10.Window.8.app.0.2bf8098_r16_ad1", "公安网自动录入系统");
            if (_SelfHwnd != IntPtr.Zero)
            {
                SendMessage(_SelfHwnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
            }
            IntPtr _BigPageHwnd = FindWindow("KCMLMasterForm_32", null);
            if (_BigPageHwnd != IntPtr.Zero)
            {
                IntPtr _WipListHwnd = FindWindow(null, "WIP号搜索");
                if (_WipListHwnd != IntPtr.Zero)
                {
                    SendMessage(_WipListHwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
                    SetWindowPos(_WipListHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_WipListHwnd);
                    OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                    CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                    OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                    CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
                }
                IntPtr _CrmHwnd = FindWindow(null, GlobalOperateUse + "的CRM");
                if (_CrmHwnd != IntPtr.Zero)
                {
                    SendMessage(_CrmHwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
                    SetWindowPos(_CrmHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_CrmHwnd);
                    OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                    CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                    OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                    CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
                }
                SendMessage(_BigPageHwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                SetWindowPos(_BigPageHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_BigPageHwnd);
                MouseClick.JustClickOne();
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, 0, 0);
                CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, 0, 0);
                CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, 0, 0);
                CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, 0, 0);
                CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                OpenWindGetMsg.keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                CommonSleep("BacktoChuShiPage", sl.HhreeSecond);
            }
            IntPtr _WipListHwnd1 = FindWindow(null, "WIP号搜索");
            if (_WipListHwnd1 != IntPtr.Zero)
            {
                SetWindowPos(_WipListHwnd1, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_WipListHwnd1);
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                CommonFunc.CommonSleep("BacktoChuShiPage", 10);
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                CommonSleep("BacktoChuShiPage", sl.Longe);
            }
            }
        /// <summary>
        /// 获取维修人员部分的截图，在维修人员没有匹配上时触发
        /// </summary>
        /// <param name="weixiudaihao">人员代号</param>
        /// <param name="hwnd">截图部分的句柄</param>
        public static void GetWeiXiuPic(string weixiudaihao, IntPtr hwnd) {
            if (!hwnd.Equals(IntPtr.Zero)) {
                string _PicPath = @"personname.png";
                Bitmap _BitMap = TakeScreenCapture.GetWindowCapture(hwnd, _PicPath);
                // 用完释放
                _BitMap.Dispose();
                CommonSleep("GetWeiXiuPic", 100);
                Bitmap _RenYuan = new Bitmap(_PicPath);
                string _RenYuanPath = @"img\\" + weixiudaihao + ".png";
                _RenYuan.Save(_RenYuanPath);
                _RenYuan.Dispose();
            }
        }
        /// <summary>
        /// 删除数据库中已经更新的人员代号对应的图片
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DeleteFilePng(string pngPath)
        {
            if (File.Exists(pngPath))
            {
                File.Delete(pngPath);
            }
        }
        /// <summary>
        /// 发送bug
        /// </summary>
        /// <param name="ErrorDescription"></param>
        /// <param name="BugLevel"></param>
        /// <param name="BugDetail"></param>
        /// <param name="BugFile"></param>
        /// <param name="BugFunction"></param>
        [Conditional("DEBUG")]
        private static void WrongInput(string ErrorDescription, string BugLevel, string BugDetail, string BugFile, string BugFunction)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("user", "haonan.lei@zebra-c.com");
            dic.Add("Notification", "chuanshanjia");
            dic.Add("BugAbstract", ErrorDescription);
            dic.Add("BugLevel", BugLevel);
            dic.Add("BugInfo", BugDetail);
            dic.Add("BugFile", BugFile);
            dic.Add("BugPosition", BugFunction);
            if (!BugFunction.Contains("Drawing") ||!BugFunction.Contains("GetEnterWipList")|| !BugDetail.Contains("Drawing") || !BugDetail.Contains("GetEnterWipList"))
            {//屏蔽bug的发送
               //string zebrastate = GetResponseString(CreatePostHttpResponse("http://dms.zebra-c.com/bug/create", dic));
            }
        }
        [Conditional("TRACE")]
        private static void WrongLabel(string ErrorDescription, string BugLevel, string BugDetail, string BugFile, string BugFunction)
        {
            Console.WriteLine("no change");   
        }
        // 出问题时发bug
        public static void SendBug(string ErrorDescription, string BugLevel, string BugDetail, string BugFile, string BugFunction)
        {
            /*
                1 | user| xxx@zebra-c.com|创建人邮箱|是|
                2 | Notification| test|bug通知类型|是|
                3 | BugAbstract| 500错误|bug描述|是|
                4 | BugLevel| 2|bug等级|是|
                5 | BugInfo| 跳转页面500错误|bug详细信息|是|
                6 | BugFile| UserController|出bug的脚本|是|
                7 | BugPosition| check()|脚本中出Bug的函数名|是
             */
            WrongInput(ErrorDescription,BugLevel,BugDetail,BugFile,BugFunction);
            WrongLabel(ErrorDescription, BugLevel, BugDetail, BugFile, BugFunction);
            //Dictionary<string, string> dic = new Dictionary<string, string>();
            //dic.Add("user", "haonan.lei@zebra-c.com");
            //dic.Add("Notification", "chuanshanjia");
            //dic.Add("BugAbstract", ErrorDescription);
            //dic.Add("BugLevel", BugLevel);
            //dic.Add("BugInfo", BugDetail);
            //dic.Add("BugFile", BugFile);
            //dic.Add("BugPosition", BugFunction);
            //string zebrastate = GetResponseString(CreatePostHttpResponse("http://dms.zebra-c.com/bug/create", dic));
        }
        // 委托查找窗体
        /// <summary>
        /// 循环查找窗体
        /// </summary>
        /// <param name="func"></param>
        public static void WaitFindWind(Func<bool> func)
        {
            //调用方法 WaitFindWind(() => YouselfFunc());
            //YouselfFunc  是你要传入的函数名
            int _zebracount = 0;
            while (_zebracount < 3)
            {
                _zebracount++;
                bool _istrue = func();
                if (_istrue == true)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 根据文件路径获取维修人员
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static Dictionary<int, List<string>> getweixiutxt(String Path)//根据文件路径获取维修得人员
        {
            Dictionary<int, List<string>> myDictionary = new Dictionary<int, List<string>>();
            StreamReader sr = new StreamReader(Path, Encoding.Default);
            string content;
            int count = 0;
            while ((content = sr.ReadLine()) != null)
            {
                count++;
                string[] sArray = content.Split('$');
                int weixiucount = OperateSql.GetWeiXiuCount(sArray[0]);
                if (weixiucount > 0)
                {
                    OperateSql.ChangeRepairMsg("1", "00" + sArray[0], sArray[1], sArray[2]);
                }
                else
                {
                    OperateSql.ChangeRepairMsg("insert", "00" + sArray[0], sArray[1], sArray[2]);
                };
                List<string> weixiulist = new List<string>(sArray);
                if (weixiulist.Count > 0)
                {
                    myDictionary.Add(count, weixiulist);
                }
            }
            return myDictionary;
        }
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// 判断网络状态
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()//判断网络状态
        {
            int I = 0;
            bool state = InternetGetConnectedState(out I, 0);
            return state;
        }
        /// <summary>
        /// 创建图片文件夹
        /// </summary>
        public static void CreateImg()
        {
            logg.Info("创建图片文件夹");
            string currPath = Application.StartupPath;

            //检查是否存在文件夹
            string subPath = currPath + "/img/";
            if (false == System.IO.Directory.Exists(subPath))
            {
                //创建pic文件夹
                System.IO.Directory.CreateDirectory(subPath);
            }
        }
        /// <summary>
        /// 查找进程
        /// </summary>
        /// <param name="pcname">进程名字</param>
        /// <returns></returns>
        public static bool ZebraSearchProcess(string pcname)
        {
            logg.Info("查找进程");
            bool pcTrue = true;
            if (System.Diagnostics.Process.GetProcessesByName(pcname).ToList().Count > 0)
            {
                //存在
                pcTrue = true;
            }
            else
            {
                //不存在
                pcTrue = false;
            }
            return pcTrue;
        }
        /// <summary>
        /// 统一的弹出框
        /// </summary>
        /// <param name="tip"></param>
        public static void tips(string tip)
        {
            MessageBox.Show(tip, "温馨提示", MessageBoxButtons.OK,
MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

    }
}
