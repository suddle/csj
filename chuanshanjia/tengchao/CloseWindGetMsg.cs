using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static tengchao.PublicDefine;
using static tengchao.CallWin32Api;
using static tengchao.GetmsgProcessNeedFunc;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace tengchao
{
    class CloseWindGetMsg
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        static systemsleep sl = new systemsleep();
        /// <summary>
        /// 获取客户信息 通过wip号搜索
        /// </summary>
        public static void GetKeHuAndOtherInfoInfo()
        {
            logg.Info("获取客户信息");
            IntPtr _WipListHwnd = FindWindow(null, "WIP号搜索");
            if (_WipListHwnd != IntPtr.Zero)
            {//如果有wip号搜索弹窗  那么关闭
                SetWindowPos(_WipListHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_WipListHwnd);
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                CommonFunc.CommonSleep("get_kehuand_other_info", 10);
                keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                CommonFunc.CommonSleep("get_kehuand_other_info", sl.Moment);
            }
            GlobalEditNum = 0;
            GlobalTag = ConstConcatTag;
            GetContacInfo();
            GlobalTag = ConstTagNoop;
            GlobalEditNum = 0;
            global_liuchegtag = 9;
            DefineVar.ZebraInt = global_liuchegtag;
            if (GlobalKehu != "")
            {
                IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);
                SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_BigHwnd);
                if (_BigHwnd != IntPtr.Zero)
                {
                    GetXiuLiXiangMuInfo(_BigHwnd);//获取修理项目信息
                }
                SetWindowPos(_BigHwnd, HWND_BOTTOM, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public static void SaveData()
        {
            if (string.IsNullOrEmpty(GlobalCheJiaHao))
            {
                GlobalCheJiaHao = GlobalGuoChanCheJiaHao;
            }
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-dd");
            string _AllCatchStr = "抓取到的信息是：GlobalGongSiName:" + GlobalGongSiName + "---GlobalXingMing:" + GlobalXingMing + "---GlobalShouIiHaoMa1:" + GlobalShouIiHaoMa1 + "---GlobalShouIiHaoMa:" + GlobalShouIiHaoMa + "---GlobalZhengJianHaoMa:" +
                            GlobalZhengJianHaoMa + "---" + GlobalHuJiDiZhi.Replace("\r\n", "") + "---GlobalCheShenYanSe1:" + GlobalCheShenYanSe1 + "---GlobalChePaiHao:" + GlobalChePaiHao + "---GlobalCheJiaHao:" +
                            GlobalCheJiaHao + "---GlobalCheLiangXingHao1:" + GlobalCheLiangXingHao1 + "---GlobalChePaiQianZhui:" + GlobalChePaiQianZhui + "---GlobalChePaiHouZhui:" + GlobalChePaiHouZhui + "---GlobalSongXiuTimeYear:" +
                            GlobalSongXiuTimeYear + "---GlobalSongXiuTimeMonth:" + GlobalSongXiuTimeMonth + "---GlobalSongXiuTimeDay:" + GlobalSongXiuTimeDay + "---GlobalQuCheTimeHour:" + GlobalQuCheTimeHour + "---GlobalSongXiuTimeMinute:" +
                            GlobalSongXiuTimeMinute + "---GlobalXiuLiPersonDaiHao:" + GlobalXiuLiPersonDaiHao + "---GlobalXiuLiRenYuan:" + GlobalXiuLiRenYuan + "---GlobalXiuLiContent:" + GlobalXiuLiContent + "---GlobalOrderState:" + GlobalOrderState + "---GlobalShiFouQuChe:" + GlobalShiFouQuChe.ToString();
            logg.Info(_AllCatchStr);
            // 月、日、时、分 存在个位数的时候，补零
            if (GlobalSongXiuTimeMonth.Length < 2)//给日期时间添加前缀
            {
                GlobalSongXiuTimeMonth = "0" + GlobalSongXiuTimeMonth;
            }
            if (GlobalSongXiuTimeDay.Length < 2)//给日期时间添加前缀
            {
                GlobalSongXiuTimeDay = "0" + GlobalSongXiuTimeDay;
            }
            if (GlobalSongXiuTimeHour.Length < 2)//给日期时间添加前缀
            {
                GlobalSongXiuTimeHour = "0" + GlobalSongXiuTimeHour;
            }
            if (GlobalSongXiuTimeMinute.Length < 2)//给日期时间添加前缀
            {
                GlobalSongXiuTimeMinute = "0" + GlobalSongXiuTimeMinute;
            }
            if (!String.IsNullOrEmpty(GlobalWipNum) && !GlobalWipNum.Equals("0"))
            {
                if (GlobalChePaiHao1.StartsWith("N1/"))
                {
                    logg.Info("此条数据是没有车牌的，不上传(新车)");
                    GlobalIsFull = "true";
                }
                else if (!String.IsNullOrEmpty(GlobalShouIiHaoMa) && !String.IsNullOrEmpty(GlobalZhengJianHaoMa) &&
                    !String.IsNullOrEmpty(GlobalHuJiDiZhi) && !String.IsNullOrEmpty(GlobalCheShenYanSe) &&
                    !String.IsNullOrEmpty(GlobalChePaiHao) && !String.IsNullOrEmpty(GlobalCheJiaHao) &&
                    !String.IsNullOrEmpty(GlobalCheLiangXingHao) && !String.IsNullOrEmpty(GlobalSongXiuTimeYear.Trim()) &&
                    !String.IsNullOrEmpty(GlobalSongXiuTimeMonth) && !String.IsNullOrEmpty(GlobalSongXiuTimeDay) &&
                    !String.IsNullOrEmpty(GlobalSongXiuTimeHour) && !String.IsNullOrEmpty(GlobalSongXiuTimeMinute) &&
                    !String.IsNullOrEmpty(GlobalXiuLiRenYuan) && !String.IsNullOrEmpty(GlobalXiuLiContent) &&
                    !String.IsNullOrEmpty(GlobalZebraDataType) && !String.IsNullOrEmpty(GlobalWipNum) &&
                    (GlobalShiFouQuChe == 0 || GlobalShiFouQuChe == 1) && !String.IsNullOrEmpty(GlobalXingMing)
                    && !String.IsNullOrEmpty(GlobalXingBie) && !String.IsNullOrEmpty(GlobalZhongWenPinPai)&&!GlobalSongXiuTimeMonth.Equals("0")&&GlobalSongXiuTimeYear.Trim().Length!=0)
                {
                    GlobalIsFull = "true";
                    try
                    {
                        int wiplists = 0;
                        int.TryParse(GlobalWipNum, out wiplists);
                        GlobalQueShiWipList.Remove(wiplists);
                    }
                    catch
                    {
                        Console.WriteLine("删除了已经");
                    }
                }
                else if (!CommonFunc.IsValidRecordName(GlobalZebraDataType))
                {
                    GlobalIsFull = "true";
                    try
                    {
                        int wiplists = 0;
                        int.TryParse(GlobalWipNum, out wiplists);
                        GlobalQueShiWipList.Remove(wiplists);
                    }
                    catch {
                        Console.WriteLine("删除了已经");
                    }
                }
                else
                {
                    GlobalIsFull = "false";
                    //缺失列表赋值
                    int wiplists=0;
                    int.TryParse(GlobalWipNum, out wiplists);
                    logg.Info(GlobalWipList.Count.ToString()+"全局列表的长度");
                    GlobalQueShiWipList.Add(wiplists);
                }
                int _st = ZebraSql.GetWipRecordStatus(GlobalWipNum);
                logg.Info("st是" + _st.ToString());
                if (!CheckIdCard18(GlobalZhengJianHaoMa)) {
                    GlobalIsFull = "false";
                }
                if (_st == 1)
                {//可插入
                    OperateSql.InsertRecordData(_st);
                }
                else if (_st == 2)
                {//可更新
                    OperateSql.InsertRecordData(_st);

                    int oldcount=OperateSql.GetOldWipCount(GlobalWipNum);
                    if (oldcount == 0)
                    {
                        OperateSql.SaveOldWipnum(GlobalWipNum, GlobalIsNew);
                    }
                    else {
                        OperateSql.UpdateOldWipnum(GlobalWipNum);
                    }
                }
                else if (_st == 3)
                {//不可操作，说明记录已经插入且完整
                    OperateSql.DeleteOldWipnum(GlobalWipNum);
                }
            }
            else
            {
                logg.Info("保存数据时没有获取到wip号");
            }
            string _time1 = DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss");
        }
        /// <summary>
        /// 关闭crm页面
        /// </summary>
        /// <returns></returns>
        public static bool CloseCrm()
        {//关闭crm页面
            logg.Info("关闭crm页面");
            bool _istrue = false;
            if (!GlobalSystemStop)
            {
                _istrue = true;
                CommonFunc.CommonSleep("GetCheLiangInfo", sl.Longe);
                IntPtr _MaindHwnd2 = FindWindow(null, GlobalOperateUse + "的CRM");
                SetWindowPos(_MaindHwnd2, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                SetForegroundWindow(_MaindHwnd2);
                if (_MaindHwnd2 != IntPtr.Zero)
                {
                    CommonFunc.CommonSleep("GetCheLiangInfo", sl.Little);
                    OpenWindGetMsg.ClickCheLaingMsg(_MaindHwnd2);//--------------------------点击车辆  获取车辆信息
                    //关闭crm页面
                    CommonFunc.CommonSleep("GetCheLiangInfo", sl.Moment);
                    keybd_event(Keys.Escape, 0, 0, 0);
                    CommonFunc.CommonSleep("GetCheLiangInfo", 10);
                    keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                    logg.Info("关闭crm页面完成");
                }
                else
                {
                    CommonFunc.CommonSleep("GetCheLiangInfo", 500);
                    _istrue = false;
                }
            }
            return _istrue;
        }
            /// <summary>
            /// 获取车辆信息，并且退出页面，回到dms最大的页面，完成后调用上传数据接口
            /// </summary>
        public static void GetCheLiangInfo()
        {
            try
            {
                logg.Info("开始获取车辆信息");
                CommonFunc.WaitFindWind(() => CloseCrm());//-----------------获取车辆信息完成 关闭车辆信息页面
            }
            catch (Exception e)
            {
                logg.Error("GetCheLiangInfo获取信息或关闭crm页面出错，信息是：" + e.ToString());
                CommonFunc.SendBug("GetCheLiangInfo获取信息或关闭crm页面出错", "2", e.ToString(), "get_msg1", "GetCheLiangInfo");
            }
            try
            {
                if (!GlobalSystemStop)
                {
                    ClosePageByEsc();//多次esc回退  以防会有弹窗没退出
                    CommonFunc.CommonSleep("GetCheLiangInfo", sl.Moment);
                    CloseTiXing();//-----------------------出现业务提醒页面并且关闭，然后获取用户信息
                }
            }
            catch (Exception e)
            {
                logg.Debug("关闭客户页面或获取用户信息时出错，信息是：" + e.ToString());
                logg.Info("get_wip_tosql.insert into data");
                CommonFunc.SendBug("关闭客户页面或获取用户信息时出错", "2", e.ToString(), "get_msg1", "GetCheLiangInfo");
            }
        }
        /// <summary>
        /// 关闭页面事件
        /// </summary>
        public static void ClosePageByEsc()
        {
            logg.Info("关闭页面");
            int esc_and_enter_num = 0;
            while (esc_and_enter_num < 7)
            {
                keybd_event(Keys.Escape, 0, 0, 0);
                CommonFunc.CommonSleep("close_page_by_esc", 10);
                keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                CommonFunc.CommonSleep("close_page_by_esc", 1500);
                esc_and_enter_num++;
            }
        }
        /// <summary>
        /// 出现业务提醒页面并且关闭，然后获取用户信息
        /// </summary>
        public static bool CloseTiXing()
        {
            logg.Info("关闭提醒弹框");
            bool finish_CloseTiXing = true;
            GlobalEditNum = 0;
            try
            {
                if (!GlobalSystemStop)
                {
                    CommonFunc.CommonSleep("CloseTiXing", sl.Longe);
                    IntPtr _MaindHwnd5 = FindWindow(null, "业务提醒");
                    SetWindowPos(_MaindHwnd5, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_MaindHwnd5);
                    if (_MaindHwnd5 != IntPtr.Zero)
                    {
                        keybd_event(Keys.Enter, 0, 0, 0);
                        CommonFunc.CommonSleep("CloseTiXing", 10);
                        keybd_event(Keys.Enter, 0, KEYEVENTF_KEYUP, 0);
                        CommonFunc.CommonSleep("CloseTiXing", sl.Moment);
                        logg.Info("出现了提醒页面，点击了关闭");
                        logg.Info("开始获取用户信息");
                        finish_CloseTiXing = true;
                    }
                }
            }
            catch (Exception e)
            {
                finish_CloseTiXing = false;
                logg.Error("关闭提醒时报错，信息是：" + e.ToString());
                CommonFunc.SendBug("关闭提醒时报错", "2", e.ToString(), "get_msg1", "CloseTiXing");
            }
            return finish_CloseTiXing;
        }
        /// <summary>
        /// 读取维修项目内容
        /// </summary>
        public static void ReadWeiXiu()
        {
            try
            {
                ClipboardAsync Clipboard2 = new ClipboardAsync();
                if (Clipboard2.ContainsText(TextDataFormat.Text))
                {
                    string txts = Clipboard2.GetText(TextDataFormat.Text);
                    string[] sArray = txts.Split(new char[2] { '\r', '\n' });
                    foreach (var txt in sArray)
                    {
                        if (txt.Length > 0)
                        {
                            string[] aArray = txt.Split('	');
                            if (aArray[2].Length > 0)
                            {
                                if (HistoryWip.IsNumeric(aArray[2]))
                                {
                                    GlobalXiuLiContent = aArray[3];
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ReadToMysql为空");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadToMysql为空");
            }
        }
        /// <summary>
        /// 获取修理项目，点击列表，获取到文本后点击取消
        /// </summary>
        /// <param name="maindHwnd7"></param>
        public static void GetXiuLiXiangMuInfo(IntPtr maindHwnd7)
        {

            logg.Debug("开始获取维修项目信息");
            int _tops = 388;//定义 维修项目框距离整个窗体得距离
            if (maindHwnd7 != IntPtr.Zero)
            {
                if (!GlobalSystemStop)
                {
                    enumwindow(maindHwnd7, ConstGlobalWeiXiuXiamgMuTag);
                    //CommonFunc.CommonSleep("GetXiuLiXiangMuInfo", 500);
                    //OpenWindGetMsg.CilikRight("GetXiuLiXiangMuInfo", maindHwnd7, _tops-10, 24);
                    //ReadWeiXiu();
                    //CommonFunc.CommonSleep("GetXiuLiXiangMuInfo", 300);
                    if (string.IsNullOrEmpty(GlobalXiuLiContent)|| GlobalXiuLiContent.Length<2)
                    {
                        GlobalXiuLiContent = "维修";
                    }
                }
            }
        }
        /// <summary>
        /// 获取结账状态，点击账单后获取进场时间
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static bool ChuChangTag(IntPtr hWnd)
        {
            bool mouse_call;
            try  //KCMLDBEdit_32  //KCMLDBEdit_32
            {
                enumwindow(hWnd, ConstGlobalOrderDateTag);//双击创建日期
                logg.Info("获取进场时间信息");
                GlobalTag = ConstChuChangTag;
                enumwindow(hWnd, ConstChuChangTag);
                GlobalTag = ConstTagNoop;
                GlobalEditNum = 0;
                logg.Info("进场时间信息获取完成");
                return true;
            }
            catch (Exception e)
            {
                logg.Debug("点击订单获取出场时间出错了，报错信息是：" + e.ToString() + "进入回退函数。。。");
                CommonFunc.SendBug("点击订单获取出场时间出错了", "2", e.ToString(), "get_msg1", "ChuChangTag");
                mouse_call = false;
                return mouse_call;
            }
        }
        /// <summary>
        /// 获取客户信息，成功后进入获取修理项目信息，失败后回退
        /// </summary>
        public static void GetUserMessage(IntPtr _BigHwnd)
        {
            GlobalEditNum = 0;
            enumwindow(_BigHwnd, ConstConcatTag);
            logg.Info("获取用户信息完成");
            GlobalShouIiHaoMa1 = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26688);
            GlobalHuJiDiZhi = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26687);
            GlobalZhengJianHaoMa = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26690);
            GlobalOrderState = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27225);
            GlobalZebraDataType = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26686);
            GlobalWipNum = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 26684);
            GlobalXiuLiPersonDaiHao = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27050);
            string songxiudate = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27010);
            string songxiutimes = CommonFunc.GettextFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27011);
            logg.Info(songxiudate + " "+ songxiutimes+"送修时间打印");
            logg.Info("GlobalXiuLiPersonDaiHao:" + GlobalXiuLiPersonDaiHao);
            if (GlobalOrderState == "X")
            {
                GlobalShiFouQuChe = 0;
            }
            else
            {
                GlobalShiFouQuChe = 1;
            }
            Match _match8 = Regex.Match(GlobalShouIiHaoMa1, @"1\d{10}");
            if (_match8.Success)
            {
                GlobalShouIiHaoMa = _match8.Value;
            }
            Match _match11 = Regex.Match(GlobalZhengJianHaoMa, @"^[1-9][0-7]\d{4}((19\d{2}(0[13-9]|1[012])(0[1-9]|[12]\d|30))|(19\d{2}(0[13578]|1[02])31)|(19\d{2}02(0[1-9]|1\d|2[0-8]))|(19([13579][26]|[2468][048]|0[48])0229))\d{3}(\d|X|x)?$");
            if (_match11.Success)
            {
                GlobalZhengJianLeiXing = "身份证";
            }
            else
            {
                GlobalZhengJianLeiXing = "其他证件";
            }
            logg.Info("抓取到的手机号码：" + GlobalShouIiHaoMa1 + "////" + "匹配到的手机号码：" + GlobalShouIiHaoMa + "///" + "抓取到的订单状态：" + GlobalOrderState.ToString() + "///" + "匹配到的订单状态：" + GlobalShiFouQuChe.ToString() + "///" + "抓取到的户籍地址：" + GlobalHuJiDiZhi.Replace("\r\n", "") + "///" + "抓取到的证件号码：" + GlobalZhengJianHaoMa);
            // 修理人员名字匹配修改为从数据库取值
            GlobalXiuLiRenYuan = OperateSql.GetXiuLiPersonName(GlobalXiuLiPersonDaiHao);
            if (GlobalXiuLiRenYuan.Equals(string.Empty))
            {
                CommonFunc.GetWeiXiuPic(GlobalXiuLiPersonDaiHao, CommonFunc.GetIntptrFromCtrlidAndJubing(GlobalJuBingCtrlidDict, 27033));
            }
            if (GlobalKehu != "")
            {
                CommonFunc.CommonSleep("GetKeHuAndOtherInfoInfo", sl.Little);
                bool mouse_call1;
                mouse_call1 = ChuChangTag(_BigHwnd);
            }
        }
        /// <summary>
        /// 检查身份证信息
        /// </summary>
        /// <param name="idNumber">传入字符串</param>
        /// <returns></returns>
        public static bool CheckIdCard18(string idNumber)
        {
            if ((!Regex.IsMatch(idNumber, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase)))
            {
                return false;
            }
            else
            {
                logg.Info("身份信息正确" + GlobalWipNum);
                return true;
            }
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static bool TryGetUserInfo()//获取用户信息
        {
            bool _istrue = false;
            CommonFunc.CommonSleep("GetKeHuAndOtherInfoInfo", sl.Little);
            IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);
            SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_BigHwnd);
            if (_BigHwnd != IntPtr.Zero)
            {
                _istrue = true;
                GetUserMessage(_BigHwnd);//获取客户信息，成功后进入获取修理项目信息，失败后回退
            }
            else
            {
                CommonFunc.CommonSleep("GetContacInfo", 500);
                _istrue = false;
            }
            return _istrue;
        }
        public static void GetContacInfo()
        {
            CommonFunc.WaitFindWind(() => TryGetUserInfo());
        }
    }
}