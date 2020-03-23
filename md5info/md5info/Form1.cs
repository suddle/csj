using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace md5info
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)//32
        {
            //65742D828BC97F879742F6E5482463
            string md5_string = this.textBox1.Text;
            string md5time = this.textBox2.Text + "_00-00-00";
            string allend = take_data(md5_string, md5time, "32");
            this.textBox3.Text = allend;
        }
        private void button2_Click(object sender, EventArgs e)//16
        {
            //8bc97f0879742f6e
            string md5_string = this.textBox1.Text;
            string md5time = this.textBox2.Text + "_00-00-00";
            string allend = take_data(md5_string, md5time, "16");
            this.textBox3.Text = allend;
        }
        public string take_data(string md5_string, string md5time, string wei)
        {
            int count = 0;
            if (wei.Contains("32"))
            {
                count = 2;
            }
            else
            {
                count = 1;
            }
            string allend = "";
            string unicode_time = StringToUnicode(md5time);
            string[] sArray = unicode_time.Split('\\');
            string[] arrays = new string[md5_string.Length];
            for (int j = 0; j < md5_string.Length / count; j++)
            {
                arrays[j] = md5_string.Substring(j * count, count);
                allend += arrays[j] + sArray[j + 1];
            }
            return allend;
        }
        public static string GetComputerInfo()//获取电脑信息
        {
            try
            {
                string info = string.Empty;
                string cpu = GetCPUInfo();
                string baseBoard = GetBaseBoardInfo();
                string bios = GetBIOSInfo();
                string mac = GetMACInfo();
                info = string.Concat(cpu, baseBoard, bios, mac);
                return info;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private static string GetBIOSInfo() //获取bios信息
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return info;
        }
        private static string GetCPUInfo()//获取cpu信息
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return info;
        }
        private static string GetBaseBoardInfo()//获取主板信息
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }
        private static string GetMACInfo()//获取mac信息
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }
        private static string GetHardWareInfo(string typePath, string key) //获取开机信息
        {
            try
            {
                ManagementClass managementClass = new ManagementClass(typePath);
                ManagementObjectCollection mn = managementClass.GetInstances();
                PropertyDataCollection properties = managementClass.Properties;
                foreach (PropertyData property in properties)
                {
                    if (property.Name == key)
                    {
                        foreach (ManagementObject m in mn)
                        {
                            return m.Properties[property.Name].Value.ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return string.Empty;
        }
        public static string StringToUnicode(string s)
        {
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }

        
    }
}
