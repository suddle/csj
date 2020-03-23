using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using static tengchao.PublicDefine;

namespace tengchao
{
    class InfoSql
    {
       /// <summary>
       /// 判断相邻两个wip号间隔的大小
       /// </summary>
       /// <returns></returns>
        public static List<string> AdjustContinue()
        {
            List<string> info_list = new List<string>();
            string is_continue = "true";
            int between_min = 0;
            int between_max = 0;
            string sql = "select cast(A.wip_num as int) ,cast(B.wip_num as int), (cast(A.wip_num as int) - cast(B.wip_num as int)) from datatb A left join data B on A.id=B.id+1";
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
                            int diff;
                            int.TryParse(reader[2].ToString(), out diff);
                            int intdiff = System.Math.Abs(diff);
                            if (intdiff > 10000)
                            {
                                is_continue = "false";
                                int _temp_min;
                                int _temp_max;
                                int.TryParse(reader[0].ToString(), out _temp_min);
                                int.TryParse(reader[1].ToString(), out _temp_max);
                                if (_temp_max > _temp_min)
                                {
                                    between_min = _temp_min;
                                    between_max = _temp_max;
                                }
                                else
                                {
                                    between_min = _temp_max;
                                    between_max = _temp_min;
                                }
                                break;
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Error("判断相邻间隔差值出错。。。");
                }
            }
            info_list.Add(is_continue);
            info_list.Add(between_min.ToString());
            info_list.Add(between_max.ToString());
            myConnnect.Close();
            myConnnect.Dispose();
            return info_list;
        }
        /// <summary>
        /// 根据时间获取最大wip号
        /// </summary>
        /// <param name="searchtime">时间</param>
        /// <returns></returns>
        public static int GetDataMaxWip(string searchtime)
        {
            int FillInfoWipCount = 0;
            string sqls = "SELECT max(cast(wip_num as int)) FROM datatb where first_ruku_time like '%" + searchtime + "%'";
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
        /// 根据时间获取最小wip号
        /// </summary>
        /// <param name="searchtime"></param>
        /// <returns></returns>
        public static int GetDataMinWip(string searchtime)
        {
            int FillInfoWipCount = 0;
            string sql = "SELECT min(cast(wip_num as int)) FROM datatb";
            string sqls = "SELECT max(cast(wip_num as int)) FROM datatb where first_ruku_time like '%" + searchtime + "%'";
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
        /// 检测data表中有人员名字匹配不上的情况，此时说明出现了人员变动
        /// </summary>
        /// <returns></returns>
        /// <summary>
        public static List<string> MonitorTbPerson()
        {
            List<string> change_list = new List<string>();
            string day = DateTime.Now.ToString("dd");
            string sql = "select xiulipersondaihao from datatb where songxiutime_day = '" + day + "' and chepaihao != '' and xiulirenyuan = ''";
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
                            if (!change_list.Contains(reader[0].ToString()))
                            {
                                change_list.Add(reader[0].ToString());
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                }
                myConnnect.Close();
                myConnnect.Dispose();
                return change_list;
            }
        }
        /// 获取weixiuperson表中人员代号存在的数量
        /// </summary>
        /// <param name="ids">人员id</param>
        /// <returns></returns>
        public static int SearchRepairMsg(string ids)
        {
            int repairmsg = 0;
            string sql = "select count(*) from weixiupersontb where id = '" + ids + "'";
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
                            int.TryParse(reader[0].ToString(), out repairmsg);
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
            return repairmsg;
        }
        /// <summary>
        /// 从weixiuperson表获取操作员的名字，既此时登录dms人员的名字
        /// </summary>
        /// <param name="operate_userid">登录人员的id</param>

        public static void GetOperateUsename(string operate_userid)
        {
            string sql = "select name from weixiupersontb where id = '" + operate_userid + "'";
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
                            PublicDefine.GlobalOperateUse = reader[0].ToString();
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

        }
        /// <summary>
        /// 从data表获取个数 
        /// </summary>
        /// <param name="wip">wip号</param>
        /// <returns></returns>
        //从history表根据时间和wip查询个数
        public static int GetCountFromDatatb(string wip) {
            string day30ago = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            string tomorrows = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string sql = "SELECT count(*) FROM datatb WHERE first_ruku_time>'" + day30ago + "' and first_ruku_time <'" + tomorrows + "' and wip_num='"+wip+"'";
            int wipnum = 0;
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
                            int.TryParse(reader[0].ToString(), out wipnum);
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
            return wipnum;
        }
        /// <summary>
        /// 从历史表获取wip号
        /// </summary>
        /// <returns></returns>
        public static List<int> GetWipFromHistory()
        {
            int wipnum = 0;
            List<int> wiplist = new List<int>();
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string lastdays = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + "00:00:00";
            string sqls = "select wip_num from historywiptb where dat like '%" + today + "%'";
            string sql = "SELECT wip_num FROM historywiptb WHERE dat >= '" + lastdays + "' AND dat <= '" + today + "' or wipdat >= '" + lastdays + "' AND wipdat <= '" + today + "'";
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
                            int.TryParse(reader[0].ToString(), out wipnum);
                            wiplist.Add(wipnum);
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
            return wiplist;
        }
    }
}
