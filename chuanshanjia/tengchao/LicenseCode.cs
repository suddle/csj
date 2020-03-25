using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static tengchao.FreePermission;
using static tengchao.PublicDefine;
namespace tengchao
{
    class LicenseCode
    {
        /// <summary>
        /// 制作加密字符串
        /// </summary>
        /// <returns></returns>
        public static string TakeMd5s()
        {
            string _EndData ="";
            Control.CheckForIllegalCrossThreadCalls = false;
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            string _end = MD5Encrypt32();
            DateTime _nows = DateTime.Now;
            string _nows1 = _nows.ToString("yyyy-MM-dd");
            string _nows2 = _nows.ToString("yyyy-MM-dd_HH-mm-ss");
            string suiji = Rand();
            _EndData = _end + suiji;
            return _EndData;
        }
        /// <summary>
        /// 判断是否授权
        /// </summary>
        /// <param name="allend"></param>
        /// <returns></returns>
        public static bool IsShouQuan(string allend)
        {
            try
            {
                string _JieXiData=JieXiMd5(allend);
                string _TakeMd5 = TakeMd5s();
                if (_JieXiData.Length > 0)
                {
                    if (_TakeMd5.Contains(_JieXiData))
                    {
                        allend = allend.Substring(0, allend.Length - 6);
                        string unicode_time = AnalysisData(allend);
                        string lice_time = UnicodeToString(unicode_time);
                        lice_time = lice_time.Replace("-", "").Replace("_", "");
                        lice_time = lice_time.Substring(0, 8);
                        DateTime dt = DateTime.ParseExact(lice_time, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        var result = IsExpriredByDay(dt, 1);//判断日期是否到期
                        if (result)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else {
                    return true;
                }
            }
            catch(Exception ex)
            {
                logg.Info("授权出错IsShouQuan"+ex.ToString());
                CommonFunc.SendBug("授权出错IsShouQuan", "2", ex.ToString(), "LicenseCode", "IsShouQuan");
                return true;
            }
        }
        /// <summary>
        /// 解析md5 
        /// </summary>
        /// <param name="allend">md5字符串</param>
        /// <returns></returns>
        public static string JieXiMd5(string allend)
        {
            string _EndData = "";
            if (allend.Length > 0)
            {
                string[] _DataArray1 = allend.Split('u');
                foreach (var item in _DataArray1)
                {
                    if (item.Length > 3)
                    {
                        _EndData += item.Substring(4, item.Length - 4);
                    }
                    else
                    {
                        _EndData += item;
                    }
                }
                string _SubData = _EndData.Remove(_EndData.Length - 7);
                return _SubData;
            }
            else {
                return _EndData;
            }
        }
        /// <summary>
        /// 判断日期的有效值
        /// </summary>
        /// <param name="allend"></param>
        /// <returns></returns>
        public DateTime YouXiaoDate(string allend)
        {
            allend = allend.Substring(0, allend.Length - 6);
            string _UnicodeTime = AnalysisData(allend);
            string _LiceTime = UnicodeToString(_UnicodeTime);
            _LiceTime = _LiceTime.Replace("-", "").Replace("_", "");
            _LiceTime = _LiceTime.Substring(0, 8);
            DateTime dt = DateTime.ParseExact(_LiceTime, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            return dt;
        }
        /// <summary>
        /// 对比日期
        /// </summary>
        /// <param name="joinDate"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static bool IsExpriredByDay(DateTime joinDate, double duration)
        {
            int compNum = DateTime.Compare(DateTime.Now, joinDate);
            if (compNum < 0)
            {
                return true;
            }
            else {
                return false;
            }
            //MessageBox.Show(compNum.ToString());
            //return DateTime.Now - joinDate > TimeSpan.FromDays(duration);
        }
        /// <summary>
        /// 解析日期
        /// </summary>
        /// <param name="allend"></param>
        /// <returns></returns>
        public static string AnalysisData(string allend)
        {
            string _EndData = "";
            string[] _sArray1 = allend.Split('u');
            foreach (var item in _sArray1)
            {
                if (item.Length > 3)
                {
                    _EndData += "\\" + "u" + item.Substring(0, 4);
                }
            }
            return _EndData;
        }
        /// <summary>
        /// unicode转string
        /// </summary>
        /// <param name="srcText">字符串</param>
        /// <returns></returns>
        public static string UnicodeToString(string srcText)
        {
            string _dst = "";
            string _src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string _str = "";
                _str = _src.Substring(0, 6).Substring(2);
                _src = _src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(_str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(_str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                _dst += Encoding.Unicode.GetString(bytes);
            }
            return _dst;
        }
        /// <summary>
        /// 重置公安网
        /// </summary>
        public void RestartGaw()
        {
            try
            {
                if (OperateSql.GetDmsInfo("gawname").Length > 0)
                {
                    GawUaerName = OperateSql.GetDmsInfo("gawname");
                    GawPassword = OperateSql.GetDmsInfo("gawpassword");
                }
                else {
                    CommonFunc.tips("请在设置中重新添加公安网信息");
                }
            }
            catch (Exception ex)
            {
                logg.Info("RestartGaw重置公安网出错，错误是：" + ex.ToString());
                CommonFunc.SendBug("RestartGaw重置公安网出错", "2", ex.ToString(), "LicenseCode", "RestartGaw");
                CommonFunc.tips("请在设置中重新添加公安网信息");
            }
        }
        /// <summary>
        /// 重置dms
        /// </summary>
        /// <returns></returns>
        public static bool RestartDms()
        {
            try
            {
                if (OperateSql.GetDmsInfo("dmsname").Length>0) {
                    DmsNme = OperateSql.GetDmsInfo("dmsname");
                    DmsPassWord = OperateSql.GetDmsInfo("dmspassword");
                    DmsIp = OperateSql.GetDmsInfo("dmsip");
                    return true;
                }
                else {
                    CommonFunc.tips("请在设置中添加DMS信息");
                    return false;
                }

            }
            catch (Exception ex)
            {
                logg.Info("请添加DMS信息，原因是：" + ex.ToString());
                CommonFunc.tips("请在设置中重新添加DMS信息");
                return false;
            }
        }
        /// <summary>
        /// 读取dms
        /// </summary>
        /// <returns></returns>
        public bool ReadDms()
        {
            bool dms_data;
            try
            {
                // string _ends = ReadConfig("dmsfile", "DMS");
                string _ends = OperateSql.GetDmsInfo("dmsname");
                if (_ends.Length > 0)
                {
                    dms_data = true;
                }
                else {
                    dms_data = false;
                }
            }
            catch (Exception ex)
            {
                dms_data = false;
                logg.Info("ReadDms重置DMS出错，错误是：" + ex.ToString());
                CommonFunc.SendBug("ReadDms重置DMS出错", "2", ex.ToString(), "LicenseCode", "ReadDms");
                CommonFunc.tips("尊敬的用户，添加DMS信息时出错，请重新尝试");
            }
            return dms_data;
        }
        /// <summary>
        /// 读取公安网
        /// </summary>
        /// <returns></returns>
        public bool ReadGongAn()
        {
            bool dms_data;
            try
            {
                //string _ends = ReadConfig("gonganfile", "GA");
                string _ends = OperateSql.GetDmsInfo("gawname");
                if (_ends.Length > 0)
                {
                    dms_data = true;
                }
                else
                {
                    dms_data = false;
                }
            }
            catch (Exception ex)
            {
                dms_data = false;
                logg.Info("ReadGongAn重置公安网出错，错误是：" + ex.ToString());
                CommonFunc.SendBug("ReadGongAn重置公安网出错", "2", ex.ToString(), "LicenseCode", "ReadGongAn");
                CommonFunc.tips("请在设置中重新添加公安网信息");
            }
            return dms_data;
        }
        /// <summary>
        /// 加密混淆字符
        /// </summary>
        /// <returns></returns>
        public static string Rand()
        {
            string _all = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,,m,m,o,p,q,r,s,t,u,w,x,y,z";
            string[] allChar = _all.Split(',');
            string _result = "";
            Random _rand = new Random();
            for (int i = 0; i < 6; i++)
            {
                _result += allChar[_rand.Next(35)];
            }
            return _result;
        }
        /// <summary>
        /// 电脑获取的信息转换为md5
        /// </summary>
        /// <returns></returns>
        public static string MD5Encrypt32()//把获取到的电脑信息转换为md5 
        {
            try
            {
                string _cl = GetComputerInfo();
                string _computerinfo = "";
                MD5 md5 = MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(_cl));
                for (int i = 0; i < s.Length; i++)
                {
                    _computerinfo = _computerinfo + s[i].ToString("X");
                }
                return _computerinfo;
            }
            catch (Exception ex)
            {
                logg.Info("错MD5Encrypt32" + ex.ToString());
                CommonFunc.SendBug("错MD5Encrypt32", "2", ex.ToString(), "LicenseCode", "MD5Encrypt32");
                return "";
            }
        }
        /// <summary>
        /// 获取电脑信息
        /// </summary>
        /// <returns></returns>
        public static string GetComputerInfo()//获取电脑信息
        {
            try
            {
                string _info = string.Empty;
                string _cpu = GetCPUInfo();
                string _baseBoard = GetBaseBoardInfo();
                string _bios = GetBIOSInfo();
                string _mac = GetMACInfo();
                _info = string.Concat(_cpu, _baseBoard, _bios, _mac);
                return _info;
            }
            catch (Exception ex)
            {
                logg.Info("错GetComputerInfo" + ex.ToString());
                CommonFunc.SendBug("错GetComputerInfo", "2", ex.ToString(), "LicenseCode", "GetComputerInfo");
                return "";
            }
        }
        private static string GetBIOSInfo() //获取bios信息
        {
            string _info = string.Empty;
            _info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return _info;
        }
        private static string GetCPUInfo()//获取cpu信息
        {
            string _info = string.Empty;
            _info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return _info;
        }
        private static string GetBaseBoardInfo()//获取主板信息
        {
            string _info = string.Empty;
            _info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return _info;
        }
        private static string GetMACInfo()//获取mac信息
        {
            string _info = string.Empty;
            _info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return _info;
        }
        private static string GetHardWareInfo(string typePath, string key) //获取开机信息
        {
            try
            {
                ManagementClass _managementClass = new ManagementClass(typePath);
                ManagementObjectCollection _mn = _managementClass.GetInstances();
                PropertyDataCollection _properties = _managementClass.Properties;
                foreach (PropertyData _property in _properties)
                {
                    if (_property.Name == key)
                    {
                        foreach (ManagementObject m in _mn)
                        {
                            return m.Properties[_property.Name].Value.ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logg.Info("错GetHardWareInfo" + ex.ToString());
                CommonFunc.SendBug("错GetHardWareInfo", "2", ex.ToString(), "LicenseCode", "GetHardWareInfo");
            }
            return string.Empty;
        }
    }
}
