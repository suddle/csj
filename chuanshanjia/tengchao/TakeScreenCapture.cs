using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static tengchao.CallWin32Api;
using static tengchao.PublicDefine;

namespace tengchao
{
    class TakeScreenCapture
    {
        static int _count = 0;
        /// <summary>
        /// 截图函数
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <param name="save_path">路径</param>
        /// <returns></returns>
        public static Bitmap GetWindowCapture(IntPtr hWnd, string save_path)//截图
        {
            try {
                if (!hWnd.Equals(IntPtr.Zero))
                {
                    IntPtr _hscrdc = GetWindowDC(hWnd);
                    RECT _windowRect = new RECT();
                    GetWindowRect(hWnd, ref _windowRect);
                    int width = _windowRect.Right - _windowRect.Left;
                    int height = _windowRect.Bottom - _windowRect.Top;
                    IntPtr _hbitmap = CreateCompatibleBitmap(_hscrdc, width, height);
                    IntPtr _hmemdc = CreateCompatibleDC(_hscrdc);
                    SelectObject(_hmemdc, _hbitmap);
                    PrintWindow(hWnd, _hmemdc, 0);
                    Bitmap _bmp = Bitmap.FromHbitmap(_hbitmap);
                    _bmp = Thresholding(_bmp);//二值化图片
                    _bmp.Save(save_path);
                    _bmp.Dispose();
                    DeleteDC(_hscrdc);//删除用过的对象
                    DeleteDC(_hmemdc);//删除用过的对象
                    DeleteObject(_hbitmap);//删除用过的对象
                    DeleteObject(_hmemdc);//删除用过的对象
                    DeleteObject(_hscrdc);//删除用过的对象
                    return _bmp;
                }
                else
                {
                    Bitmap bmp1 = new Bitmap(@"pic.png");
                    bmp1.Dispose();
                    return bmp1;
                }
            }
            catch (Exception exp) {
                _count++;
                var code = Marshal.GetLastWin32Error();
                if (_count < 2) {
                    CommonFunc.SendBug("GetWindowCapture错误" + code.ToString(), "2", exp.ToString(), "take_screen", "GetWindowCapture");
                }
                logg.Error(code.ToString() + exp.Message + exp.ToString());
                Bitmap bmp1 = new Bitmap(@"pic.png");
                bmp1.Dispose();
                return bmp1;
            }
        }
        /// <summary>
        /// 二值化 
        /// </summary>
        /// <param name="bmp">图片</param>
        /// <returns></returns>
        static Bitmap Thresholding(Bitmap bmp)//二值化图片
        {
            int[] histogram = new int[256];
            int minGrayValue = 255, maxGrayValue = 0;
            //求取直方图
            try
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixelColor = bmp.GetPixel(i, j);
                        histogram[pixelColor.R]++;
                        if (pixelColor.R > maxGrayValue) maxGrayValue = pixelColor.R;
                        if (pixelColor.R < minGrayValue) minGrayValue = pixelColor.R;
                    }
                }
                return bmp;
            }
            catch
            {
                return bmp;
            }
        }
        /// <summary>
        /// 灰度化图片
        /// </summary>
        /// <param name="img1">图片</param>
        public static void ToGrey(Bitmap img1)//灰度化图片
        {
            for (int i = 0; i < img1.Width; i++)
            {
                for (int j = 0; j < img1.Height; j++)
                {
                    Color pixelColor = img1.GetPixel(i, j);
                    //计算灰度值
                    int grey = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
                    Color newColor = Color.FromArgb(grey, grey, grey);
                    img1.SetPixel(i, j, newColor);
                }
            }
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
         string lpszDriver,         // driver name驱动名
         string lpszDevice,         // device name设备名
         string lpszOutput,         // not used; should be NULL
         IntPtr lpInitData   // optional printer data
         );
        [DllImport("gdi32.dll")]
        public static extern int BitBlt(
         IntPtr hdcDest, // handle to destination DC目标设备的句柄
         int nXDest,   // x-coord of destination upper-left corner目标对象的左上角的X坐标
         int nYDest,   // y-coord of destination upper-left corner目标对象的左上角的Y坐标
         int nWidth,   // width of destination rectangle目标对象的矩形宽度
         int nHeight, // height of destination rectangle目标对象的矩形长度
         IntPtr hdcSrc,   // handle to source DC源设备的句柄
         int nXSrc,    // x-coordinate of source upper-left corner源对象的左上角的X坐标
         int nYSrc,    // y-coordinate of source upper-left corner源对象的左上角的Y坐标
         UInt32 dwRop   // raster operation code光栅的操作值
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(
         IntPtr hdc // handle to DC
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(
         IntPtr hdc,         // handle to DC
         int nWidth,      // width of bitmap, in pixels
         int nHeight      // height of bitmap, in pixels
         );

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(
         IntPtr hdc,           // handle to DC
         IntPtr hgdiobj    // handle to object
         );

        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(
         IntPtr hdc           // handle to DC
         );

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
         IntPtr hwnd,                // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,              // HDC to print into,Handle to the device context.
         UInt32 nFlags               // Optional flags,Specifies the drawing options. It can be one of the following values.
         );

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// 全屏截图
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// 指定窗口截图
        /// </summary>
        /// <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>
        /// <returns></returns>

        public static void CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = GetWindowDC(handle);
            RECT windowRect = new RECT();
            GetWindowRect(handle, ref windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            IntPtr hdcDest = CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);

            IntPtr hOld = SelectObject(hdcDest, hBitmap);
            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, 0x00CC0020);

            Image objImage = new Bitmap(1600, 900);
            Graphics g = Graphics.FromImage(objImage);
            g.CopyFromScreen(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new Size(1600, 900));
            g.ReleaseHdc(hOld);

            objImage.Save("d:\\test.jpg");

        }
        /// <summary>
        /// 转换图片颜色
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string IndentifyImg(string path)
        {
            Image image = System.Drawing.Image.FromFile(path);
            Bitmap pbitmap = new Bitmap(image);
            int x1 = 17, y1 = 309;
            int x2 = 67, y2 = 538;
            int width = pbitmap.Width;//获取图片宽度
            int height = pbitmap.Height;//获取图片高度
            Bitmap newmap = new Bitmap(width, height);//保存新图片
            Color pixel;//颜色匹对
            ArrayList miny = new ArrayList();
            ArrayList maxy = new ArrayList();
            ArrayList tmp = new ArrayList();
            /*对于每个x值，获取背景色所在处的一组y值，y值的取法是对于每个x值取所有的特定rgb的所有y,
            然后取数组中的最小值和最大值放到miny和maxy列表。*/
            for (int i = x1; i < x2; i++)
            {
                tmp.Clear();
                for (int j = y1; j < y2; j++)
                {
                    pixel = pbitmap.GetPixel(i, j);//获取旧图片的颜色值（ARGB存储方式）
                    int r, g, b, a;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    a = pixel.A;
                    //白色RGB(255，255，255),黑色（0,0,0）
                    //Console.Out.WriteLine(r+ "+g+" "+b);
                    //判断是否属于白色背景
                    if (r == 0 && g == 120 && b == 215)
                    {
                        tmp.Add(j);
                    }
                }
                if (tmp.Count > 0)
                {
                    tmp.Sort();
                    miny.Add(tmp[0]);
                    maxy.Add(tmp[tmp.Count - 1]);
                }
            }
            //取出最小值组group后最多的值
            var res1 = from n in miny.ToArray()
                       group n by n into g
                       orderby g.Count() descending
                       select g;
            var gr = res1.First();
            int gr1 = 0;
            foreach (int x in gr)
            {
                Console.Write(" {0}", x);
                gr1 = x;
                break;
            }
            //取出最大值组group后最多的值
            var res2 = from n in maxy.ToArray()
                       group n by n into g
                       orderby g.Count() descending
                       select g;
            gr = res2.First();
            int gr2 = 0;
            foreach (int x in gr)
            {
                gr2 = x;
                break;
            }
            //对(x1,y1)(x2,y2)区域内的有底色的区域进行颜色变换，背景色换成白色，数字换成黑色。其它区域的颜色不变。
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    pixel = pbitmap.GetPixel(i, j);//获取旧图片的颜色值（ARGB存储方式）
                    int r, g, b, a;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    a = pixel.A;

                    if (j < gr1 || j > gr2)
                    {//非区域内完全不变
                        newmap.SetPixel(i, j, Color.FromArgb(r, g, b));
                        continue;
                    }

                    //白色RGB(255，255，255),黑色（0,0,0）

                    //判断是否属于白色背景
                    if (r == 255 && g == 255 && b == 255)
                    {
                        //设置新图片中指定像素的颜色为黑色
                        newmap.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                    else if (r == 0 && g == 0 && b == 0)
                    {   //设置新图片中指定像素的颜色为白色
                        newmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    else if (r == 51 && g == 94 && b == 168)
                    {   //设置新图片中指定像素的颜色为白色
                        newmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                    else
                    {//默认都设置成白色
                        newmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                    }
                }
            }
            string newpath = @"zebra.png";
            newmap.Save(newpath);
            newmap.Dispose();
            pbitmap.Dispose();
            image.Dispose();
            return newpath;
        }
        /// <summary>
        /// 判断传入得list中得wip是否是合法得wip号
        /// </summary>
        /// <param name="enddata"></param>
        /// <returns></returns>
        public static List<string> ScreenWipList(List<string> enddata)//判断传入得list中得wip是否是合法得wip号
        {
            List<string> tiplist = new List<string>();
            try
            {
                foreach (var txts in enddata)
                {
                    string result = System.Text.RegularExpressions.Regex.Replace(txts.Trim(), @"[^0-9]+", "");
                    const string pattern = "^[0-9]*$";
                    Regex rx = new Regex(pattern);
                    bool is_int = rx.IsMatch(result);
                    if (is_int)
                    {
                        int wipnum = 0;
                        Int32.TryParse(result, out wipnum);
                        if (result.Length > 4)
                        {
                            tiplist.Add(result.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logg.Info("识别图像错误:" + ex.Message);
                CommonFunc.SendBug("识别图像错误", "2", ex.ToString(), "commonfunc", "Recognize");
            }
            return tiplist;
        }
        /// <summary>
        /// 识别图片中的内容
        /// </summary>
        /// <param name="imgPath">路径</param>
        /// <param name="startX">x坐标</param>
        /// <param name="startY">y坐标</param>
        /// <param name="Width">宽</param>
        /// <param name="Height">高</param>
        /// <returns></returns>
        public static List<string> Recognize(string imgPath, int startX, int startY, int Width, int Height)
        {
            logg.Info("图片识别函数aaaaa");
            string _str = "0"; //默认为0
            List<string> _StringList = new List<string>();
            logg.Info("图片识别函数bbbbbb");
            try
            {
                logg.Info("图片识别函数cccc");
                _str = Marshal.PtrToStringAnsi(OCRpart(imgPath, -1, startX, startY, Width, Height));
                logg.Info(_str.ToString() + "图片识别函数dddddd");
                _str = _str.Trim();
                _str = _str.Replace('o', '0');//数字0 替换 o.
                _str = _str.Replace('O', '0');//数字0 替换 O.
                _str = _str.Replace('i', '1');//数字1 替换 i
                _str = _str.Replace('I', '1');//数字1 替换 I
                _str = _str.Replace('l', '1');
                _str = _str.Replace('!', '1');//数字1 替换 !
                _str = _str.Replace(" ", "");
                List<string> _StriParr = _str.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                _StriParr = _StriParr.Where(s => !string.IsNullOrEmpty(s)).ToList();
                return _StriParr;
            }
            catch (Exception ex)
            {
                logg.Info("识别图像错误:" + ex.Message);
                CommonFunc.SendBug("识别图像错误", "2", ex.ToString(), "commonfunc", "Recognize");
            }
            return _StringList;
        }

    }
}
