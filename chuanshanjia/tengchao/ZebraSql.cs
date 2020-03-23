using System;
using System.Collections.Generic;
using System.IO;
using static tengchao.PublicDefine;
using MySql.Data.MySqlClient;
namespace tengchao
{
    class ZebraSql
    {
        /// <summary>
        /// 删除data表和upload表
        /// </summary>
        /// <param name="daynum">天数</param>
        public static void DeleteDataAndUploadManyDay(int daynum)// 删除data表和upload表 daynum天 以前的数据
        {
            string tb_name1 = "datatb";
            string tb_name2 = "uploadrecordtb";
            string one_day_date = DateTime.Now.AddDays(-daynum).ToString("dd");
            string one_day_date1 = DateTime.Now.AddDays(-daynum).ToString("yyyy-MM-dd");
            string data_sql = "delete from " + tb_name1 + " where first_ruku_time like '" + one_day_date1 + "%'";
            string upload_sql = "delete from " + tb_name2 + " where dat like '" + one_day_date1 + "%'";
            OperateSql.PublicSql(data_sql);
            OperateSql.PublicSql(upload_sql);
        }
        /// <summary>
        /// 保留20天的数据，此方法只针对log表 和 缺失wip号表
        /// </summary>
        /// <param name="day"></param>
        public static void DeleteTableAndLogSomeDay(int day)
        {
            string today = DateTime.Now.AddDays(-day).ToString("yyyy-MM-dd");
            string sql1 = "delete from queshitb where dat like '%"+ today + "%'";
            OperateSql.PublicSql(sql1);
        }
        /// <summary>
        /// 判断此wip记录是否可插入1，还是可更新2，还是不可操作3
        /// </summary>
        /// <param name="wip">传入的wip号</param>
        /// <returns></returns>
        public static int GetWipRecordStatus(string wip)
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            int b = 1;
            string sql = "select wip_num,is_full from datatb where datatb.wip_num = '" + wip + "'";
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
                            if (reader[1].ToString().Equals("true"))
                            {
                                b = 3;
                                break;
                            }
                            else if (reader[1].ToString().Equals("false"))
                            {
                                b = 2;
                                break;
                            }
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

            return b;
        }
        /// <summary>
        /// 判断wip的抓取内容是否完整 
        /// </summary>
        /// <param name="wip_num">wip号</param>
        /// <returns></returns>
        public static bool IsFullInData(string wip_num,string first_ruku_time)
        {
            bool is_true = true;
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "";
            sql = "select is_full from datatb WHERE wip_num='" + wip_num + "' and first_ruku_time like '%"+ first_ruku_time + "%'";
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
                            if (reader[0].ToString().Equals("true"))
                            {
                                is_true = true;
                            }
                            else
                            {
                                is_true = false;
                            }
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

            return is_true;
        }
        /// <summary>
        /// 删除缺失表里面的内容
        /// </summary>
        /// <param name="queshilist">缺失wip号列表</param>
        public static void DeletePicQueshi(List<string> queshilist)
        {
            string str = "";
            string today = DateTime.Now.ToString("yyyyMMdd");
            string sql = "";
            try
            {

                foreach (string sts in queshilist)
                {
                    if (str.Length == 0)
                    {
                        str = "'" + sts + "'";
                    }
                    else
                    {
                        str = str + ",'" + sts + "'";
                    }
                }
                if (str.Length == 0)
                {
                    return;
                }
                else
                {
                    str = "(" + str + ")";
                }
                sql = "delete from  queshitb where queshi in " + str + " and dat like '%"+today+"%'";
                OperateSql.PublicSql(sql);
            }
            catch (Exception ex)
            {
                logg.Info("删除出错" + sql);
                CommonFunc.SendBug("删除出错", "2", ex.ToString(), "operatesql", "DeletePicQueshi");
            }
        }
        /// <summary>
        /// 获取上传数量通过日期
        /// </summary>
        /// <param name="startdate">开始date</param>
        /// <param name="enddate">结束date</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static int get_uploadnum_between_startdate_and_enddate(string startdate, string enddate, string type)
        {
            int getnum_bysql = 0;
            string sql = "SELECT count(*) FROM uploadrecordtb WHERE op_dat >= '" + startdate + "' AND op_dat <= '" + enddate + "' AND jinchangorchuchang = '" + type + "'";
            //string sql = "select count(*) from uploadrecordtb where SUBSTR(dat,0 ,11) between date('" + startdate + "') and date('" + enddate + "') and jinchangorchuchang = " + type;
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
                            logg.Info("sql--->" + reader[0].ToString());
                            int.TryParse(reader[0].ToString(), out getnum_bysql);
                        }
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    logg.Error("error: " + e);
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return getnum_bysql;
        }
        /// <summary>
        /// 获取信息  从data表
        /// </summary>
        /// <param name="is_full">是否完整</param>
        /// <returns></returns>
        public static List<string> GetCaptureLogInfo(string is_full)
        {
            List<string> info_list = new List<string>();
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string day = DateTime.Now.ToString("dd");
            string time = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "select * from datatb where first_ruku_time like '%" + time + "%' and is_full = '" + is_full + "'";
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
                            string ordertime = reader[25].ToString() + "-" + reader[26].ToString() + "-" + reader[27].ToString() + " " + reader[28].ToString() + ":" + reader[29].ToString();
                            string info = reader[32].ToString() + "@" + reader[1].ToString() + "@" + reader[15].ToString()
                                + "@" + reader[9].ToString() + "@" + reader[11].ToString() + "@" + reader[5].ToString() + "@" + reader[4].ToString()
                                + "@" + reader[13].ToString() + "@" + reader[14].ToString() + "@" + reader[16].ToString().Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "") + "@" + reader[18].ToString()
                                + "@" + reader[20].ToString() + "@" + reader[19].ToString() + "@" + reader[22].ToString() + "@" + reader[23].ToString()
                                + "@" + reader[21].ToString() + "@" + ordertime + "@" + reader[33].ToString() + "@" + reader[30].ToString();
                            info_list.Add(info);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info(ex.ToString());
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return info_list;
        }
        /// <summary>
        /// 获取上传信息
        /// </summary>
        /// <returns></returns>
        public static List<string> GetUploadLogInfo()
        {
            List<string> info_list = new List<string>();
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "select * from uploadrecordtb where op_dat like '" + today + "%' ";
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
                            string jinchangorchuchang = "";
                            if (reader[5].ToString().Equals("0"))
                            {
                                jinchangorchuchang = "进场";
                            }
                            else if (reader[5].ToString().Equals("1"))
                            {
                                jinchangorchuchang = "出场";
                            }
                            string info = reader[1].ToString() + "@"   + reader[4].ToString() + "@" + reader[2].ToString() + "@" + reader[3].ToString().Replace("@", "");
                            info_list.Add(info);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    logg.Info(ex.ToString());
                }
            }
            myConnnect.Close();
            myConnnect.Dispose();
            return info_list;
        }
    }
}
