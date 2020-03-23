using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static tengchao.CloseWindGetMsg;
using static tengchao.GetmsgProcessNeedFunc;
using static tengchao.PublicDefine;
using static tengchao.CallWin32Api;
using System.Diagnostics;
using Uploads;
namespace tengchao
{
    class SearhWindFunc
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        static systemsleep sl = new systemsleep();
        /// <summary>
        /// 点击菜单
        /// </summary>
        /// <returns></returns>
        public static bool TryClickCaidan()//点击菜单
        {
            bool _istrue = false;
            IntPtr _CaiDanHwnd = FindWindow(null, "选择菜单中选项");
            SetWindowPos(_CaiDanHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
            SetForegroundWindow(_CaiDanHwnd);
            if (_CaiDanHwnd != IntPtr.Zero)
            {
                _istrue = true;
                bool _mouse_call = MouseClick.AddYanZhengClickTwo("", "ListBox_Class", _CaiDanHwnd, 120, 100, 2000);
            }
            else
            {
                CommonFunc.CommonSleep("ClickCaiDan", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="maindHwnd2"></param>
        /// <returns></returns>
        public static bool TryGetCheLiangMsg(IntPtr maindHwnd2)//获取车辆信息
        {
            bool _istrue = false;
            if (maindHwnd2 != IntPtr.Zero)
            {
                _istrue = true;
                OpenWindGetMsg.TryClickCrm(maindHwnd2);
                OpenWindGetMsg.GetCrmMsg();
            }
            else
            {
                CommonFunc.CommonSleep("ClickCheLaingMsg", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 点击车辆
        /// </summary>
        /// <returns></returns>
        public static bool TryClickCheLiang(IntPtr bighwnd)//点击车辆
        {
            string _kehutitle = "";
            bool _istrue = false;
            IntPtr _MaindHwnds = FindWindow("KCMLMasterForm_32", null);
            if (_MaindHwnds != IntPtr.Zero) {
                string _WindowTitle = "";
                StringBuilder _title = new StringBuilder(102400);
                GetWindowText(_MaindHwnds, _title, _title.Capacity);//获取窗口标题
                if (_title.ToString().Length > 1)
                {
                    _WindowTitle = _title.ToString();
                }
                if (_WindowTitle.Contains("客户及车辆"))
                {
                    _kehutitle = _WindowTitle;
                }
            }
            if (_kehutitle.Contains("客户及车辆"))
            {
                if (!GlobalSystemStop)
                {
                    IntPtr _MaindHwnd1 = FindWindow(null, _kehutitle);
                    SetWindowPos(_MaindHwnd1, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_MaindHwnd1);
                    if (_MaindHwnd1 != IntPtr.Zero)
                    {
                        if (!GlobalSystemStop)
                        {
                            logg.Info("开始查找车辆按钮。。。");
                            enumwindow(_MaindHwnd1, ConstCheLiangTag);
                            global_liuchegtag = 4;
                            DefineVar.ZebraInt = global_liuchegtag;
                            _istrue = true;
                        }
                    }
                    else
                    {
                        CommonFunc.CommonSleep("ClickCheLiang", sl.Moment);
                        if (!GlobalSystemStop)
                        {
                            IntPtr _MaindHwnd2 = FindWindow(null, _kehutitle);
                            SetWindowPos(_MaindHwnd2, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                            SetForegroundWindow(_MaindHwnd2);
                            if (_MaindHwnd2 != IntPtr.Zero)
                            {
                                logg.Info("开始查找车辆按钮。。。");
                                CommonFunc.CommonSleep("ClickCheLiang", sl.Little);
                                enumwindow(_MaindHwnd1, ConstCheLiangTag);
                                global_liuchegtag = 4;
                                DefineVar.ZebraInt = global_liuchegtag;
                                _istrue = true;
                            }
                            else
                            {
                                if (!GlobalSystemStop)
                                {
                                    AppareWiplockPage();
                                }
                                global_liuchegtag = 4;
                                _istrue = true;
                            }
                        }
                    }
                }
            }
            else {
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, 0, 0);
                CommonFunc.CommonSleep("CloseFuJia", 10);
                OpenWindGetMsg.keybd_event(Keys.Escape, 0, KEYEVENTF_KEYUP, 0);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// wip锁定页面，找到取消按钮
        /// </summary>
        public static void AppareWiplockPage()
        {
            CommonFunc.WaitFindWind(() => TryClickLockWip());
        }
        /// <summary>
        /// 点击锁定的wip号
        /// </summary>
        /// <returns></returns>
        public static bool TryClickLockWip()//点击锁定wip号
        {
            bool _istrue = false;
            IntPtr _MaindHwnd11 = FindWindow("KCMLMasterForm_32", null);
            if (_MaindHwnd11 != IntPtr.Zero)
            {
                enumwindow(_MaindHwnd11, ConstGlobalSuoDIngWipTag);
                _istrue = true;
            }
            else
            {
                CommonFunc.CommonSleep("AppareWiplockPage", 500);
                _istrue = false;
            }
            return _istrue;
        }
        /// <summary>
        /// 出现输入确认页面，点击退出，并回退到第一步
        /// </summary>
        /// <returns></returns>
        public static bool KaiDanClick()
        {
            bool _mouse_call;
            bool _is_true = true;
            try
            {
                CommonFunc.CommonSleep("KaiDanClick", sl.Little);
                int _zebracount = 0;
                while (_zebracount < 3)
                {
                    _zebracount++;
                    IntPtr _KaiDanHwnd = FindWindow(null, "输入确认");
                    SetWindowPos(_KaiDanHwnd, HWND_TOPMOST, 1, 1, 1, 1, SWP_NOMOVE | SWP_NOSIZE);
                    SetForegroundWindow(_KaiDanHwnd);
                    if (_KaiDanHwnd != IntPtr.Zero)
                    {
                        _mouse_call = MouseClick.AddYanZhengClickTwo("只显示", "KCMLButton_32", _KaiDanHwnd, 150, 300, 500);
                        _is_true = true;
                        break;
                    }
                    else
                    {
                        _is_true = false;
                    }
                }
                return _is_true;
            }
            catch (Exception e)
            {
                logg.Debug("CheLiangTag点击列表后出错，记录，此时应该重头开始。。。报错信息是：" + e.ToString());
                CommonFunc.SendBug("CheLiangTag点击列表后出错", "2", e.ToString(), "get_msg", "KaiDanClick");
                return false;
            }
        }
    }
}
