using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static tengchao.PublicDefine;
namespace tengchao
{
    class OperateSql
    {
        /// <summary>
        /// 更新weixiuperson表
        /// </summary>
        /// <param name="is_insert">更新类型</param>
        /// <param name="name_id">姓名id</param>
        /// <param name="type">职位类型</param>
        /// <param name="name">姓名</param>
        public static void ChangeRepairMsg(string is_insert, string name_id, string type, string name)
        {
            string sql = "";
            if (is_insert.Equals("insert"))
            {
                sql = "insert into weixiupersontb (id, type, name) values ('" + name_id + "', '" + type + "', '" + name + "')";
            }
            else
            {
                sql = "UPDATE weixiupersontb SET type='" + type + "', name='" + name + "'  WHERE id='" + name_id + "'";
            }
            PublicSql(sql);
        }
        /// <summary>
        /// 从weixiuperson表根据修理人员代号获取修理人员的名字
        /// </summary>
        /// <param name="xiulipersondaihao"></param>
        /// <returns></returns>
        public static string GetXiuLiPersonName(string xiulipersondaihao)
        {
            string xiuliperson_name = "";
            string sql = "select name from weixiupersontb where id = '" + xiulipersondaihao + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            xiuliperson_name = reader[0].ToString();
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return xiuliperson_name;
        }
        /// <summary>
        /// 获取上传成功出厂个数
        /// </summary>
        /// <param name="stat">状态</param>
        /// <returns></returns>
        public static int GetChuchangNum(string stat)
        {
            int chuchangnum = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string day = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-dd");
            string get_wipnum_sql = "select count(*) from uploadrecordtb where op_dat like '" + today + "%' and jinchangorchuchang = 1"; 
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(get_wipnum_sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int.TryParse(reader[0].ToString(), out chuchangnum);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    logg.Info("000GetChuchangNum出错" + e.ToString());
                    chuchangnum = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return chuchangnum;
        }
        /// <summary>
        /// 获取上传成功入厂个数
        /// </summary>
        /// <param name="stat">状态</param>
        /// <returns></returns>
        public static int GetRuChangNum(string stat)
        {
            int ruchangnum = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string day = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-dd");
            string get_wipnum_sql = "select count(*) from uploadrecordtb where op_dat like '" + today + "%' and jinchangorchuchang = 0";
            string sql = "select count(distinct wip_num) from uploadrecordtb where stat = '" + stat + "' and jinchangorchuchang = '0' and dat like '" + today + "%'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(get_wipnum_sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int.TryParse(reader[0].ToString(), out ruchangnum);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    logg.Info("000GetChuchangNum出错" + e.ToString());
                    ruchangnum = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return ruchangnum;
        }
        /// <summary>
        /// 获取某个wip在upload记录表中的个数
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>
        public static int GetUploadRecordWipCount(string wip_num)
        {
            int FillInfoWipCount = 0;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select count(*) from uploadrecordtb where uploadrecordtb.wip_num = '" + wip_num + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out FillInfoWipCount);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("GetUploadRecordWipCount出错" + ex.ToString());
                    CommonFunc.SendBug("GetUploadRecordWipCount出错", "2", ex.ToString(), "operatesql", "GetUploadRecordWipCount");
                    FillInfoWipCount = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return FillInfoWipCount;
        }
        /// <summary>
        /// 获取数量从data表 
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>
        public static int GetDataWipAndHistoryCount(string wip_num)
        {
            int FillInfoWipCount = 0;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select count(*) from datatb where wip_num = '" + wip_num + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out FillInfoWipCount);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("GetDataWipCount出错" + ex.ToString());
                    CommonFunc.SendBug("GetDataWipCount出错", "2", ex.ToString(), "operatesql", "GetDataWipCount");
                    FillInfoWipCount = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return FillInfoWipCount;
        }

        /// <summary>
        /// 从data表中获取wip个数
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>\
        public static int GetDataWipCount(string wip_num)
        {
            int FillInfoWipCount = 0;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select count(*) from datatb where wip_num = '" + wip_num + "' and is_full = 'true'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out FillInfoWipCount);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("GetDataWipCount出错" + ex.ToString());
                    CommonFunc.SendBug("GetDataWipCount出错", "2", ex.ToString(), "operatesql", "GetDataWipCount");
                    FillInfoWipCount = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return FillInfoWipCount;
        }
        // 保存抓取、上传数据到数据库
        /// <summary>
        /// 保存内容到data
        /// </summary>
        /// <param name="tp"></param>
        public static void InsertRecordData(int tp)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-dd");
            string sql = "";
            if (tp == 1)
            {//插入
                sql = "insert into datatb (id, chepaihao,chepaihao1,chepaiqianzhui,haopaizhonglei,cheliangleixing,xingbie,chepaihouzhui,cheshenyanse1, cheshenyanse, cheliangxinghao1, cheliangxinghao,zhongwenpinpai1, zhongwenpinpai, chejiahao, kehu, hujidizhi, shoujihaoma1, shoujihaoma, zhengjianhaoma, zhengjianleixing, xiulicontent, xiulipersondaihao, xiulirenyuan, gongsiname, songxiutime_year, songxiutime_month, songxiutime_day, songxiutime_hour, songxiutime_minute, orderstate, shifouquche,wip_num,zebradatatype,is_full,color, first_ruku_time) values (null, '" + GlobalChePaiHao + "', '" + GlobalChePaiHao1 + "','" + GlobalChePaiQianZhui + "','" + GlobalHaoPaiZhongLei + "','" + GlobalCheLiangLeiXing + "','" + GlobalXingBie + "','" + GlobalChePaiHouZhui + "', '" + GlobalCheShenYanSe1 + "', '" + GlobalCheShenYanSe + "', '" + GlobalCheLiangXingHao1 + "', '" + GlobalCheLiangXingHao + "', '" + GlobalZhongWenPinPai1 + "', '" + GlobalZhongWenPinPai + "', '" + GlobalCheJiaHao + "', '" + GlobalKehu + "', '" + GlobalHuJiDiZhi + "', '" + GlobalShouIiHaoMa1 + "', '" + GlobalShouIiHaoMa + "', '" + GlobalZhengJianHaoMa + "', '" + GlobalZhengJianLeiXing + "', '" + GlobalXiuLiContent + "', '" + GlobalXiuLiPersonDaiHao + "', '" + GlobalXiuLiRenYuan + "', '" + GlobalGongSiName + "', '" + GlobalSongXiuTimeYear + "', '" + GlobalSongXiuTimeMonth + "', '" + GlobalSongXiuTimeDay + "', '" + GlobalSongXiuTimeHour + "', '" + GlobalSongXiuTimeMinute + "', '" + GlobalOrderState + "', '" + GlobalShiFouQuChe.ToString() + "', '" + GlobalWipNum + "','" + GlobalZebraDataType + "','" + GlobalIsFull + "','颜色','" + time + "')";
                logg.Info("add-params-test-sql.." + sql);
                PublicSql(sql);
            }
            else if (tp == 2)
            {//根据wip更新
                sql = "update datatb set chepaihao='" + GlobalChePaiHao + "', chepaihao1='" + GlobalChePaiHao1 + "',chepaiqianzhui='" + GlobalChePaiQianZhui + "',haopaizhonglei='" + GlobalHaoPaiZhongLei + "',cheliangleixing='" + GlobalCheLiangLeiXing + "',xingbie='" + GlobalXingBie + "', chepaihouzhui='" + GlobalChePaiHouZhui + "',cheshenyanse1='" + GlobalCheShenYanSe1 + "', cheshenyanse='" + GlobalCheShenYanSe + "',cheliangxinghao1='" + GlobalCheLiangXingHao1 + "', cheliangxinghao='" + GlobalCheLiangXingHao + "',zhongwenpinpai1='" + GlobalZhongWenPinPai1 + "', zhongwenpinpai='" + GlobalZhongWenPinPai + "', chejiahao='" + GlobalCheJiaHao + "', kehu='" + GlobalKehu + "', hujidizhi='" + GlobalHuJiDiZhi + "', shoujihaoma1='" + GlobalShouIiHaoMa1 + "', shoujihaoma='" + GlobalShouIiHaoMa + "', zhengjianhaoma='" + GlobalZhengJianHaoMa + "', zhengjianleixing='" + GlobalZhengJianLeiXing + "', xiulicontent='" + GlobalXiuLiContent + "', xiulipersondaihao='" + GlobalXiuLiPersonDaiHao + "',  xiulirenyuan='" + GlobalXiuLiRenYuan + "',  gongsiname='" + GlobalGongSiName + "', songxiutime_year='" + GlobalSongXiuTimeYear + "', songxiutime_month='" + GlobalSongXiuTimeMonth + "', songxiutime_day='" + GlobalSongXiuTimeDay + "', songxiutime_hour='" + GlobalSongXiuTimeHour + "', songxiutime_minute='" + GlobalSongXiuTimeMinute + "', orderstate='" + GlobalOrderState + "', shifouquche='" + GlobalShiFouQuChe.ToString() + "',zebradatatype='" + GlobalZebraDataType + "',is_full='" + GlobalIsFull + "',first_ruku_time='" + time + "' where wip_num='" + GlobalWipNum + "'";
                logg.Info("add-params-test-sql111.." + sql);
                PublicSql(sql);
            }
        }
        /// <summary>
        /// 从data表中根据is_full判断的成功状态进行返回日志
        /// </summary>
        /// <param name="type">抓取类型</param>
        /// <returns></returns>
        public static string GetLogFromData(string type)//获取数据类容  通过'抓取成功...'
        {
            string txt = "";
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select * from datatb where is_full = '" + type + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            txt += "" + reader["wip_num"].ToString() + "|" + reader["chepaihao"].ToString() + "|" + reader["chepaihao1"].ToString() + "|" + reader["cheshenyanse1"].ToString() +
                    "|" + reader["cheshenyanse"].ToString() + "|" + reader["cheliangxinghao1"].ToString() + "|" +
                    reader["cheliangxinghao"].ToString() + "|" + reader["zhongwenpinpai1"].ToString() + "|" + reader["zhongwenpinpai"].ToString() +
                    "|" + reader["chejiahao"].ToString() + "|" + reader["kehu"].ToString() + "|" + reader["hujidizhi"].ToString() + "|" +
                    reader["shoujihaoma1"].ToString() + "|" + reader["shoujihaoma"].ToString() + "|" + reader["zhengjianhaoma"].ToString() + "|" +
                    reader["zhengjianleixing"].ToString() + "|" + reader["xiulicontent"].ToString() + "|" + reader["xiulipersondaihao"].ToString() + "|" +
                    reader["xiulirenyuan"].ToString() + "|" + reader["gongsiname"].ToString() + "|" + reader["songxiutime_year"].ToString() + "|" +
                    reader["songxiutime_month"].ToString() + "|" + reader["songxiutime_day"].ToString() + "|" + reader["songxiutime_hour"].ToString() +
                    "|" + reader["songxiutime_minute"].ToString() + "|" + reader["orderstate"].ToString() + "|" + reader["shifouquche"].ToString() + "|" +
                    reader["zebradatatype"].ToString() + "|" + reader["is_full"].ToString() + "\r\n";
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("GetLogFromData出错了:" + ex.ToString());
                    CommonFunc.SendBug("GetLogFromData出错了", "2", ex.ToString(), "operatesql", "GetLogFromData");
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return txt;
        }
        /// <summary>
        /// 根据类型查找数据库中符合条件的总条数
        /// </summary>
        /// <param name="type">分类</param>  // 例如“抓取成功”
        /// <returns></returns>
        public static int CountsFromSqlite(string type)//获取数量  通过传入“抓取成功。。。。”
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("yyyy-MM-dd");
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string day = DateTime.Now.ToString("dd");
            int ct = 0;
            string sql = "select count(*) from datatb where first_ruku_time like '%" + time + "%' and is_full = '" + type + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out ct);
                        }
                    }
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                ct = 0;
                logg.Info("CountsFromSqlite错" + ex.ToString());
                CommonFunc.SendBug("CountsFromSqlite错", "2", ex.ToString(), "operatesql", "CountsFromSqlite");
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return ct;
        }
        /// <summary>
        /// 获取维修内容
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<string>> GetWeixiuDic()
        {
            Dictionary<int, List<string>> myDictionary = new Dictionary<int, List<string>>();
            string sql = "select * from weixiupersontb";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            count++;
                            List<string> weixiulist = new List<string>();
                            weixiulist.Add(reader[0].ToString());
                            weixiulist.Add(reader[1].ToString());
                            weixiulist.Add(reader[2].ToString());
                            if (weixiulist.Count > 0)
                            {
                                myDictionary.Add(count, weixiulist);
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return myDictionary;
        }
        /// <summary>
        /// 获取维修数量
        /// </summary>
        /// <param name="wip_num"></param>
        /// <returns></returns>
        public static int GetWeiXiuCount(string wip_num)
        {
            int FillInfoWipCount = 0;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select count(*) from weixiupersontb where id = '" + wip_num + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out FillInfoWipCount);
                        }
                    }
                    reader.Close();
                }
                catch
                {
                    FillInfoWipCount = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return FillInfoWipCount;
        }
        /// <summary>
        /// 获取wip号从历史表
        /// </summary>
        /// <returns></returns>
        public static bool AdjustHistoryWip()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            bool _exist = false;
            string sql = "select wip_num from historywiptb where dat like '%" + today + "%'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            logg.Info(reader[0].ToString());
                            if (!reader[0].ToString().Equals(string.Empty))
                            {
                                _exist = true;
                                break;
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("判斷是否存在历史工单時出错。。。");
                    _exist = false;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return _exist;
        }
        /// <summary>
        /// 查找时间为0000/00/00 的wip 从新定义事件
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>
        public static string GetQueShiTimeWip(string wip_num)
        {
            MessageBox.Show(DateTime.Now.ToString());
            string geshitime = "";
            string sql = "SELECT songxiutime_year,songxiutime_month,songxiutime_day FROM datatb WHERE wip_num='" + wip_num + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            geshitime = reader[0].ToString() + "/" + reader[1].ToString() + "/" + reader[2].ToString() + " " + "00:00:00";
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    geshitime = DateTime.Now.ToString();
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return geshitime;
        }
        /// <summary>
        /// 判断是否是日期
        /// </summary>
        /// <param name="StrSource">日期内容</param>
        /// <returns></returns>
        public static bool IsDate(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }
        /// <summary>
        /// 保存历史内容
        /// </summary>
        /// <param name="Wipnum">wip号</param>
        /// <param name="wipdat">wip日期</param>
        public static void SaveHistoryWipnum(string Wipnum, string wipdat)
        {
            logg.Info("SaveHistoryWipnum");
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "";
            wipdat = wipdat.Replace('/','-');
            bool isdate = IsDate(wipdat);
            if (isdate)
            {
                if (wipdat.Contains("0000") || string.IsNullOrEmpty(wipdat))
                {
                    string newdat = GetQueShiTimeWip(Wipnum);
                    string sqld = "insert ignore into historywiptb(wip_num, dat,wipdat) values( '" + Wipnum + "', '" + today + "','" + newdat + "')";
                    string msg = sqld + "//" + Wipnum + "//" + wipdat + "csj1";
                    logg.Info(msg);
                }
                else
                {
                    sql = "insert ignore into historywiptb(wip_num, dat,wipdat) values( '" + Wipnum + "', '" + today + "','" + wipdat + "')";
                    string msg = sql + "//" + Wipnum + "//" + wipdat + "csj2";
                    logg.Info(msg);
                }

                PublicSql(sql);
            }
            else {
                sql = "insert ignore into historywiptb(wip_num, dat,wipdat) values( '" + Wipnum + "', '" + today + "','" + wipdat + "')";
                string msg = sql + "//" + Wipnum + "//" + wipdat + "csj3";
                logg.Info(msg);
            }
        }
        /// <summary>
        /// 获取最小的wip号
        /// </summary>
        /// <param name="searchtime">日期</param>
        /// <returns></returns>
        public static string GetMinHistoryWip(string searchtime)
        {
            string _wipnum = string.Empty;
            string sql = "SELECT wip_num FROM historywiptb ORDER BY wip_num LIMIT 0,1 ";
            string sqls = "SELECT max(cast(wip_num as int)) FROM historywiptb where dat like '%" + searchtime + "%'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sqls, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader[0].Equals(string.Empty))
                            {
                                _wipnum = reader[0].ToString();
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("獲取最小历史工单數據出错。。。");
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return _wipnum;
        }
        /// <summary>
        /// 获取所有的历史wip号
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllHistoryWip()
        {
            List<string> _HisWipList = new List<string>();
            string sql = "SELECT wip_num FROM historywiptb";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader[0].Equals(string.Empty))
                            {
                                _HisWipList.Add(reader[0].ToString());
                            }
                        }
                    }
                    reader.Close();
                }
                catch
                {
                    logg.Info("獲取所有历史工单數據出错。。。");
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return _HisWipList;
        }
        /// <summary>
        /// 通过wip号  删除一些数据
        /// </summary>
        /// <param name="Wipnum"></param>
        public static void DeleteHistoryWip(string Wipnum)
        {
            string sql = "delete from historywiptb where wip_num = '" + Wipnum + "'";
            PublicSql(sql);
        }
        /// <summary>
        /// 公共mysql函数
        /// </summary>
        /// <param name="sql"></param>
        public static void PublicSql(string sql)
        {
            try
            {
                MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
                using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
                {
                    cmd.ExecuteNonQuery();
                }
                myConnnect.Close();
                myConnnect.Dispose();
            }
            catch
            {
                logg.Info("公共函数出错，错误内容" + sql);
            }
        }
        /// <summary>
        /// 保存旧的wip号
        /// </summary>
        /// <param name="Wipnum"></param>
        /// <param name="isnew"></param>
        public static void SaveOldWipnum(string Wipnum, string isnew)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "insert into incompleteworktb(wip_num,isnew, dat) values( '" + Wipnum + "', '" + isnew + "','" + today + "')";
            PublicSql(sql);
        }
        /// <summary>
        /// 更新wip号
        /// </summary>
        /// <param name="Wipnum"></param>
        public static void UpdateOldWipnum(string Wipnum)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "UPDATE incompleteworktb SET dat='" + today + "' WHERE wip_num='" + Wipnum + "'";
            PublicSql(sql);
        }
        /// <summary>
        /// 删除wip号
        /// </summary>
        /// <param name="Wipnum"></param>
        public static void DeleteOldWipnum(string Wipnum)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = "delete from incompleteworktb WHERE wip_num='" + Wipnum + "'";
            PublicSql(sql);
        }
        /// <summary>
        /// 查询旧wip号
        /// </summary>
        /// <param name="isnew"></param>
        /// <returns></returns>
        public static string oldwip(string isnew)
        {
            string sql = "select wip_num from incompleteworktb WHERE isnew='" + isnew + "' order by dat desc limit 1";
            string oldwip = "";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader[0].ToString().Equals(string.Empty))
                            {
                                oldwip = reader[0].ToString();
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info("获取老的wip号出错。。。");
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return oldwip;
        }
        /// <summary>
        /// 获取不完整的wip号
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>
        public static int GetOldWipCount(string wip_num)
        {
            int FillInfoWipCount = 0;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "select count(*) from incompleteworktb where wip_num = '" + wip_num + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Int32.TryParse(reader[0].ToString(), out FillInfoWipCount);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    FillInfoWipCount = 0;
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return FillInfoWipCount;
        }
        /// <summary>
        /// 设置dms信息
        /// </summary>
        /// <param name="keys">配置表的名称</param>
        /// <param name="val">配置表内容</param>
        public static void SetDmsInfo(string keys, string val)
        {
            string times = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string sql = "insert into configuretb(configure_key,configure_value,updatetime) VALUES('" + keys + "','" + val + "','" + times + "') ON DUPLICATE KEY UPDATE configure_value='" + val + "',updatetime='" + times + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sql, myConnnect))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
        }
        /// <summary>
        /// 
        /// 更新历史信息
        /// </summary>
        public static void  Updatehistory() {
            string sql1 = "update historywiptb set wipdat = (select concat_ws('-',songxiutime_year,songxiutime_month,songxiutime_day) from datatb where datatb.wip_num = historywiptb.wip_num) where wipdat LIKE '0000-00-00 00:00:00'";
            PublicSql(sql1);
        }
        /// <summary>
        /// 获取dms信息
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string GetDmsInfo(string keys)
        {
            string enddafta = "";
            //string sql = "select xiulipersondaihao from datatb where songxiutime_day = '" + day + "' and chepaihao != '' and xiulirenyuan = ''";
            string sqls = "select configure_value from configuretb where configure_key='" + keys + "'";
            MySqlConnection myConnnect = GlobalConnectMysql.getFreeConnectItem().mySqlConn;
            using (MySqlCommand cmd = new MySqlCommand(sqls, myConnnect))
            {
                try
                {
                    MySqlDataReader re = cmd.ExecuteReader();
                    if (re.HasRows)
                    {
                        while (re.Read())
                        {
                            //读取三列数据
                            enddafta = re[0].ToString();
                        }
                    }
                    re.Close();
                }
                catch (Exception ex)
                {
                    enddafta = "";
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return enddafta;
        }
    }
}
