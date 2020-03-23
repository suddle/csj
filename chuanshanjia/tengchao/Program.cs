using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using tengchao;

namespace MyApplication

{
    static class Program
    {
        [STAThread]
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int SW_SHOWNOMAL = 1;


        static void Main()
        {
            Process process = RunningInstance();
            if (process != null)
            {
                HandleRunningInstance(process);
            }
            else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWind());
            }
        }
        private static void HandleRunningInstance(Process instance)// 显示已运行的程序。
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);   
            SetForegroundWindow(instance.MainWindowHandle);
        }
        public static Process RunningInstance()// 获取正在运行的实例，没有运行的实例返回null;
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (Process process in Processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
            throw new NotImplementedException();
        }

    }
}