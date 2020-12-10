namespace ConsoleApplication1
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class Program
    {
        private static string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        private static int recInterval;
        private static string last;
        private static ArrayList points = new ArrayList();
        private static long start;
        private static int recSecconds;

        [DllImport("coredll.dll")]
        private static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, RasterOperation rasterOperation);
        private static void crash(string text)
        {
            try
            {
                string str = "crash-" + DateTime.Now.ToString().Replace("/", "-").Replace(":", "-") + ".txt";
                using (StreamWriter writer = new StreamWriter(Path.Combine(currentDirectory, str)))
                {
                    writer.WriteLine(text);
                    writer.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Critical error, unable save crash log..");
            }
        }

        [DllImport("coredll.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        private static string getName()
        {
            string str = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
            int num = 0x2;
            string[] strArray = new string[] { currentDirectory, @"\images\", last, @"\", str, ".jpg" };
            if (File.Exists(string.Concat(strArray)))
            {
                while (true)
                {
                    object[] objArray = new object[] { currentDirectory, @"\images\", last, @"\", str, "_", num, ".jpg" };
                    if (!File.Exists(string.Concat(objArray)))
                    {
                        str = str + "_" + num;
                        break;
                    }
                    num++;
                }
            }
            return (last + @"\" + str);
        }

        [DllImport("coredll.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        public static uint GetPixelColor(int x, int y)
        {
            IntPtr dC = GetDC(IntPtr.Zero);
            uint num = GetPixel(dC, x, y);
            ReleaseDC(IntPtr.Zero, dC);
            return num;
        }

        private static void loadConfig()
        {
            if (currentDirectory.StartsWith("file:"))
            {
                currentDirectory = @"C:\" + currentDirectory.Substring(9);
            }
            using (StreamReader reader = File.OpenText(currentDirectory + @"\config.txt"))
            {
                string line = "";
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        reader.Close();
                        break;
                    }
                    char[] separator = new char[] { ':' };
                    string[] strArray = line.Split(separator);
                    if (line.StartsWith("recordInterval"))
                    {
                        recInterval = int.Parse(strArray[1]);
                        continue;
                    }
                    if (line.StartsWith("recordSeconds"))
                    {
                        recSecconds = int.Parse(strArray[1]) * 0x3e8;
                        continue;
                    }
                    if (line.Split(new char[] { ',' }).Length == 3)
                    {
                        points.Add(new Dot(line));
                    }
                }
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("1.3.4 old");
                loadConfig();
                if (recSecconds == 0)
                {
                    while (true)
                    {
                        Point mousePosition = Control.MousePosition;
                        int x = mousePosition.X;
                        Point point2 = Control.MousePosition;
                        int y = point2.Y;
                        Console.WriteLine(string.Concat(new object[] { x, ",", y, ",", GetPixelColor(x, y) }));
                        Thread.Sleep(recInterval);
                    }
                }
                while (true)
                {
                    if (start > 0L)
                    {
                        screenshot();
                        DateTime now = DateTime.Now;
                        if (((now.Ticks / 0x2710L) - start) >= recSecconds)
                        {
                            start = 0L;
                        }
                    }
                    else
                    {
                        bool flag = true;
                        foreach (Dot dot in points)
                        {
                            if (GetPixelColor(dot.x, dot.y) != dot.color)
                            {
                                flag = false;
                            }
                        }
                        if (!flag)
                        {
                            if (start == -1L)
                            {
                                start = DateTime.Now.Ticks / 0x2710L;
                            }
                        }
                        else
                        {
                            if (start != -1L)
                            {
                                last = DateTime.Now.ToString().Replace("/", "-").Replace(":", "-");
                                Directory.CreateDirectory(currentDirectory + @"\images\" + last);
                                Console.WriteLine("Recording " + last);
                                start = -1L;
                            }
                            screenshot();
                        }
                    }
                    Thread.Sleep(recInterval);
                }
            }
            catch (Exception exception)
            {
                crash("1.3.4 old Error: " + exception.Message + " Stack Trace: " + exception.StackTrace);
            }
        }

        [DllImport("coredll.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        private static void screenshot()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            IntPtr dC = GetDC(IntPtr.Zero);
            Bitmap image = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format16bppRgb565);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                IntPtr hdc = graphics.GetHdc();
                BitBlt(hdc, 0, 0, bounds.Width, bounds.Height, dC, 0, 0, RasterOperation.SRC_COPY);
                graphics.ReleaseHdc(hdc);
            }
            image.Save(currentDirectory + @"\images\" + getName() + ".jpg", ImageFormat.Jpeg);
            ReleaseDC(IntPtr.Zero, dC);
        }

        private enum RasterOperation : uint
        {
            SRC_COPY = 0xcc0020
        }
    }
}
