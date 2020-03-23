using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using static tengchao.CallWin32Api;

namespace tengchao
{
    class PublicDefine
    {
        public static Version ver = System.Environment.OSVersion.Version;//检测系统
        public static PlatformID platform = Environment.OSVersion.Platform;

        // dms路径和公安网路径
        public static string DmsPath = "";
        public static string DmsNme = "";
        public static string GawPath = "";
        public static string DmsUserName = "";
        public static string DmsPassWord = "";
        public static string DmsIp = "";
        public static string GawUaerName = "";
        public static string GawPassword = "";
        // dms刚进去后读取操作员名称
        public static string GlobalOperateUse = "";
        // log函数
        public static ILog logg = LogManager.GetLogger("穿山甲");
        System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();

        // 窗体参数
        public const int BM_CLICK = 0xF5;//点击
        public const int WM_CLOSE = 0x10;//关闭
        public const int WM_KEYDOWN = 0x0100;//普通按键按下
        public const int WM_GETTEXT = 0x000D;//获取txt
        public const int WM_GETTEXTLENGTH = 0x000E;//获取txt长度
        public const int WM_SETTEXT = 0x000C;
        public const int WM_COPY = 0x0301;
        public const int WM_COPYDATA = 0x004A;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int SC_MAXIMIZE = 0xf030;
        public const int SC_MINIMIZE = 0xf020;
        public const int SC_RESTORE = 0xF120;

        // 按键参数
        public const byte vbKeyControl = 0x11;
        public const byte vbKeyA = 65;
        public const byte vbKeyC = 67;

        // 屏幕参数
        public static Rectangle GlobalRects = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        public static int GlobalWindowH = GlobalRects.Height;
        public static int GlobalWindowW = GlobalRects.Width;

        // 初始
        public string verifysuccessinfo = "";
        public string verifyfailureinfo = "";
        public string zhuaqujiankonginfo = "";
        public int Sign = 0;
        public const uint SWP_NOMOVE = 0x0002; //不调整窗体位置
        public const uint SWP_NOSIZE = 0x0001; //不调整窗体大小
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        
        // 鼠标参数
        public static int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        public static int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        public static int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        public static int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标 
        public static int MOUSEEVENTF_WHEEL = 0x800;//控制鼠标滚轮
        // combobox参数
        public const int CB_SELECTSTRING = 0x014D;
        public const int CB_SHOWDROPDOWN = 0x014F;
        public const int VK_RETURN = 0x0D;
        public const int CB_GETDROPPEDSTATE = 0x0157;

        // keybd_event参数
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int WM_CHAR = 0x0102;
        public const int WM_KEYUP = 0x101;
        
        // 鼠标参数
        public enum MouseEventFlags
        {
            Move = 0x0001, //移动鼠标
            LeftDown = 0x0002,//模拟鼠标左键按下
            LeftUp = 0x0004,//模拟鼠标左键抬起
            RightDown = 0x0008,//鼠标右键按下
            RightUp = 0x0010,//鼠标右键抬起
            MiddleDown = 0x0020,//鼠标中键按下 
            MiddleUp = 0x0040,//中键抬起
            Wheel = 0x0800,
            Absolute = 0x8000//标示是否采用绝对坐标
        }

        // 遍历窗体控件时标记页面的参数
        public static int GlobalTag = ConstTagNoop;//当前Tag值
        public const int ConstTagNoop = 0;//无操作Tag值
        public const int ConstWipTag = 1;//点击wip工单的标记
        public const int ConstDiPanTag = 2;//获取底盘号的标记
        public const int ConstCheLiangTag = 3;//点击车辆按钮的标记
        public const int ConstCrmTag = 4;//获取信息一的标记
        public const int ConstCrmTag1 = 5;//点击信息二的标记
        public const int ConstCrmTag2 = 6;//获取信息二的标记
        public const int ConstCrmTag3 = 7; //获取信息三的标记
        public const int ConstXgclTag = 8;//点击相关车辆的标记
        public const int ConstCloseCrmTag = 9;//关闭crm页面的标记
        public const int ConstCloseKeHuTag = 10;//关闭客户及车辆页面的标记
        public const int ConstConcatTag = 11;//获取用户信息的标记
        public const int ConstDengJiTag = 12;//点击登记的标记
        public const int ConstXiuLiTag = 13;//获取修理项目信息的标记
        public const int ConstXieRuTag = 14; //不是老用户时写入信息的标记
        public const int ConstXieRuTag1 = 15;//是老用户时写入信息的标记
        public const int ConstOldUserTag = 16;//判断是不是老用户的标记
        public const int ConstOldUserInfoTag = 17;//判断老用户时获取信息的标记
        public const int ConstPanDuanTag = 18;//判断信息是否已经录入的标记
        public const int ConstFindInfoTag = 19;//判断信息是否录入时获取数据的标记
        public const int ConstQuCheRenTag = 20; //判断取车人的标记
        public const int ConstQuCheTimeTag = 21;//获取取车时间的标记
        public const int ConstGawTag = 22;//重启公安网时传值的标记
        public const int ConstDmsTag = 23;//重启dms时传值的标记
        public const int ConstPanDuanChePaiTag = 24;//判断车牌前缀的的标记
        public const int ConstChuChangTag = 25;//获取出厂时间
        public const int ConstJieZhangTag = 26;//获取结账状态
        public const int ConstSendDataTag = 27;//传值 
        public const int ConstClickDateTag = 28;//点击日期
        public const int ConstCreateDateTag = 29;//双击创建日期
        public const int ConstWipLockTag = 30;//出现wip锁定时，点击确定
        public const int ConstPicTag = 31;//图片
        public const int ConstGlobalSettleTag = 32;//点击结账列表
        public const int ConstGlobalDateTag = 33;//点击结账列表
        public const int ConstGlobalMeiBanDiPanTag = 34;//点击结账列表
        public const int ConstGlobalCopyTag = 34;//点击结账列表
        public const int ConstGlobalOrderDateTag = 35;//点击结账列表
        public const int ConstGlobalXinjianTag = 36;//点击新建
        public const int ConstGlobalDaKaiTag = 37;//点击打开
        public const int ConstGlobalTuiChuBaoBiaoTag = 38;//点击退出报表
        public const int ConstGlobalCopyWeiXiuTag = 39;//点击复制维修
        public const int ConstGlobalSuoDIngWipTag = 40;//点击锁定
        public const int ConstGlobalWeiXiuXiamgMuTag = 41;//获取维修项目框体








        // 需要抓取的信息和判断标记
        public static string GlobalIsNew = "true";
        public static string GlobalKehu = ""; // 客户姓名（XXX先生）
        public static string GlobalXingMing = ""; // 客户姓名（XXX）
        public static string GlobalXingBie = ""; // 客户性别
        public static string GlobalShouIiHaoMa1 = "";// 抓取到的手机号码
        public static string GlobalShouIiHaoMa = ""; // 匹配出来的手机号码
        public static string GlobalZhengJianLeiXing = "身份证";// 证件类型
        public static string GlobalZhengJianHaoMa = "";// 证件号码
        public static string GlobalHuJiDiZhi = "";// 户籍地址
        public static string GlobalCheLiangLeiXing = "轿车";// 车辆类型
        public static string GlobalCheShenYanSe1 = "";// 抓取到的车身颜色
        public static string GlobalCheShenYanSe = "";// 匹配出来的车身颜色
        public static string GlobalHaoPaiZhongLei = "小型汽车（蓝底白字）";// 号牌种类
        public static string GlobalChePaiHao = "";// 车牌号
        public static string GlobalChePaiHao1 = "";// 直接抓取的车牌号
        public static string GlobalCheJiaHao = "";// 车架号（底盘号）
        public static string GlobalGuoChanCheJiaHao = "";// 车架号（底盘号）
        public static string GlobalZhongWenPinPai1 = "";// 抓取到的中文品牌
        public static string GlobalZhongWenPinPai = "奔驰";// 匹配出来的中文品牌
        public static string GlobalCheLiangXingHao1 = "";// 抓取到的车辆型号
        public static string GlobalCheLiangXingHao = "";// 匹配出来的车辆型号
        public static string GlobalCheZhu = "";// 车主
        public static string GlobalXiuLiXiangMu = "";// 修理项目
        public static string GlobalDengJiShiJian = "";// 登记时间
        public static string GlobalChePaiQianZhui = "";// 车牌前缀
        public static string GlobalChePaiHouZhui = "";// 车牌后缀
        public static string GlobalQuCheTimeYear = "";// 取车时间-年
        public static string GlobalQuCheTimeMonth = "";// 取车时间-月
        public static string GlobalQuCheTimeDay = "";// 取车时间-日
        public static string GlobalQuCheTimeHour = "";// 取车时间-时
        public static string GlobalQuCheTimeMinute = "";// 取车时间-分
        public static string GlobalSongXiuTimeYear = "";// 送修时间-年
        public static string GlobalSongXiuTimeMonth = "";// 送修时间-月
        public static string GlobalSongXiuTimeDay = "";// 送修时间-日
        public static string GlobalSongXiuTimeHour = "";// 送修时间-时
        public static string GlobalSongXiuTimeMinute = "";// 送修时间-分
        public static string GlobalXiuLiRenYuan = "";// 修理人员
        public static string GlobalXiuLiPersonDaiHao = "";// 修理人员代号
        public static string GlobalXiuLiContent = "";// 修理项目
        public static string GlobalGongSiName = "";// 公司名
        public static string GlobalOrderState = "";// 订单状态
        public static string GlobalWipNum = "0";// 现在抓取的wip号
        public static string GlobalLastWip = "0";// 上一次抓取的wip号
        public static string GlobalWipYuQi = "0";// 预期抓取的wip号
        public static string GlobalZebraDataType = ""; // 客户名字或者PDI或者一般客户之类，用此区分工单
        public static string GlobalIsFull = ""; // 数据抓取字段是否完整
        public static int GlobalShiFouQuChe; // 工单是否取车
        public static string GlobalWeiXiuPanDuan;  // 维修判断，针对点开公安网的wip工单中的数据判断
        public static string GlobalQuChePanDuan;  // 判断点开公安网的wip工单是否已点取车
        public static string GlobalSongXiuYuePanDuan;  // 送修时间中月份的判断
        public static string GlobalSongXiuDayPanDuan;  // 送修时间中天数的判断
        public static string GlobalSongXiuHourPanDuan;  // 送修时间中小时的判断
        public static string GlobalSongXiuMinutePanDuan;  // 送修时间中分钟的判断

        // 公共参数
        
        public static MySqlConnectionPool GlobalConnectMysql = null;
        public static int GlobalHistoryDataCount = 0; // sqlite_password
        public static byte[] SqlitePassword = new byte[20]; // sqlite_password
        public static bool GlobalIsAllRapair = false; // 执行历史工单开始的时间，二十四小时制
        public static int ExecHistoryWipTime = 18; // 执行历史工单开始的时间，二十四小时制
        public static int GlobalNeedDataDay = 30; // 需要保留的数据天数
        public static string GlobalTenDayBefore; // 当前时间十天之前
        public static List<string> GlobalAllInfoList = new List<string>();//创建了一个空列表
        public static List<string> GlobalTextBoxList = new List<string>();
        public static List<string> GlobalComboboxList = new List<string>();
        public static int GlobalTextBoxTag = 0;
        public static int GlobalComboBoxTag = 0;
        public static int GlobalEditNum = 0;
        public static int GlobalYiWeiXiu = 0;
        public static int GlobalYiQuChe = 0;
        public static int GlobalPanDuanBiaoZhi = 1;
        public static int GlobalDoubleClick = 1;
        public static int GlobalCanWeiXiu = 1;
        public static int GlobalCanQuChe = 1;
        public static int GlobalSameUserTag = 1;
        public static List<string> GlobalAllTxt = new List<string>(); // 保存DMS路径和公安网路径数组
        private static bool globalUploadSuccess;
        public static int GlobalWipAdd = 1;
        public const int ConstClickListLocationInitValue = 26;
        public static int GlobalClickListLocation = ConstClickListLocationInitValue;//点击工单列表的位置top
        public static int GlobalWipListRows = 12;//列表行数
        public static int GlobalWipListCaluValue = 5;//计算行距时需要用到的计算值，适用于加减的情况
        public static int GlobalKeyDown = 0;  // key_down次数
        public static string GlobalGetChuChangTimes = "";
        public static string GlobalGetJinChangTimes = "";
        public static int GlobalXiuLiCount = 0;
        public static int GlobalChuChangCount = 0;
        public static string GlobalFirstWeiXiuRenYuan;
        public static string GlobalFirstRepairItem;
        public static string GlobalPicapth;
        public static string GlobalAllInfo;  // 所有信息集合字段
        public static int GlobalCounts = 0;
        public static int GlobalTagi = 0;
        public static DateTime _LoopTime = new DateTime();
        public static List<int> GlobalWipList = new List<int>(); // 抓取wip列表
        public static List<int> GlobalQueShiWipList = new List<int>(); // 抓取wip列表
        public static List<int> GlobalHistoryColorWiplist = new List<int>(); // 历史工单颜色发生变化的wip列表
        public static List<int> GlobalChuChangWipList = new List<int>(); // 出厂工单wip列表

        IntPtr jubing = FindWindow(null, null);
        System.Timers.Timer Timers_Timer = new System.Timers.Timer();
        public string ZebraContent = "报错内容";
        public ReaderWriterLock _rwlock = new ReaderWriterLock();

        // 休眠时间参数
        public class systemsleep
        {
            public int Little = 300;
            public int Moment = 1000;
            public int Longe = 2000;
            public int HhreeSecond = 3000;
            public int Longer = 4000;
            public int BigLonger = 8000;
        }

        // 判断布尔值
        public static bool GlobalSystemStop=false;  // 系统休眠判断lse;  // 系统休眠判断
        //public static bool GlobalSystemStop { get => globalSystemStop; set => globalSystemStop = false; }

        public static bool GlobalUploadSuccess { get => globalUploadSuccess; set => globalUploadSuccess = false; }
        public static bool GlobalIsContinue { get; set; }

        private static bool globalIsOldUser;
        public static bool GlobalIsOldUser { get => globalIsOldUser; set => globalIsOldUser = false; }

        public static bool GlobalIsZhuaQuSucc { get; set; }

        public bool _istrue;
        public bool IsTrue { get => _istrue; set => _istrue = true; }

        public static int global_liuchegtag = 0; // 流程标记

        // 线程传值
        public static class DefineVar
        {
            private static object lockme = new object();
            // 步骤tag
            private static int zebraint = 0;
            // for循环tag
            private static int tagis = 0;
            public static int ZebraInt
            {
                get
                {
                    return zebraint;
                }
                set
                {
                    lock (lockme)
                    {
                        zebraint = value;
                    }
                }
            }
            public static int Tagi
            {
                get
                {
                    return tagis;
                }
                set
                {
                    lock (lockme)
                    {
                        tagis = value;
                    }
                }
            }
        }
        
        // 控件类
        public class kongjian
        {
            //public int controlid { get; set; }
            public IntPtr hwnd { get; set; }
            public int top { get; set; }
            public int left { get; set; }
        }

        // 记录控件id和句柄的字典
        public static List<string> GlobalDataList = new List<string>() { };
        public static Dictionary<int, int> GlobalJuBingCtrlidDict = new Dictionary<int, int>();
        public static Dictionary<int, int> GlobalJuBing1CtrlidDict = new Dictionary<int, int>();
        public static Dictionary<int, int> GlobalJuBing2CtrlidDict = new Dictionary<int, int>();
        public static Dictionary<int, int> GlobalJuBing3CtrlidDict = new Dictionary<int, int>();
        public static Dictionary<int, int> GlobalJuBing4CtrlidDict = new Dictionary<int, int>();
        public static Dictionary<int, int> GlobalJuBing5CtrlidDict = new Dictionary<int, int>();
    }
}
