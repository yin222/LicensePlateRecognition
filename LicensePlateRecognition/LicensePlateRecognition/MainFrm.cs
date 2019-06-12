using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace LicensePlateRecognition
{
    public partial class MainFrm : CCWin.Skin_Mac
    {

        #region 字段/属性/实例对象
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);
        Stopwatch sw = new Stopwatch();
        private string imgPath = "img";
        private string logPath = "home/test-imgs/No99mresults.txt";
        Color backColor = Color.FromArgb(240, 240, 240);

        private string imgName = "";
        private Bitmap extract_Bitmap_one;
        private Bitmap extract_Bitmap_two;

        private Bitmap z_Bitmap1;
        private Bitmap[] z_Bitmaptwo = new Bitmap[7];
        private Bitmap objNewPic;

        int[] gray = new int[256];
        int[] rr = new int[256];
        int[] gg = new int[256];
        int[] bb = new int[256];

        private int cHeight;
        private int cWidth;
        float[,] m = new float[5000, 5000];

        private Bitmap[] charFont;
        private Bitmap[] provinceFont;

        string[] charString;//存储的路径
        string[] provinceString;//省份字体
        string[] charDigitalString;
        string[] provinceDigitalString;

        private DateTime dtBegin;
        private DateTime dtEnd;

        /// <summary>
        /// 车牌图像
        /// </summary>
        private Bitmap c_Bitmap { get; set; }
        /// <summary>
        /// 保存打开的图片
        /// </summary>
        private Bitmap newBmp { get; set; }
        /// <summary>
        /// 永远是彩色图像
        /// </summary>
        private Bitmap colorBmp { get; set; }
        /// <summary>
        /// 保存处理后的图片
        /// </summary>
        private Bitmap newHandleBmp { get; set; }

        public static string charSourceBath = "dataSource\\char\\";
        public static string provinceSourceBath = "dataSource\\font\\";

        #endregion

        #region 程序初始化
        public MainFrm()
        {
            InitializeComponent();

            this.BackColor = beginBtn.BackColor = openImageBtn.BackColor = txtResult.BackColor = backColor;
            DirectoryInfo imgDir = new DirectoryInfo(imgPath);
            if (!imgDir.Exists)
            {
                imgDir.Create();
            }

            if (!File.Exists(logPath))
            {
                //创建目录
                DirectoryInfo dir = new DirectoryInfo("home/test-imgs");
                dir.Create();
                //创建文件
                File.Create(logPath);
            }
        }
        private void MainFrm_Load(object sender, EventArgs e)
        {
            txtResult.Text = "-欢迎使用车牌识别系统";
            beginBtn.Enabled = btnBegin2.Enabled = false;
            beginBtn.BackColor = btnBegin2.BackColor = Color.Red;
        }
        #endregion

        #region 窗体拖动
        System.Drawing.Point mouseOf;
        bool leftFlag;
        private void MainFrm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOf = new System.Drawing.Point(-e.X, -e.Y);
                leftFlag = true;
            }
        }

        private void MainFrm_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                System.Drawing.Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOf.X, mouseOf.Y);
                Location = mouseSet;
            }
        }

        private void MainFrm_MouseUp(object sender, MouseEventArgs e)
        {
            leftFlag = false;
        }
        #endregion

        #region 车牌识别流程
        /// <summary>
        /// 打开图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openImageBtn_Click(object sender, EventArgs e)
        {
            //open对话框实例化
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp|所有合适文件(*.bmp/*.jpg)|*.bmp;*.jpg";
            open.FilterIndex = 2;
            //该值指示对话框在关闭前是否还原当前目录
            open.RestoreDirectory = true;
            if (open.ShowDialog() == DialogResult.OK)
            {
                imgName = open.FileName.Split('\\')[open.FileName.Split('\\').Length - 1];
                string bmpHandlePath = open.FileName;

                if (File.Exists(imgPath))
                    File.Delete(imgPath);
                imgPath = @"img/imgHandle.jpg";

                CompressImage(bmpHandlePath, imgPath);

                Bitmap bmp = new Bitmap(imgPath);
                Bitmap bmpHandle = new Bitmap(bmp.Width, bmp.Height);

                Graphics draw = Graphics.FromImage(bmpHandle);
                draw.DrawImage(bmp, 0, 0);
                //克隆
                colorBmp = newBmp = bmpHandle;/*bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.DontCare);*/
                LicensePlateImg.Image = newBmp;
                beginBtn.Enabled = btnBegin2.Enabled = true;
                beginBtn.BackColor = btnBegin2.BackColor = openImageBtn.BackColor;
                bmp.Dispose();
                draw.Dispose();
            }
            else
            {
                beginBtn.Enabled = btnBegin2.Enabled = false;
                beginBtn.BackColor = btnBegin2.BackColor = Color.Red;
            }
        }
        /// <summary>
        /// 处理流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void beginBtn_Click(object sender, EventArgs e)
        {
            sw.Start();//开始计时
            txtResult.Text += "\r\n-开始识别...";

            new Thread(()=> {
                #region 图像灰度化

                ImgToGray(newBmp);

                #endregion

                #region 直方图均衡化

                Equalization(newHandleBmp);

                #endregion

                #region 中值滤波

                ColorfulBitmapMedianFilterFunction();

                #endregion

                #region 边缘检测

                EdgeDetection();

                #endregion

                #region 车牌定位
                this.c_Bitmap = Recoginzation.licensePlateLocation(newHandleBmp, colorBmp, m);//

                extract_Bitmap_one = c_Bitmap.Clone(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height),
                    PixelFormat.DontCare);
                #endregion

                #region 车牌灰度化

                CarImgToGray();
                #endregion

                #region 车牌二值化

                ConvertTo1Bpp(c_Bitmap);
                #endregion

                #region 字符切割

                WordSplit();

                #endregion

                #region 字符识别

                Result();

                #endregion

                sw.Stop();//停止计时
                if (txtResult.InvokeRequired)
                {
                    Action<string> actionDelegate = (x) => {
                        WriteResult();
                        this.txtResult.Text += x + "\r\n耗时：" + sw.Elapsed;
                    };
                    this.txtResult.Invoke(actionDelegate, "");
                }
            }).Start();
          
        }
        /// <summary>
        /// 处理流程2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBegin2_Click(object sender, EventArgs e)
        {
            if (IsConnectInternet())
            {
                sw.Start();
                Type type = typeof(BaiduTool);
                MethodInfo method = type.GetMethod("LicensePlateDemo");
                if (method != null)
                {
                    string result = method.Invoke(type, new object[] { imgPath }).ToString();
                    sw.Stop();
                    this.txtResult.Text = "\r\n-识别结果:" + result+"\r\n耗时："+sw.Elapsed;
                    WriteResult();
                    //if (!Directory.Exists("log"))
                    //{
                    //    DirectoryInfo dir = new DirectoryInfo("log");
                    //    dir.Create();
                    //}
                    //string newPath = "log/handle.txt";
                    //if (!File.Exists(newPath))
                    //    File.Create(newPath);
                }
            }
            else
            {
                MessageBox.Show("请确认您的网络是否连接");
            }
        }
        #endregion

        #region 处理代码函数封装
        /// <summary>
        /// 将识别结果写入文件
        /// </summary>
        private void WriteResult()
        {
            using (FileStream stream = new FileStream(logPath, FileMode.Append))
            {
                using (StreamWriter write = new StreamWriter(stream))
                {
                    write.WriteLine(imgName + "\r\n" + this.txtResult.Text.Split(':')[1]);
                }
            }
        }
        /// <summary>
        /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败 
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
        }
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="originalFileFullName">原图片地址</param>
        /// <param name="afterConversionFileFullName">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="isFirst">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(string originalFileFullName, string afterConversionFileFullName, int flag = 90, int size = 300, bool isFirst = true)
        {
            Image iSource = Image.FromFile(originalFileFullName);
            ImageFormat tFormat = iSource.RawFormat;
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(originalFileFullName);
            if (isFirst == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(afterConversionFileFullName, true);
                return true;
            }

            int dHeight = iSource.Height / 2;
            int dWidth = iSource.Width / 2;
            int sW = 0, sH = 0;
            //按比例缩放
            System.Drawing.Size tem_size = new System.Drawing.Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(afterConversionFileFullName, jpegICIinfo, ep);
                    FileInfo fi = new FileInfo(afterConversionFileFullName);
                    if (fi.Length > 1024 * size)
                    {
                        flag = flag - 10;
                        CompressImage(originalFileFullName, afterConversionFileFullName, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(afterConversionFileFullName, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
        /// <summary>
        /// 车牌灰度化
        /// </summary>
        private void CarImgToGray()
        {
            if (c_Bitmap != null)
            {
                int tt = 0;
                for (int i = 0; i < 256; i++)
                {
                    gray[i] = 0;
                    rr[i] = 0;
                    gg[i] = 0;
                    bb[i] = 0;
                }
                BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride; //获取或设置Bitmap对象的跨距宽度（也称为扫描宽度）
                System.IntPtr Scan0 = bmData.Scan0; //获取或设置第一个像素数据的地址
                unsafe //"生成"选择“允许不安全代码”
                {
                    byte* p = (byte*)(void*)Scan0;
                    //这里stride是图片一行数据的长度， 
                    //sitide - m_Bitmap.Width*3 是获取一行元素中多余的部分
                    //因为每个m_Bitmap.Width获取一行的像素，乘三是因为每个像素由rgb三个值组成。
                    //参考https://www.cnblogs.com/zkwarrior/p/5665216.html
                    int nOffset = stride - c_Bitmap.Width * 3;
                    byte red, green, blue;
                    int nWidth = c_Bitmap.Width;
                    int nHeight = c_Bitmap.Height;
                    cWidth = c_Bitmap.Width;
                    cHeight = c_Bitmap.Height;
                    for (int y = 0; y < nHeight; y++)
                    {
                        for (int x = 0; x < nWidth; x++)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            //加权平均值法
                            tt = p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                            //最大值法
                            //byte max = red > green ? red : green;
                            //max = max > blue ? max : blue;
                            //tt = p[0] = p[1] = p[2] = max;

                            //平均值法
                            //tt = p[0] = p[1] = p[2] = (byte)((red+green+blue)/3);

                            rr[red]++;
                            gg[green]++;
                            bb[blue]++;
                            gray[tt]++; //统计灰度值tt的像素点数目
                            p += 3;
                        }
                        //加上多余的部分，跳到下一行的开头
                        p += nOffset;
                    }
                }
                c_Bitmap.UnlockBits(bmData);
            }
        }
        /// <summary>
        /// 字符切割
        /// </summary>
        private void WordSplit()
        {
            BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - c_Bitmap.Width * 3;

                int nWidth = c_Bitmap.Width;
                int nHeight = c_Bitmap.Height;
                int[] countHeight = new int[nHeight];
                int[] countWidth = new int[nWidth];
                int Yheight = nHeight, YBottom = 0;
                //这一步没有必要，因为默认的初值就是0
                //for (int i = 0; i < nHeight; i++)
                //{
                //    countHeight[i] = 0;
                //}
                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if ((p[0] == 0 && p[3] == 255) || (p[0] == 255 && p[3] == 0))
                        {
                            countHeight[y]++;
                        }
                        p += 3;
                    }
                    p += nOffset;
                }
                //计算车牌号的上边缘
                for (int y = nHeight / 2; y > 0; y--)
                {
                    if (countHeight[y] >= 16 && countHeight[(y + 1) % nHeight] >= 12)
                    {
                        if (Yheight > y)
                        {
                            Yheight = y;
                        }
                        if ((Yheight - y) == 1)
                        {
                            Yheight = y - 3;
                        }

                    }
                }

                //计算车牌号的下边缘
                for (int y = nHeight / 2; y < nHeight; y++)
                {
                    if (countHeight[y] >= 12 && countHeight[(y + 1) % nHeight] >= 12)
                    {
                        if (YBottom < y)
                        {
                            YBottom = y;
                        }
                        if ((y - Yheight) == 1)
                        {
                            YBottom = y + 3;
                        }

                    }
                }
                YBottom += 1;
                byte* p1 = (byte*)(void*)Scan0;

                p1 += stride * (Yheight - 1);   //跳到车牌的顶部
                for (int y = Yheight; y < YBottom; ++y)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (p1[0] == 255)
                        {
                            countWidth[x]++;
                        }
                        p1 += 3;
                    }
                    p1 += nOffset;
                }
                int contg = 0, contd = 0, countRightEdge = 0, countLeftEdge = 0,
                    Y1 = nWidth, Yr = 0;
                int[] XLeft = new int[20];
                int[] xRight = new int[20];
                //这里不需要初始化，所以省去了。
                for (int y = 0; y < YBottom; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (countWidth[(x + 1) % nWidth] < y && countWidth[x] >= y &&
                            countWidth[Math.Abs((x - 1) % nWidth)] >= y && contg >= 2)
                        {
                            if (countRightEdge == 6)
                            {
                                Yr = x;
                            }
                            if ((countRightEdge == 2 && (x >= XLeft[2] && XLeft[2] > 0)))
                            {
                                xRight[countRightEdge] = x;
                                countRightEdge++;
                                contd = 0;
                            }
                            else
                            {
                                if ((countRightEdge != 2))
                                {
                                    if (countRightEdge == 0 && contg < 4)
                                    {
                                        XLeft[0] = 0;
                                        countLeftEdge = 0;
                                    }
                                    if ((x >= XLeft[0] && XLeft[0] > 0))
                                    {
                                        xRight[countRightEdge] = x;
                                        countRightEdge++;
                                        contd = 0;
                                    }
                                }
                            }
                        }
                        if (countWidth[Math.Abs((x - 1) % nWidth)] < y &&
                            countWidth[x] >= y && countWidth[(x + 1) % nWidth] >= y && contd >= 2)
                        {
                            if (countLeftEdge == 0 && countWidth[(x + 2) % nWidth] >= y)
                            {
                                Y1 = x;
                            }
                            if ((countLeftEdge == 2 && contd > 5))
                            {
                                XLeft[countLeftEdge] = x;
                                countLeftEdge++;
                            }
                            else
                            {
                                if ((countLeftEdge != 2))
                                {
                                    XLeft[countLeftEdge] = x;
                                    countLeftEdge++;
                                    contg = 0;
                                    if (countLeftEdge == 0 && countWidth[(x + 2) % nWidth] < y)
                                    {
                                        XLeft[0] = 0;
                                        countLeftEdge = 0;
                                    }
                                }
                            }
                        }
                        contg++;
                        contd++;
                    }
                    if (countRightEdge + countLeftEdge >= 14)
                    {
                        break;
                    }
                    countRightEdge = 0;
                    countLeftEdge = 0;
                    for (int i = 0; i < xRight.Length; i++)
                    {
                        xRight[i] = 0;
                    }
                    for (int i = 0; i < XLeft.Length; i++)
                    {
                        XLeft[i] = 0;
                    }
                }
                //字符切割
                c_Bitmap.UnlockBits(bmData);
                if ((YBottom - Yheight) > 1 && (Yr - Y1) > 1)
                {
                    Rectangle sourceRectangle = new Rectangle(Y1, Yheight, Yr - Y1, YBottom - Yheight);
                    extract_Bitmap_two = extract_Bitmap_one.Clone(sourceRectangle, PixelFormat.DontCare);
                    BitmapData bmData2 = extract_Bitmap_two.LockBits(new Rectangle(0, 0, extract_Bitmap_two.Width,
                        extract_Bitmap_two.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    int stride2 = bmData2.Stride;
                    System.IntPtr Scan02 = bmData2.Scan0;
                    byte* p2 = (byte*)(void*)Scan02;
                    int nOffset2 = stride2 - extract_Bitmap_two.Width * 3;

                    int nWidth2 = extract_Bitmap_two.Width;
                    int nHeight2 = extract_Bitmap_two.Height;
                    for (int y = 0; y < nHeight2; y++)
                    {
                        for (int x = 0; x < nWidth2; x++)
                        {
                            if (x == (xRight[0] - Y1) || x == (XLeft[0] - Y1) ||
                                x == (xRight[1] - Y1) || x == (XLeft[1] - Y1) ||
                                x == (xRight[2] - Y1) || x == (XLeft[2] - Y1) ||
                                x == (xRight[3] - Y1) || x == (XLeft[3] - Y1) ||
                                x == (xRight[4] - Y1) || x == (XLeft[4] - Y1) ||
                                x == (xRight[5] - Y1) || x == (XLeft[5] - Y1) ||
                                x == (xRight[6] - Y1) || x == (XLeft[6] - Y1) ||
                                x == (xRight[7] - Y1) || x == (XLeft[7] - Y1))
                            {
                                if (x != 0)
                                {
                                    p2[2] = 255;
                                    p2[0] = p2[1] = 0;
                                }
                            }
                            p2 += 3;

                        }
                        p2 += nOffset2;
                    }
                    extract_Bitmap_two.UnlockBits(bmData2);
                    IntPtr ip = extract_Bitmap_two.GetHbitmap(); //将Bitmap转换为BitmapSource
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging
                        .CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    for (int i = 0; i < 7; i++)
                    {
                        if ((YBottom - Yheight) > 1 && (xRight[i] - XLeft[i]) > 1)
                        {
                            Rectangle sourceRectangle2 = new Rectangle(XLeft[i], Yheight,
                                xRight[i] - XLeft[i], YBottom - Yheight);
                            z_Bitmap1 = extract_Bitmap_one.Clone(sourceRectangle2, PixelFormat.DontCare);
                            z_Bitmaptwo[i] = c_Bitmap.Clone(sourceRectangle2, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[i], 9, 16);
                            z_Bitmaptwo[i] = objNewPic;
                            ip = z_Bitmaptwo[i].GetHbitmap();
                            bitmapSource = System.Windows.Interop.Imaging
                            .CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 字符识别
        /// </summary>
        private void Result()
        {
            //字符识别
            int charBmpCount = this.TransformFiles(charSourceBath);
            int provinceBmpCount = this.TransformFiles(provinceSourceBath);
            int[] charMatch = new int[charBmpCount];
            int[] provinceMatch = new int[provinceBmpCount];

            charFont = new Bitmap[charBmpCount];
            provinceFont = new Bitmap[provinceBmpCount];

            //初始化省去

            for (int i = 0; i < charBmpCount; i++)
            {
                charFont[i] = (Bitmap)Bitmap.FromFile(charString[i], false);
            }
            for (int i = 0; i < provinceBmpCount; i++)
            {
                provinceFont[i] = (Bitmap)Bitmap.FromFile(provinceString[i], false);
            }
            int matchIndex = 0;
            string[] digitalFont = new string[7];
            unsafe
            {
                if (z_Bitmaptwo[0] != null)
                {
                    BitmapData bmData = z_Bitmaptwo[0].LockBits(new Rectangle(0, 0, z_Bitmaptwo[0].Width, z_Bitmaptwo[0].Height),
                  ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    int stride = bmData.Stride;
                    System.IntPtr Scan = bmData.Scan0;
                    int nOffset = stride - z_Bitmaptwo[0].Width * 3;
                    int nWidth = z_Bitmaptwo[0].Width;
                    int nHeight = z_Bitmaptwo[0].Height;
                    int lv, lc = 30;
                    for (int i = 0; i < provinceBmpCount; i++)
                    {
                        byte* p = (byte*)(void*)Scan;
                        BitmapData bmData1 = provinceFont[i].LockBits(new Rectangle(0, 0, provinceFont[i].Width, provinceFont[i].Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        int stride1 = bmData1.Stride;
                        System.IntPtr Scan1 = bmData1.Scan0;
                        byte* p1 = (byte*)(void*)Scan1;
                        int nOffset1 = stride1 - provinceFont[i].Width * 3;
                        int nWidth1 = provinceFont[i].Width;
                        int nHeight1 = provinceFont[i].Height;
                        int ccc0 = 0, ccc1 = 0;
                        lv = 0;
                        for (int y = 0; y < nHeight; y++)
                        {
                            for (int x = 0; x < nWidth; x++)
                            {
                                if ((p[0] - p1[0] != 0))
                                {
                                    provinceMatch[i]++;
                                }
                                p1 += 3;
                                p += 3;
                            }
                            p1 += nOffset;
                            p += nOffset;
                        }
                        //这里返回的是最小值
                        //需要的是最小值的下标
                        matchIndex = MinNumber(provinceMatch);

                        //provinceMatch.Min();
                        digitalFont[0] = provinceDigitalString[matchIndex].Substring(0, 1);
                        provinceFont[i].UnlockBits(bmData1);
                    }

                    z_Bitmaptwo[0].UnlockBits(bmData);
                }
                if (z_Bitmaptwo[1] != null && z_Bitmaptwo[2] != null &&
                    z_Bitmaptwo[3] != null && z_Bitmaptwo[4] != null &&
                    z_Bitmaptwo[5] != null && z_Bitmaptwo[6] != null)
                {
                    for (int j = 1; j < 7; j++)
                    {
                        BitmapData bmData = z_Bitmaptwo[j].LockBits(new Rectangle(0, 0, z_Bitmaptwo[j].Width, z_Bitmaptwo[j].Height),
             ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        int stride = bmData.Stride;
                        System.IntPtr Scan = bmData.Scan0;
                        int nOffset = stride - z_Bitmaptwo[j].Width * 3;
                        int nWidth = z_Bitmaptwo[j].Width;
                        int nHeight = z_Bitmaptwo[j].Height;
                        int lv, lc = 0;
                        for (int i = 0; i < charBmpCount; i++)
                        {
                            charMatch[i] = 0;
                        }
                        for (int i = 0; i < charBmpCount; i++)
                        {
                            byte* p = (byte*)(void*)Scan;
                            BitmapData bmData1 = charFont[i].LockBits(new Rectangle(0, 0, charFont[i].Width, charFont[i].Height),
               ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                            int stride1 = bmData1.Stride;
                            System.IntPtr Scan1 = bmData1.Scan0;
                            byte* p1 = (byte*)(void*)Scan1;
                            int nOffset1 = stride1 - charFont[i].Width * 3;
                            int nWidth1 = charFont[i].Width;
                            int nHeight1 = charFont[i].Height;
                            int ccc0 = 0, ccc1 = 0;
                            lv = 0;
                            for (int y = 0; y < nHeight; y++)
                            {
                                for (int x = 0; x < nWidth; x++)
                                {
                                    if ((p[0] - p1[0] != 0))
                                    {
                                        charMatch[i]++;
                                    }
                                    lv++;
                                    p1 += 3;
                                    p += 3;
                                }
                                p1 += nOffset;
                                p += nOffset;
                            }
                            //这里返回的是最小值
                            matchIndex = MinNumber(charMatch);

                            //charMatch
                            //provinceMatch.Min();
                            digitalFont[j] = charDigitalString[matchIndex].Substring(0, 1);
                            charFont[i].UnlockBits(bmData1);

                        }
                        z_Bitmaptwo[j].UnlockBits(bmData);
                    }

                }
            }
            string result = "";
            for (int i = 0; i < digitalFont.Length; i++)
            {
                result += digitalFont[i];
            }

            if (txtResult.InvokeRequired)
            {
                Action<string> actionDelegate = (x) => { this.txtResult.Text = x.ToString(); };
                this.txtResult.Invoke(actionDelegate, "\r\n-识别结果:" + result);
            }
            else
            {
                this.txtResult.Text += "\r\n-识别结果:" + result;
            }
        }
        /// <summary>
        /// 中值滤波
        /// </summary>
        private void ColorfulBitmapMedianFilterFunction()
        {
            BitmapData bmData = newHandleBmp.LockBits(new Rectangle(0, 0, newHandleBmp.Width, newHandleBmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int i = 0; i < 256; i++)
            {
                gray[i] = 0;
            }
            unsafe
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                byte* p = (byte*)(void*)Scan0;
                byte* pp;
                int tt;
                int nOffset = stride - newHandleBmp.Width * 3;
                int nWidth = newHandleBmp.Width;
                int nHeight = newHandleBmp.Height;

                long sum = 0;
                int[,] gaussianMatrix = { { 1, 2, 3, 2, 1 }, { 2, 4, 6, 4, 2 }, { 3, 6, 7, 6, 3 }, { 2, 4, 6, 4, 2 }, { 1, 2, 3, 2, 1 } };//高斯滤波器所选的n=5模板
                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {

                        if (!(x <= 1 || x >= nWidth - 2 || y <= 1 || y >= nHeight - 2))
                        {
                            pp = p;
                            sum = 0;
                            int dividend = 79;
                            for (int i = -2; i <= 2; i++)
                                for (int j = -2; j <= 2; j++)
                                {
                                    pp += (j * 3 + stride * i);
                                    sum += pp[0] * gaussianMatrix[i + 2, j + 2];
                                    if (i == 0 && j == 0)
                                    {
                                        if (pp[0] > 240)//如果模板中心的灰度大于240
                                        {
                                            sum += p[0] * 30;
                                            dividend += 30;
                                        }
                                        else if (pp[0] > 230)
                                        {
                                            sum += pp[0] * 20;
                                            dividend += 20;
                                        }
                                        else if (pp[0] > 220)
                                        {
                                            sum += p[0] * 15;
                                            dividend += 15;
                                        }
                                        else if (pp[0] > 210)
                                        {
                                            sum += pp[0] * 10;
                                            dividend += 10;
                                        }
                                        else if (p[0] > 200)
                                        {
                                            sum += pp[0] * 5;
                                            dividend += 5;
                                        }
                                    }
                                    pp = p;
                                }
                            sum = sum / dividend;
                            if (sum > 255)
                            {
                                sum = 255;
                            }

                            //高斯滤波
                            p[0] = p[1] = p[2] = (byte)(sum);

                            //中值滤波
                            //3*3
                            //byte[] zzP = new byte[9];
                            //zzP[0] = p[-stride - 3];
                            //zzP[1] = p[-stride];
                            //zzP[2] = p[-stride + 3];
                            //zzP[3] = p[-3];
                            //zzP[4] = p[0];
                            //zzP[5] = p[3];
                            //zzP[6] = p[stride - 3];
                            //zzP[7] = p[stride];
                            //zzP[8] = p[stride + 3];
                            //Array.Sort(zzP);
                            //p[0] = p[1] = p[2] = zzP[4];


                            //均值滤波
                            //p[0] = p[1] = p[2] = (byte)((p[-stride] + p[-stride - 3] + p[-stride + 3]
                            //    + p[0] + p[3] + p[-3] +
                            //    p[stride] + p[stride - 3] + p[stride + 3]) / 9);
                        }
                        tt = p[0];
                        gray[tt]++;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            newHandleBmp.UnlockBits(bmData);
        }
        /// <summary>
        /// 字符图像归一化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int TransformFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.bmp");
            int i = 0, j = 0;
            try
            {
                foreach (FileInfo f in files)
                {
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            if (path.Equals(charSourceBath))
            {
                this.charString = new string[i];
                this.charDigitalString = new string[i];
                try
                {
                    foreach (FileInfo f in files)
                    {
                        charString[j] = (dir + f.ToString());
                        charDigitalString[j] =
                            Path.GetFileNameWithoutExtension(charString[j]);
                        j++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    throw;
                }
            }
            else
            {
                provinceString = new string[i];
                provinceDigitalString = new string[i];
                try
                {
                    foreach (FileInfo f in files)
                    {
                        provinceString[j] = (dir + f.ToString());
                        provinceDigitalString[j] =
                            Path.GetFileNameWithoutExtension(provinceString[j]);
                        j++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    throw;
                }
            }
            return i;
        }
        /// <summary>
        /// 获取最小值得下标
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private int MinNumber(int[] promatch)
        {
            int min = promatch.Min();
            int n = 0;
            for (int i = 0; i < promatch.Length; i++)
            {
                if (promatch[i] == min)
                {
                    n = i;
                }
            }
            return n;
        }
        /// <summary>
        /// 车牌二值化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private void ConvertTo1Bpp(Bitmap bmp)
        {
            int Mr = 0;
            long sum = 0;
            int count = 0;
            //像素个数与灰度等级的乘积除以像素个数
            //应该是为了求整个颜色灰度的平均值，以便区分黑白
            for (int i = 0; i < 256; i++)
            {
                sum += gray[i] * i;
                count += gray[i];
            }
            Mr = (int)(sum / count);
            int sum1 = 0;
            int count1 = 0;
            for (int i = 0; i < Mr; i++)
            {
                sum1 += gray[i] * i;
                count1 += gray[i];
            }
            int g1 = sum1 / count1;

            int sum2 = 0;
            int count2 = 0;
            for (int i = Mr; i < 255; i++)
            {
                sum2 += gray[i] * i;
                count2 += gray[i];
            }
            int g2 = sum2 / count2;

            //求阈值
            int va;
            if (count1 < count2)
            {
                //白底黑字
                va = Mr - count1 / count2 * Math.Abs(g1 - Mr);
            }
            else
            {
                va = Mr + count2 / count1 * Math.Abs(g2 - Mr);
            }

            BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width,
                c_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - c_Bitmap.Width * 3;

                int nWidth = c_Bitmap.Width;
                int nHeight = c_Bitmap.Height;

                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (p[0] > va)
                        {
                            p[0] = p[1] = p[2] = 255;
                        }
                        else
                        {
                            p[0] = p[1] = p[2] = 0;
                        }
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            c_Bitmap.UnlockBits(bmData);
        }
        /// <summary>
        /// 边缘检测
        /// </summary>
        /// <returns></returns>
        private void EdgeDetection()
        {
            // sobel边缘检测
            Rectangle rect = new Rectangle(0, 0, newHandleBmp.Width, newHandleBmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = newHandleBmp.LockBits(rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite, newHandleBmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * bmpData.Height;
            byte[] grayValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
            byte[] tempArray = new byte[bytes];
            //double gradX, gradY, grad;
            newHandleBmp.UnlockBits(bmpData);
            //改进梯度幅值计算
            BitmapData bmData = newHandleBmp.LockBits(new Rectangle(0, 0, newHandleBmp.Width, newHandleBmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            float valve = 67;
            for (int i = 0; i < 256; i++)
            {
                gray[i] = 0;
            }
            unsafe
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                byte* p = (byte*)(void*)Scan0;
                byte* pp;
                int tt;
                int nOffset = stride - newHandleBmp.Width * 3;
                int nWidth = newHandleBmp.Width;
                int nHeight = newHandleBmp.Height;
                int Sx = 0;
                int Sy = 0;
                double sumM = 0;
                double sumCount = 0;

                //Priwitt算子边缘检测
                int[] marginalMx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                int[] marginalMy = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

                int[,] dlta = new int[nHeight, nWidth];
                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (!(x <= 0 || x >= nWidth - 1 || y <= 0 || y >= nHeight - 1))
                        {
                            pp = p;
                            Sx = 0;
                            Sy = 0;
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    pp += (j * 3 + stride * i);
                                    Sx += pp[0] * marginalMx[(i + 1) * 3 + j + 1];
                                    Sy += pp[0] * marginalMy[(i + 1) * 3 + j + 1];
                                    pp = p;
                                }
                            }
                            m[y, x] = (int)(Math.Sqrt(Sx * Sx + Sy * Sy));
                            //增强白点
                            if (m[y, x] > valve / 2)
                            {
                                if (p[0] > 240)
                                {
                                    m[y, x] += valve;
                                }
                                else if (p[0] > 220)
                                {
                                    m[y, x] += (float)(valve * 0.8);
                                }
                                else if (p[0] > 200)
                                {
                                    m[y, x] += (float)(valve * 0.6);
                                }
                                else if (p[0] > 180)
                                {
                                    m[y, x] += (float)(valve * 0.4);
                                }
                                else if (p[0] > 160)
                                {
                                    m[y, x] += (float)(valve * 0.2);
                                }
                            }
                            float tan;
                            if (Sx != 0)
                            {
                                tan = Sy / Sx;
                            }
                            else
                            {
                                tan = 10000;
                            }

                            if (-0.41421356 <= tan && tan < 0.41421356)   //角度为-22.5度到22.5度之间
                            {
                                dlta[y, x] = 0;
                            }
                            else if (0.41421356 <= tan && tan < 2.41421356)//角度为22.5度到67.5度之间
                            {
                                dlta[y, x] = 1; //m[y,x] = 0;
                            }
                            else if (tan >= 2.41421356 || tan < -2.41421356)//角度为67.5度到90度之间或-90度到-67.5度
                            {
                                dlta[y, x] = 2;    //	m[y,x]+=valve;
                            }
                            else
                            {
                                dlta[y, x] = 3;//m[y,x] = 0;     
                            }
                        }
                        else
                        {
                            m[y, x] = 0;

                        }
                        p += 3;
                        if (m[y, x] > 0)
                        {
                            sumCount++;
                            sumM += m[y, x];
                        }
                    }
                    p += nOffset;
                }

                p = (byte*)(void*)Scan0;    //非极大值抑制和阈值
                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        if (m[y, x] > sumM / sumCount * 1.2)
                        {
                            p[0] = p[1] = p[2] = (byte)(m[y, x]);
                        }
                        else
                        {
                            m[y, x] = 0;
                            p[0] = p[1] = p[2] = 0;
                        }
                        if (x >= 1 && x <= nWidth - 1 && y >= 1 && y <= nHeight - 1 && m[y, x] > valve)
                        {
                            switch (dlta[y, x])
                            {
                                case 0:
                                    if (m[y, x] >= m[y, x - 1] && m[y, x] >= m[y, x + 1])
                                    {
                                        p[0] = p[1] = p[2] = 255;
                                    }
                                    break;
                                case 1:
                                    if (m[y, x] >= m[y + 1, x - 1] && m[y, x] >= m[y - 1, x + 1])//正斜45度边缘
                                    {
                                        p[0] = p[1] = p[2] = 255;
                                    }
                                    break;

                                case 2:
                                    if (m[y, x] >= m[y - 1, x] && m[y, x] >= m[y + 1, x])//垂直边缘
                                    {
                                        p[0] = p[1] = p[2] = 255;
                                    }
                                    break;

                                case 3:
                                    if (m[y, x] >= m[y + 1, x + 1] && m[y, x] >= m[y - 1, x - 1])//反斜45度边缘
                                    {
                                        p[0] = p[1] = p[2] = 255;
                                    }
                                    break;

                            }
                        }
                        if (p[0] == 255)
                        {
                            m[y, x] = 1;
                        }
                        else
                        {
                            m[y, x] = 0;
                            p[0] = p[1] = p[2] = 0;
                        }
                        tt = p[0];
                        gray[tt]++;
                        p += 3;
                    }

                }
                newHandleBmp.UnlockBits(bmData);
            }
        }

        /// <summary>
        /// 将 BitmapSource 转化为 Bitmap
        /// </summary>
        /// <param name="source"/>要转换的 BitmapSource
        /// <returns>转化后的 Bitmap</returns>
        private Bitmap ToBitmap(BitmapSource source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(ms);
                return new Bitmap(ms);
            }
        }
        /// <summary>
        /// 直方图均衡化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private void Equalization(Bitmap bmp)
        {
            if (bmp != null)
            {
                newHandleBmp = bmp.Clone() as Bitmap;//clone一个副本
                int width = newHandleBmp.Width;
                int height = newHandleBmp.Height;
                int size = width * height;
                //总像素个数
                int[] gray = new int[256];
                //定义一个int数组，用来存放各像元值的个数
                double[] graydense = new double[256];
                //定义一个float数组，存放每个灰度像素个数占比
                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                    {
                        Color pixel = newHandleBmp.GetPixel(i, j);
                        //计算各像元值的个数
                        gray[Convert.ToInt16(pixel.R)] += 1;
                        //由于是灰度只读取R值
                    }
                for (int i = 0; i < 256; i++)
                {
                    graydense[i] = (gray[i] * 1.0) / size;
                    //每个灰度像素个数占比
                }

                for (int i = 1; i < 256; i++)
                {
                    graydense[i] = graydense[i] + graydense[i - 1];
                    //累计百分比
                }

                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                    {
                        Color pixel = newHandleBmp.GetPixel(i, j);
                        int oldpixel = Convert.ToInt16(pixel.R);//原始灰度
                        int newpixel = 0;
                        if (oldpixel == 0)
                            newpixel = 0;
                        //如果原始灰度值为0则变换后也为0
                        else
                            newpixel = Convert.ToInt16(graydense[Convert.ToInt16(pixel.R)] * 255);
                        //如果原始灰度不为0，则执行变换公式为   <新像元灰度 = 原始灰度 * 累计百分比>
                        pixel = Color.FromArgb(newpixel, newpixel, newpixel);
                        newHandleBmp.SetPixel(i, j, pixel);//读入newbitmap
                    }
            }
        }
        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public void ImgToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            newHandleBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.DontCare);
        }

        #endregion

    }
}
