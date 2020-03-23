using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace tengchao
{
    public class MySqlConnectionPool
    {
        private readonly string sqlConnect = string.Empty;
        public MySqlConnectionPool(string Connection)
        {
            sqlConnect = Connection;

            //定时器轮询连接，清理不在使用的连接
            var timer = new Timer();
            timer.Enabled = true;
            timer.Elapsed += (a, b) =>
            {
                //轮询连接池连接,删除满足条件的连接
                delwithConnectPool("remove");
                Console.WriteLine("连接数:" + getCount());

            };
            timer.Interval = 1000 * 10; //10分钟一次
            timer.AutoReset = true;//一直执行
        }

        private static List<ConnectionItem> listConnects = new List<ConnectionItem>();


        private static readonly object obj_getConnects = new object();
        public Tuple<bool, ConnectionItem> delwithConnectPool(string type)
        {
            //保证并发条件下集合增删改查时的数据唯一性
            lock (obj_getConnects)
            {
                bool result = false;
                ConnectionItem result_item = null;
                switch (type)
                {
                    case "get":
                        var connectItem = listConnects.Where(u => u.ifBusy == false).FirstOrDefault();

                        if (connectItem == null)
                        {
                            listConnects.Add(result_item = getInstance(sqlConnect));
                        }
                        else
                        {
                            if (connectItem.mySqlConn.State == System.Data.ConnectionState.Open)
                            {
                                connectItem.setBusy(true);
                                connectItem.updateTime(DateTime.Now);
                                result_item = connectItem;
                            }
                            else
                            {
                                listConnects.Add(result_item = getInstance(sqlConnect));
                            }
                        }

                        break;
                    case "remove":
                        if (listConnects != null && listConnects.Any())
                        {
                            //删除移除 超过10分钟未使用的的连接，使用两分钟的未释放连接，连接状态已关闭的连接
                            var listOuteTimes = listConnects.Where(u => (u.ifBusy == true && (DateTime.Now - u.time).TotalSeconds > 120) || ((DateTime.Now - u.time).TotalSeconds > 60 * 10) || (u.mySqlConn.State != System.Data.ConnectionState.Open));
                            foreach (var item in listOuteTimes)
                            {
                                item.mySqlConn.Close();
                                item.mySqlConn.Dispose();//释放 
                            }
                            //超时连接移除
                            listConnects.RemoveAll(u => (u.ifBusy == true && (DateTime.Now - u.time).TotalSeconds > 120) || ((DateTime.Now - u.time).TotalSeconds > 60 * 10) || (u.mySqlConn.State != System.Data.ConnectionState.Open));
                        }
                        break;
                }
                return new Tuple<bool, ConnectionItem>(result, result_item);
            }

        }


        public ConnectionItem getInstance(string connect)
        {

            var item = new ConnectionItem()
            {

                ifBusy = true,
                time = DateTime.Now,
                mySqlConn = new MySqlConnection(connect)
            };
            item.mySqlConn.Open();
            return item;


        }


        //获取一个空闲连接
        public ConnectionItem getFreeConnectItem()
        {
            return delwithConnectPool("get").Item2;
        }

        public int getCount()
        {
            return listConnects.Count;
        }

    }
    public class ConnectionItem : IDisposable
    {
        public DateTime time { get; set; }

        public MySqlConnection mySqlConn { get; set; }

        public bool ifBusy { get; set; }//设置是否在使用
        public void setBusy(bool busy)
        {
            ifBusy = busy;
        }

        public void updateTime(DateTime dt)
        {
            time = dt;
        }

        public void Dispose()
        {
            ifBusy = false;
        }
    }
}
