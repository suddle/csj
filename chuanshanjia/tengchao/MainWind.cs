using CCWin;
using MySqlConnectionZebra;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static tengchao.CallWin32Api;
using static tengchao.FreePermission;
using static tengchao.PublicDefine;

namespace tengchao
{
    public partial class MainWind : CCSkinMain
    {
        public MainWind()
        {
            InitializeComponent();
            OpenWindGetMsg frm = new OpenWindGetMsg(this);
        }
        LicenseCode lices = new LicenseCode();
        private FormWindowState fwsPrevious;
        public static FrmTopMost myTopMost;
        bool Is_start = true;
        int t1_state = 0;
        private static Thread _objThread;
        private static Dictionary<int, List<string>> WeixiuDictionary = new Dictionary<int, List<string>>();
        static systemsleep sl = new systemsleep();

        // 删除img中的图片以及personname
        public static void DeletePicFromImg()
        {
            logg.Info("删除图片");
            // 删除personname.png
            CommonFunc.DeleteFilePng("./personname.png");
            // 删除img下的所有png
            string _fileDir = Environment.CurrentDirectory;//获取当前路径
            string _endPath = _fileDir + "\\" + "img";//找到img文件
            string[] files = Directory.GetFiles(_endPath, "*.png");//获取其下所有的图片
            foreach (string file in files)//遍历图片
            {
                CommonFunc.CommonSleep("DeletePicFromImg", 500);
                CommonFunc.DeleteFilePng(file);
            }
        }
        //初始化函数
        private bool ReloadFunc()
        {
            // 读取外部配置
            try
            {
                string dmspath = OperateSql.GetDmsInfo("dmspath");
                string gawpath = OperateSql.GetDmsInfo("gawpath");
                if (dmspath.Length < 1)
                {
                    CommonFunc.tips("请确认是否输入dms路径");
                }
                if (gawpath.Length < 1)
                {
                    CommonFunc.tips("请确认是否输入公安网路径");
                }
            }
            catch
            {
                CommonFunc.tips("请确认是否设置DMS或公安网路径");
                this.settip.Show();
            }
            if (DmsNme.Length == 0)
            {//判断dms和公安网名字是否设置
                CommonFunc.tips("请添加DMS用户名");
            }
            else if (DmsPassWord.Length == 0)
            {
                CommonFunc.tips("请添加DMS密码");
            }
            else if (DmsIp.Length == 0)
            {
                CommonFunc.tips("请添加DMS的IP号码");
            }
            else
            {
                Is_start = false;
                t1_state = 1;
                this.systemstat.Text = "系统状态：开始抓取";
                this.start.Text = "暂停";
                bool _isconnect = CommonFunc.IsConnected();//判断网络状态
                if (_isconnect)
                {
                    bool _exist_dms = CommonFunc.ZebraSearchProcess("kclient");
                    if (_exist_dms)
                    {//判断是否打开dms
                        bool _exist_gaw = CommonFunc.ZebraSearchProcess("jixiu");
                        CommonFunc.BacktoChuShiPage();//程序开始时 初始化
                                                      // OperateSql.CreatSql();
                        bool _zhidingweizhi = false;
                        if (_zhidingweizhi)
                        {//指定位置开始抓取
                            GlobalKeyDown = 6;
                            if (GlobalKeyDown > 11)
                            {
                                GlobalClickListLocation = ConstClickListLocationInitValue + 21 * 10;
                            }
                            else
                            {
                                GlobalClickListLocation = ConstClickListLocationInitValue + 21 * GlobalKeyDown;
                            }
                        }
                        GlobalKeyDown = 0;
                        // 登陆dms人员姓名的取值， 需要点击新建，确保抓取到的是登陆的用户
                        string _WindowTitle = "";
                        IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);
                        SetWindowPos(_BigHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                        SetForegroundWindow(_BigHwnd);
                        if (_BigHwnd != IntPtr.Zero)
                        {
                            GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalXinjianTag);

                            //StringBuilder _title = new StringBuilder(102400);
                            //GetWindowText(_BigHwnd, _title, _title.Capacity);//获取窗口标题
                            //if (_title.ToString().Length > 5)
                            //{
                            //    _WindowTitle = _title.ToString();
                            //}
                            //tengchao.MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _BigHwnd, 74, 23, 1500);
                        }
                        string _OperateUserid = CommonFunc.GetDengLuUserId();//获取句柄id
                        InfoSql.GetOperateUsename(_OperateUserid);//通过句柄id获取当前dms的用户名
                        int i = 0;
                        while (this.Visible)
                        {
                            i++;
                            _LoopTime = DateTime.Now;
                            DefineVar.Tagi = i;
                            string _NowTime = DateTime.Now.ToString("HH:mm");
                            int _NowHour;
                            int _NowMinute;
                            int.TryParse(_NowTime.Split(':')[0], out _NowHour);//获取当前的小时
                            int.TryParse(_NowTime.Split(':')[1], out _NowMinute);//获取当前的分钟
                            int _CloseTimeHour;
                            int _CloseTimeMinute;
                            int.TryParse(this.closetime.Text.Split(':')[0], out _CloseTimeHour);//获取关闭小时数
                            int.TryParse(this.closetime.Text.Split(':')[1], out _CloseTimeMinute);//获取关闭分钟数
                            if (_NowHour < _CloseTimeHour)
                            {
                                if (!GlobalSystemStop)
                                {//统计系统占用时间
                                 //bool _is_top = true;
                                    logg.Info("当前时间是：" + DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss"));
                                    System.Diagnostics.Process[] MyProcesses = Process.GetProcessesByName("tengchao");
                                    long _memorySize = MyProcesses[0].WorkingSet64 / 1024;
                                    logg.Info("我们的程序占用内存是：" + _memorySize.ToString());
                                    System.Diagnostics.Process[] MyProcesses1 = Process.GetProcessesByName("kclient");
                                    long _memorySize1 = MyProcesses1[0].WorkingSet64 / 1024;
                                    logg.Info("DMS占用内存是：" + _memorySize1.ToString());
                                    MainFunc();//开始抓取
                                }
                            }
                            else if (_NowHour == _CloseTimeHour && _NowMinute <= _CloseTimeMinute)
                            {
                                if (!GlobalSystemStop)
                                {
                                    logg.Info("当前时间是：" + DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss"));
                                    System.Diagnostics.Process[] MyProcesses = Process.GetProcessesByName("tengchao");
                                    long _memorySize = MyProcesses[0].WorkingSet64 / 1024;
                                    logg.Info("我们的程序占用内存是：" + _memorySize.ToString());
                                    System.Diagnostics.Process[] MyProcesses1 = Process.GetProcessesByName("kclient");
                                    long _memorySize1 = MyProcesses1[0].WorkingSet64 / 1024;
                                    logg.Info("DMS占用内存是：" + _memorySize1.ToString());
                                    MainFunc();//开始抓取
                                }
                            }
                            else
                            {
                                Dispose();
                                Application.Exit();
                                Application.ExitThread();
                                System.Environment.Exit(System.Environment.ExitCode);
                            }
                            List<string> _ChangeList = InfoSql.MonitorTbPerson();//需要修改的修理人列表
                            if (_ChangeList.Count != 0)
                            {
                                string _fileDir = Environment.CurrentDirectory;//获取当前路径
                                string _endPath = _fileDir + "\\" + "img";//找到img文件
                                string[] files = Directory.GetFiles(_endPath, "*.png");//获取其下所有的图片
                                foreach (string file in files)//遍历图片
                                {
                                    GlobalPicapth = file;//放入窗体显示
                                    string _strText = string.Empty;
                                    Show(out _strText);//显示窗体
                                    CommonFunc.CommonSleep("reloadfunc", 5000);
                                    CommonFunc.DeleteFilePng(file);
                                }
                            }
                        }
                    }
                    else
                    {
                        CommonFunc.tips("dms没有打开，请检查后重新启动");
                    }
                }
                else
                {
                    CommonFunc.tips("网络连接异常，请检查");
                }
            }
            return true;
        }
        
        public static DialogResult Show(out string strText)//窗体显示文件
        {
            string _strTemp = string.Empty;
            FrmInputDialog _inputDialog = new FrmInputDialog();
            _inputDialog.TextHandler = (str) => { _strTemp = str; };
            DialogResult _result = _inputDialog.ShowDialog();
            strText = _strTemp;
            return _result;
        }

        private void StartClick(object sender, EventArgs e)//点击开始
        {
            string _allEnd = OperateSql.GetDmsInfo("md5s");
            bool _IsShouQuan = LicenseCode.IsShouQuan(_allEnd);
            _objThread = new Thread(new ThreadStart(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                ReloadFunc();
            }));
            if (!_IsShouQuan)
            {
                if (!LicenseCode.RestartDms())
                {
                    return;
                }
                if (t1_state == 0)//是开始
                {
                    this.start.Text = "暂停";
                    this.systemstat.Text = "系统状态：暂停";
                    GlobalSystemStop = false;
                    _objThread.Start();
                    t1_state = 1;
                }
                else if (t1_state == 1)
                {//是暂停
                    this.start.Text = "开始";
                    this.systemstat.Text = "系统状态：开始抓取";
                    GlobalSystemStop = true;
                    if (_objThread.IsAlive)
                    {
                        _objThread.Suspend();
                    }
                    t1_state = 2;
                }
                else
                {
                    if (_objThread.IsAlive)
                    {
                        _objThread.Resume();
                    }
                    t1_state = 1;
                    GlobalSystemStop = false;
                    this.systemstat.Text = "系统状态：暂停";
                    this.start.Text = "暂停";
                }
            }
            else
            {
                this.buildkey.Show();
                this.buildkeyinput.Show();
                this.buildkeybutton.Show();
                this.timelabel.Text = "试用期";
                if (istrytime())
                {
                    CommonFunc.tips("尊敬的用户，请在设置中添加授权信息继续使用本软件");
                    GlobalSystemStop = false;
                }
                else
                {
                    if (!LicenseCode.RestartDms())
                    {
                        return;
                    }
                    if (t1_state == 0)//是开始
                    {
                        this.start.Text = "暂停";
                        this.systemstat.Text = "系统状态：暂停";
                        GlobalSystemStop = false;
                        _objThread.Start();
                        t1_state = 1;
                    }
                    else if (t1_state == 1)
                    {//是暂停
                        this.systemstat.Text = "系统状态：开始抓取";
                        GlobalSystemStop = true;
                        if (_objThread.IsAlive)
                        {
                            _objThread.Suspend();
                        }
                        t1_state = 2;
                        this.start.Text = "开始";
                    }
                    else
                    {
                        if (_objThread.IsAlive)
                        {
                            _objThread.Resume();
                        }
                        t1_state = 1;
                        GlobalSystemStop = false;
                        this.systemstat.Text = "系统状态：暂停";
                        this.start.Text = "暂停";
                    }
                }
            }
        }

        /*获取dms信息，存储到数据库，并启动上传公安网，上传完成恢复dms到初始状态*/
        public void GetInfoAndUpload()
        {
            IntPtr _BigHwnd = FindWindow("KCMLMasterForm_32", null);
            GlobalIsZhuaQuSucc = false;
            CommonFunc.CommonSleep("GetInfoAndUpload", sl.Little);
            logg.Info("开始进入getmsg");
            SendMessage(_BigHwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
            OpenWindGetMsg.GetMsg();//获取所有信息
            logg.Info("getmsg调用完成");
            string _WindowTitle = "";
            if (_BigHwnd != IntPtr.Zero)
            {
                GetmsgProcessNeedFunc.enumwindow(_BigHwnd, ConstGlobalXinjianTag);

                //// 恢复dms页面到原样
                //StringBuilder _title = new StringBuilder(102400);
                //GetWindowText(_BigHwnd, _title, _title.Capacity);//获取窗口标题
                //if (_title.ToString().Length > 5)
                //{
                //    _WindowTitle = _title.ToString();
                //}
                //logg.Info("恢复dms页面成功");
                //CommonFunc.CommonSleep("GetInfoAndUpload", sl.Moment);
                //bool _mouse_call = tengchao.MouseClick.AddYanZhengClickOne(_WindowTitle, "ToolBar_Class", _BigHwnd, 74, 23, 2500);//点击新建
            }
            CommonFunc.CommonSleep("GetInfoAndUpload", sl.Longer);
        }
        //运行初始化
        private void MainFunc()
        {
            // 把自己的程序最小化
            this.WindowState = FormWindowState.Minimized;
            logg.Info("开始更新状态");
            LoadTextboxEnd();
            myTopMost.update_hwind_txt();
            CommonFunc.CommonSleep("MainFunc", sl.Moment);

            logg.Info("进入当日工单抓取过程");
            GetInfoAndUpload();//开始抓取
            logg.Info("一次当日工单抓取过程完成");

        }
        //加载目前抓取上传状态
        public void LoadTextboxEnd()
        {
            logg.Info("更新log...现在时间是：" + DateTime.Now.ToString());
            int zhuaqusuccess = OperateSql.CountsFromSqlite("true");
            int zhuaqufails = OperateSql.CountsFromSqlite("false");
            int allcount = zhuaqusuccess + zhuaqufails;
            string zhuaqusucc = OperateSql.GetLogFromData("true");
            this.allcountlabel.Text =  allcount.ToString();
            this.spidersuccelllabel.Text =  OperateSql.CountsFromSqlite("true").ToString();
            this.spiderfaillabel.Text =OperateSql.CountsFromSqlite("false").ToString();
            this.uploadsucclabel.Text =  OperateSql.GetRuChangNum("上传成功").ToString() + "/" + OperateSql.GetChuchangNum("上传成功").ToString();
            logg.Info("开始显示log...");
            GetCaptureSuccessInfo();
        }
        //加载表格内容
        public void GetCaptureSuccessInfo()
        {
            List<string> _successinfo = ZebraSql.GetCaptureLogInfo("true");
            try
            {
                for (int i = 0; i < _successinfo.Count; i++)
                {
                    successDataGridView.Rows.Add();
                    string[] info_list = _successinfo[i].ToString().Split(new char[] { '@' });
                    for (int j = 0; j < info_list.Length; j++)
                    {
                        successDataGridView.Rows[i].Cells[j].Value = info_list[j];
                    }
                }
            }
            catch (Exception ex)
            {
                logg.Error("抓取成功log, Error..." + ex.ToString());
            }

            List<string> _failureinfo = ZebraSql.GetCaptureLogInfo("false");
            try
            {
                for (int i = 0; i < _failureinfo.Count; i++)
                {
                    failureDataGridView.Rows.Add();
                    string[] info_list = _failureinfo[i].ToString().Split(new char[] { '@' });
                    for (int j = 0; j < info_list.Length; j++)
                    {
                        failureDataGridView.Rows[i].Cells[j].Value = info_list[j];
                    }
                }
            }
            catch (Exception ex)
            {
                logg.Error("抓取失败log, Error..." + ex.ToString());
            }
            List<string> _uploadinfo = ZebraSql.GetUploadLogInfo();
            try
            {
                UploadDataGridView.Rows.Clear();
                for (int i = 0; i < _uploadinfo.Count; i++)
                {
                    UploadDataGridView.Rows.Add();
                    string[] info_list = _uploadinfo[i].ToString().Split(new char[] { '@' });
                    for (int j = 0; j < info_list.Length; j++)
                    {
                        UploadDataGridView.Rows[i].Cells[j].Value = info_list[j];
                    }
                }
            }
            catch (Exception ex)
            {
                logg.Error("上传成功log, Error..." + ex.ToString());
            }
            try
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string todaybefore20 = DateTime.Now.AddDays(-20).ToString("yyyy-MM-dd HH:mm:ss");
                string todaybefore19 = DateTime.Now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss");
                string todaybefore30 = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                chu_and_ru_datanum.Rows[0].Cells[0].Value = ZebraSql.get_uploadnum_between_startdate_and_enddate(todaybefore30, todaybefore20, "0").ToString();
                chu_and_ru_datanum.Rows[0].Cells[1].Value = ZebraSql.get_uploadnum_between_startdate_and_enddate(todaybefore30, todaybefore20, "1").ToString();
                chu_and_ru_datanum.Rows[0].Cells[2].Value = ZebraSql.get_uploadnum_between_startdate_and_enddate(todaybefore19, today, "0").ToString();
                chu_and_ru_datanum.Rows[0].Cells[3].Value = ZebraSql.get_uploadnum_between_startdate_and_enddate(todaybefore19, today, "1").ToString();
            }
            catch (Exception ex)
            {
                logg.Error("数据统计log, Error..." + ex.ToString());
            }
        }

        //杀掉程序
        public static void KillProcess(string processName)
        {
            Process[] myproc = Process.GetProcesses();
            foreach (Process item in myproc)
            {
                if (item.ProcessName == processName)
                {
                    item.Kill();
                }
            }
        }

        [Conditional("DEBUG")]
        private void WrongInput()
        {
            skinLabel1.Hide();
            CommonFunc.CreateImg();
        }
        [Conditional("TRACE")]
        private void WrongLabel()
        {
            label4.Hide();
            richTextBox1.Hide();
            submitbutton.Hide();
            submitpanel.Height = 140;
        }
        //加载文字 窗体
        private bool InitializeFile()
        {
            //this.Text = GlobalZhongWenPinPai + "4S自动录入系统"; //通过品牌设置dms标题
            WrongInput();//根据宏设置窗体显示和隐藏
            WrongLabel();//根据宏设置窗体显示和隐藏
            string allrepair = OperateSql.GetDmsInfo("allrepair");//读取是否抓取全部工单
            if (allrepair.Contains("true"))
            {
                this.radioButton2.Checked = false;
                this.radioButton1.Checked = true;
                GlobalIsAllRapair = true;
            }
            else
            {
                this.radioButton2.Checked = true;
                this.radioButton1.Checked = false;
                GlobalIsAllRapair = false;
            }
            string dayspider = OperateSql.GetDmsInfo("dayspider");//读取抓取新老工单的配置项
            if (dayspider.Contains("true"))
            {
                this.radioButton3.Checked = true;
                this.radioButton4.Checked = false;
            }
            else
            {
                this.radioButton4.Checked = true;
                this.radioButton3.Checked = false;
            }
            CommonFunc.CreateImg(); //创建img文件 保存截图
            try
            {
                if (File.Exists(@"weixiupath.txt"))//判断路径是否存在
                {
                    WeixiuDictionary = CommonFunc.getweixiutxt(@"weixiupath.txt");//读取全部维修人员
                }
                else
                {
                    WeixiuDictionary = OperateSql.GetWeixiuDic();
                }
            }
            catch
            {
                WeixiuDictionary = OperateSql.GetWeixiuDic();
            }
            foreach (KeyValuePair<int, List<string>> kvp in WeixiuDictionary)
            {//把读取到的人员 填入表格
                if (kvp.Value[0].ToString().Length > 0)
                {
                    int index = this.skinDataGridView1.Rows.Add();
                    skinDataGridView1.Rows[index].Cells[0].Value = kvp.Value[0].ToString();
                    skinDataGridView1.Rows[index].Cells[1].Value = kvp.Value[1].ToString();
                    skinDataGridView1.Rows[index].Cells[2].Value = kvp.Value[2].ToString();
                }
            }
            // 删除data表和upload表30天以前的数据
            ZebraSql.DeleteDataAndUploadManyDay(GlobalNeedDataDay);
            // 增加删除img下的图片，此做法是为了保证dms卡死截图出错的情况下，不影响后面的程序运行
            MainWind.DeletePicFromImg();
            ZebraSql.DeleteTableAndLogSomeDay(GlobalNeedDataDay);
            return true;
        }
        //授权信息以及窗体其他文字加载
        private bool initialize()
        {
            fwsPrevious = this.WindowState;//显示最小化窗体
            myTopMost = new FrmTopMost(this);
            try
            {
                try
                {
                    this.closetime.Text = OperateSql.GetDmsInfo("closetime");//读取关闭程序时间
                    this.textBox1.Text = OperateSql.GetDmsInfo("dmsname");//dms账号
                    this.textBox2.Text = OperateSql.GetDmsInfo("dmspassword");//dms密码
                    this.textBox6.Text = OperateSql.GetDmsInfo("dmsip");//dms ip
                    this.textBox3.Text = OperateSql.GetDmsInfo("gawname");//公安网账号
                    this.textBox4.Text = OperateSql.GetDmsInfo("gawpassword");//公安网密码
                }
                catch (Exception e)
                {
                    logg.Error("initialize error, please check..." + e.ToString());
                }
                //LoadTextboxEnd();//从数据库读取抓取个数
                myTopMost.update_hwind_txt();//更新窗体的信息
                bool _is_dms = lices.ReadDms();//判断是否添加了dms信息
                bool _is_gaw = lices.ReadGongAn();//判断是否添加了公安网信息
                if (_is_dms)
                {//根据状态更新窗体显示内容
                    this.dmsstat.Text = "DMS账号状态：已添加";
                }
                else
                {
                    this.dmsstat.Text = "DMS账号状态：未添加";
                }
                if (_is_gaw)
                {
                    this.gawstat.Text = "公安网账号状态：已添加";
                }
                else
                {
                    this.gawstat.Text = "公安网账号状态：未添加";
                }
                LicenseCode.RestartDms();//读取dms信息
                lices.RestartGaw();//读取公安网信息
                string allend = OperateSql.GetDmsInfo("md5s");//读取授权码信息
                bool _IsShouQuan = LicenseCode.IsShouQuan(allend);//判断是否授权
                if (allend.Length > 5)
                {
                    _IsShouQuan = true;
                }
                else {
                    _IsShouQuan = false;
                }
                if (!_IsShouQuan)//判断是否授权
                {//未授权状态
                    this.buildkey.Show();
                    this.textBox1.Show();
                    this.buildkeybutton.Show();
                    if (istrytime())//判断是否超过试用期
                    {
                        this.shouquanlabel.Text = "未授权";
                        GlobalSystemStop = false;
                        CommonFunc.tips("尊敬的用户，您的试用已到期，请购买授权码");
                    }
                    else
                    {
                        string congigtime = OperateSql.GetDmsInfo("trytime");//appconfig中读取时间
                        string nowtimres = GMT2Local(GetNetDateTime()).ToString("yyyy-MM-dd");
                        if (congigtime.Length > 0)
                        {
                            string congigtimes = MD5Decrypt(congigtime, "JUNDAOXT");//解密时间
                            DateTime joinDate1 = Convert.ToDateTime(congigtimes);
                            DateTime joinDate2 = Convert.ToDateTime(nowtimres);
                            int endday = 7 - (int)(joinDate2 - joinDate1).TotalDays;//判断7天试用期是否超过
                            if (endday > 0)
                            {
                                CommonFunc.tips("尊敬的用户，您的试用期还有" + endday.ToString() + "天");
                            }
                            else
                            {
                                CommonFunc.tips("尊敬的用户，您的试用已到期，请购买授权码");
                            }
                        }
                        this.timelabel.Text = "试用期";
                        this.shouquanlabel.Text = "试用期";
                    }
                }
                else
                {
                    this.buildkey.Hide();
                    this.buildkeyinput.Hide();
                    this.buildkeybutton.Hide();
                    this.shouquanlabel.Text = "已授权";
                    try
                    {// 新增到期检测
                        DateTime dt = lices.YouXiaoDate(allend);//判断是否有效
                        this.timelabel.Text = dt.ToString();//更新窗体授权相关信息
                        TimeSpan ts = dt - DateTime.Now;
                        double dDays = ts.TotalDays;//带小数的天数，比如1天12小时结果就是1.5
                        int nDays = ts.Days;//整数天数，1天12小时或者1天20小时结果都是1
                        if (15 >= nDays)
                        {
                            if (nDays >= 0)
                            {
                                CommonFunc.tips("尊敬的用户，此软件到期时间是" + dt.ToString().Split(' ')[0] + "，离到期时间还有" + nDays.ToString() + "天，请尽快联系售后人员重新获取授权码，联系方式:176-1006-5353");
                            }
                            else
                            {
                                CommonFunc.tips("尊敬的用户，您的授权已到期，请联系售后人员重新获取授权码");
                                this.shouquanlabel.Text = "授权过期";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.timelabel.Text = "试用期";
                    }
                }
            }
            catch (Exception e)
            {
                logg.Error("look error..." + e.ToString());

                if (istrytime())
                {
                    GlobalSystemStop = false;
                    CommonFunc.tips("尊敬的用户，您的试用已到期，请购买授权码");
                }
                else
                {
                    this.timelabel.Text = "试用期";
                    this.shouquanlabel.Text = "试用期";
                    string congigtime = OperateSql.GetDmsInfo("trytime");//appconfig中读取时间
                    string nowtimres = GMT2Local(GetNetDateTime()).ToString("yyyy-MM-dd");
                    if (congigtime.Length > 0)
                    {
                        string congigtimes = MD5Decrypt(congigtime, "JUNDAOXT");//解密时间
                        DateTime joinDate1 = Convert.ToDateTime(congigtimes);
                        DateTime joinDate2 = Convert.ToDateTime(nowtimres);
                        int endday = 7 - (int)(joinDate2 - joinDate1).TotalDays;
                        if (endday > 0)
                        {
                            CommonFunc.tips("尊敬的用户，您的试用期还有" + endday.ToString() + "天");
                        }
                        else
                        {
                            CommonFunc.tips("尊敬的用户，您的试用已到期，请购买授权码");
                        }
                    }
                }
            }
            return true;
        }
        //读取路径
        private bool InitializeBat()
        {
            try
            {//读取dms和公安网路径
                this.dmsinputform.Text = OperateSql.GetDmsInfo("dmspath");
                this.gawinputfrom.Text = OperateSql.GetDmsInfo("gawpath");
            }
            catch
            {
                CommonFunc.tips("请确认是否设置DMS或公安网路径");
                this.settip.Show();
            }
            return true;
        }
        //读取维修人员
        private bool InitializeWaitData()
        {
            while (true)
            {
                if (WeixiuDictionary.Count > 3)
                {
                    this.start.Enabled = true;
                    break;
                }
            };
            return true;
        }
        //读取展示数据
        private void InputinfoLoad(object sender, EventArgs e)//窗体初始化加载
        {
            string dbConnect_A = "server=localhost;user id=root;password=root;database=gongandmsdb;charset=utf8";
            MySqlConnectionPool ConnectMysql = new MySqlConnectionPool(dbConnect_A);
            GlobalConnectMysql = ConnectMysql;
            string today = DateTime.Now.ToString("yyyy/MM/dd");
            string todaybefore20 = DateTime.Now.AddDays(-20).ToString("yyyy/MM/dd");
            string todaybefore19 = DateTime.Now.AddDays(-19).ToString("yyyy/MM/dd");
            string todaybefore30 = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");
            this.chu_and_ru_datanum.Columns[0].HeaderText = todaybefore30 + "-" + todaybefore20 + "入场上传总数";
            this.chu_and_ru_datanum.Columns[0].ToolTipText = todaybefore30 + "-" + todaybefore20 + "入场上传总数";
            this.chu_and_ru_datanum.Columns[1].HeaderText = todaybefore30 + "-" + todaybefore20 + "出场上传总数";
            this.chu_and_ru_datanum.Columns[1].ToolTipText = todaybefore30 + "-" + todaybefore20 + "出场上传总数";
            this.chu_and_ru_datanum.Columns[2].HeaderText = todaybefore19 + "-" + today + "入场上传总数";
            this.chu_and_ru_datanum.Columns[2].ToolTipText = todaybefore19 + "-" + today + "入场上传总数";
            this.chu_and_ru_datanum.Columns[3].HeaderText = todaybefore19 + "-" + today + "出场上传总数";
            this.chu_and_ru_datanum.Columns[3].ToolTipText = todaybefore19 + "-" + today + "出场上传总数";
            this.submitpanel.Hide();
            this.start.Enabled = false;
            Thread _intlizeThread = new Thread(new ThreadStart(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                initialize();
            }));
            Thread _intlizeThreadFile = new Thread(new ThreadStart(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                InitializeFile();
            }));
            Thread _intlizeThreadBat = new Thread(new ThreadStart(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                InitializeBat();
            }));
            Thread _initializeWaitData = new Thread(new ThreadStart(delegate
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                InitializeWaitData();
            }));
            _intlizeThread.Start();
            _intlizeThreadFile.Start();
            _intlizeThreadBat.Start();
            _initializeWaitData.Start();
        }
        // 点击开始按钮，此时会检查公安网和dms是否都已经启动，如果没有启动会提示
        private void RestartDmsButtonClick(object sender, EventArgs e)//重启dms
        {
            string _allEnd = OperateSql.GetDmsInfo("md5s");
            bool _IsShouQuan = LicenseCode.IsShouQuan(_allEnd);
            if (!_IsShouQuan)
            {
                string _DeskTopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string _DmsName = OperateSql.GetDmsInfo("dmspath");//获取dms路径
                LicenseCode.RestartDms();
            }
            else
            {
                this.buildkey.Show();
                this.buildkeyinput.Show();
                this.buildkeybutton.Show();
                if (istrytime())
                {
                    CommonFunc.tips("尊敬的用户，请在设置中添加授权信息继续使用本软件");
                    GlobalSystemStop = false;
                }
            }
        }
        private void ReatsrtGawButtonClick(object sender, EventArgs e)//重启公安网
        {
            string _AllEnd = OperateSql.GetDmsInfo("md5s");
            bool _IsShouQuan = LicenseCode.IsShouQuan(_AllEnd);
            if (!_IsShouQuan)
            {
                lices.RestartGaw();
                KillProcess("jixiu");
                CommonFunc.CommonSleep("ReatsrtGawButtonClick", 2000);
                string _GawPath = OperateSql.GetDmsInfo("gawpath");
                ShellExecute(IntPtr.Zero, new StringBuilder("Open"), new StringBuilder(_GawPath), new StringBuilder(""), null, 1);
                CommonFunc.CommonSleep("ReatsrtGawButtonClick", 2000);
            }
            else
            {
                this.buildkey.Show();
                this.buildkeyinput.Show();
                this.buildkeybutton.Show();
                if (istrytime())
                {
                    CommonFunc.tips("尊敬的用户，请在设置中添加授权信息继续使用本软件");
                    GlobalSystemStop = false;
                }
            }
        }
        private void SetButtonClick(object sender, EventArgs e)//点击设置
        {
            this.settip.Show();
        }
        private void CloseClick(object sender, EventArgs e)//点击设置页的关闭符号
        {
            this.settip.Hide();
        }
        private void VerifyKeyButtonClick(object sender, EventArgs e)//点击验证密钥
        {
            if (this.showkeyinput.Text.Length == 0)
            {
                try
                {
                    bool _IsShouQuan = false;
                    string _AllEnd = OperateSql.GetDmsInfo("md5s");
                    if (_AllEnd.Length > 5)
                    {
                        this.showkeyinput.Text = _AllEnd;
                        _IsShouQuan = LicenseCode.IsShouQuan(_AllEnd);//判断是否授权
                        DateTime dt = lices.YouXiaoDate(_AllEnd);
                        this.timelabel.Text = dt.ToString();
                    }
                    else {
                        _IsShouQuan = false;
                    }
                    if (_IsShouQuan)
                    {//未授权
                        this.buildkey.Show();
                        this.buildkeyinput.Show();
                        this.buildkeybutton.Show();
                        this.shouquanlabel.Text = "已授权";
                        CommonFunc.tips("尊敬的用户，您的授权码处于有效期");
                    }
                    else
                    {//授权
                        this.buildkey.Hide();
                        this.buildkeyinput.Hide();
                        this.buildkeybutton.Hide();
                        this.shouquanlabel.Text = "未授权";
                        if (istrytime())
                        {
                            CommonFunc.tips("尊敬的用户，系统检测到您的授权已失效");
                            GlobalSystemStop = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (istrytime())
                    {
                        logg.Info(ex.ToString());
                        CommonFunc.tips("尊敬的用户，系统检测授权码时出错，请重新添加");
                        GlobalSystemStop = false;
                    }
                }
            }
            else
            {
                string _AllEnd = this.showkeyinput.Text;
                bool _IsShouQuan = LicenseCode.IsShouQuan(_AllEnd);
                DateTime dt = lices.YouXiaoDate(_AllEnd);
                this.timelabel.Text = dt.ToString();
                OperateSql.SetDmsInfo("md5s", _AllEnd);
                if (_IsShouQuan)
                {
                    this.buildkey.Show();
                    this.buildkeyinput.Show();
                    this.buildkeybutton.Show();
                    this.shouquanlabel.Text = "未授权";
                    if (istrytime())
                    {
                        CommonFunc.tips("尊敬的用户，系统检测到您的授权已失效");
                        GlobalSystemStop = false;
                    }
                }
                else
                {
                    this.buildkey.Hide();
                    this.buildkeyinput.Hide();
                    this.buildkeybutton.Hide();
                    this.shouquanlabel.Text = "已授权";
                    CommonFunc.tips("尊敬的用户，您的授权码处于有效期");
                }
            }
        }
        private void SaveDmsButtonClick(object sender, EventArgs e)//设置里 点击dms保存
        {
            string _DmsName = this.textBox1.Text;
            string _DmsPassWord = this.textBox2.Text;
            string _DmsIp = this.textBox6.Text;
            if (_DmsName.Length == 0)
            {
                CommonFunc.tips("请输入DMS账号");
            }
            else if (_DmsPassWord.Length == 0)
            {
                CommonFunc.tips("请输入DMS密码");
            }
            else if (_DmsIp.Length == 0)
            {
                CommonFunc.tips("请输入IP号");
            }
            else
            {
                if (Regex.IsMatch(_DmsIp, @"\b(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\b"))
                {
                    OperateSql.SetDmsInfo("dmsname", _DmsName);
                    OperateSql.SetDmsInfo("dmspassword", _DmsPassWord);
                    OperateSql.SetDmsInfo("dmsip", _DmsIp);
                    this.dmsstat.Text = "DMS账号状态：已添加";
                    CommonFunc.tips("保存DMS信息成功");
                }
                else
                {
                    CommonFunc.tips("请输入正确ip号");
                }
            }
        }
        private void SaveGawButtonClick(object sender, EventArgs e)//设置里 点击公安网保存
        {
            string _DmsName = this.textBox3.Text;
            string _DmsPassWord = this.textBox4.Text;
            if (_DmsName.Length == 0)
            {
                CommonFunc.tips("请输入公安网账号");
            }
            else if (_DmsPassWord.Length == 0)
            {
                CommonFunc.tips("请输入公安网密码");
            }
            else
            {

                OperateSql.SetDmsInfo("gawname", _DmsName);
                OperateSql.SetDmsInfo("gawpassword", _DmsPassWord);
                this.gawstat.Text = "公安网账号状态：已添加";
                CommonFunc.tips("保存公安网信息成功");
            }
        }
        private void UrlLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)//点击超链接
        {
            System.Diagnostics.Process.Start("iexplore.exe", "www.data-stone.com");
        }
        private void GawTabDrawItem(object sender, DrawItemEventArgs e)//重新绘制字体 使tab栏目字体横向排列
        {
            StringFormat StrFormat = new StringFormat();
            SolidBrush bru = new SolidBrush(Color.FromArgb(255, 255, 255));
            SolidBrush bruFont = new SolidBrush(Color.FromArgb(0, 0, 0));// 标签字体颜色
            Font font = new System.Drawing.Font("微软雅黑", 9F);//设置标签字体样式
            StrFormat.LineAlignment = StringAlignment.Center;// 设置文字垂直方向居中
            StrFormat.Alignment = StringAlignment.Center;// 设置文字水平方向居中 
            for (int i = 0; i < gawtab.TabPages.Count; i++)
            {
                Rectangle recChild = gawtab.GetTabRect(i);
                e.Graphics.FillRectangle(bru, recChild);
                e.Graphics.DrawString(gawtab.TabPages[i].Text, font, bruFont, recChild, StrFormat);
            }
        }
        private void InputinfoSizeChanged(object sender, EventArgs e)//点击窗体放大缩小
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                myTopMost.Show();
                this.ShowInTaskbar = true;
            }
            else if (this.WindowState != fwsPrevious)
            {
                fwsPrevious = this.WindowState;
            }
        }
        public void RestoreWindow()//恢复窗体放大前状态
        {
            this.WindowState = fwsPrevious;
            this.ShowInTaskbar = true;
        }
        private void BuildKeyButtonClick(object sender, EventArgs e)//赋值时间
        {
            string md5ss = LicenseCode.TakeMd5s();
            this.buildkeyinput.Text = md5ss;
        }
        private void CloseTimeMouseLeave(object sender, EventArgs e)//关闭时间框 创建时间xml文档
        {
            DateTime dt;
            if (!DateTime.TryParseExact(this.closetime.Text, "HH:mm", null, DateTimeStyles.None, out dt))
            {
                CommonFunc.tips("请按照正确的格式输入");
            }
            else
            {
                string timres = this.closetime.Text;
                OperateSql.SetDmsInfo("closetime", timres);
            }
        }

        private void SubmitBugClick(object sender, EventArgs e)//提交错误按钮
        {
            this.submitpanel.Show();
        }

        private void CloseSubmitClick(object sender, EventArgs e)//点击隐藏报错窗口
        {
            this.submitpanel.Hide();
        }

        private void SubmitButtonClick(object sender, EventArgs e)//点击提交报错信息
        {
            string bugtxt = this.richTextBox1.Text;
            if (bugtxt.Length == 0)
            {
                CommonFunc.tips("报错信息不能为空");
            }
            else
            {
                CommonFunc.SendBug(bugtxt, "1", bugtxt, "inputinfo", "sendbug");
                CommonFunc.tips("提交成功，请关闭对话框");
            }
        }
        //点击确定保存路径
        private void dmsbtnconfirm_Click(object sender, EventArgs e)
        {
            if (this.dmsinputform.Text.Length == 0)
            {
                CommonFunc.tips("请输入DMS路径");
            }
            else if (this.gawinputfrom.Text.Length == 0)
            {
                CommonFunc.tips("请输入公安网路径");
            }
            else
            {
                OperateSql.SetDmsInfo("dmspath", this.dmsinputform.Text);
                OperateSql.SetDmsInfo("gawpath", this.dmsinputform.Text);
                this.settip.Hide();
                Dispose();
                Thread.Sleep(1000);
                System.Windows.Forms.Application.Restart();
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            GlobalIsAllRapair = true;//设置抓取时候工项是否全部抓取
            OperateSql.SetDmsInfo("allrepair", "true");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            GlobalIsAllRapair = false;//设置抓取时候工项是否全部抓取
            OperateSql.SetDmsInfo("allrepair", "false");
        }
        //暂时不调用
        private void failureDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            string msg = "是否删除此wip号？";
            if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
            {
                try
                {
                    int index = failureDataGridView.CurrentCell.RowIndex;
                    string content = this.failureDataGridView.Rows[index].Cells[0].Value.ToString();
                    string dates = DateTime.Now.ToString("yyyy-MM-dd");
                    string sql = "delete FROM datatb WHERE wip_num='" + content + "' and first_ruku_time LIKE '%" + dates + "%'";
                    OperateSql.PublicSql(sql);
                    CommonFunc.tips("删除wip号成功");
                }
                catch (Exception ex)
                {
                    CommonFunc.tips("删除wip号失败");
                }
            }
        }
        //点击确认信息
        private void skinDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string msg = "是否确认此信息？";
            if ((int)MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == 1)
            {
                try
                {
                    string name_id = skinDataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    string type = skinDataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    string name = skinDataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    int count = OperateSql.GetWeiXiuCount(name_id);
                    if (count > 0)
                    {
                        OperateSql.ChangeRepairMsg("1", name_id, type, name);
                        CommonFunc.tips("信息保存成功");
                    }
                    else
                    {
                        OperateSql.ChangeRepairMsg("insert", name_id, type, name);
                        CommonFunc.tips("信息保存成功");
                    };
                }
                catch (Exception ex)
                {
                    CommonFunc.tips("信息保存成功");
                }
                try
                {
                    File.Delete(@"weixiupath.txt");
                }
                catch
                {

                }
            }
        }
        //改变是否抓取的单选
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            OperateSql.SetDmsInfo("dayspider", "true");
        }
        //
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            OperateSql.SetDmsInfo("dayspider", "false");
        }
        private void InputinfoFormClosing(object sender, FormClosingEventArgs e)//关闭窗体
        {
            DialogResult result = MessageBox.Show("是否统计数据？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                //取消
                GlobalSystemStop = true;
                Dispose();
                Application.Exit();
                Application.ExitThread();
                System.Environment.Exit(System.Environment.ExitCode);
            }
            else
            {
                //入场多少个  出厂多少个
                //是否确认 确认就关闭窗体  取消就继续抓
                e.Cancel = true; //取消关闭操作
                int chuchangcount = OperateSql.GetChuchangNum("上传成功");
                int ruchangcount = OperateSql.GetRuChangNum("上传成功");
                DialogResult result1 = MessageBox.Show("出厂数据总条数" + chuchangcount + "入场数据总条数" + ruchangcount+"，是否确认?", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result1 == DialogResult.No)
                {
                    e.Cancel = true; //取消关闭操作
                }
                else
                {
                    Dispose();
                    Application.Exit();
                    Application.ExitThread();
                    System.Environment.Exit(System.Environment.ExitCode);
                }
            }
        }
        //点击帮助按钮
        private void help_Click(object sender, EventArgs e)
        {
            string path1 = @"intouduce.docx";
            string foldPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)+"\\"+ "intouduce.docx";
            File.Copy(path1,foldPath,true);
            MessageBox.Show("说明文档已经下载到桌面");
        }
    }
}