namespace Project1
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class Class1
    {
        private static int recInterval;
        private static long start;
        private static int recSecconds;
        private static ArrayList points = new ArrayList();
        private static string last;

        private static void Capture()
        {
            Rectangle bounds = Screen.AllScreens[0].Bounds;
            Bitmap image = new Bitmap(bounds.Width, bounds.Height);
            Graphics.FromImage(image).CopyFromScreen(0, 0, 0, 0, new Size(image.Width, image.Height));
            image.Save(Directory.GetCurrentDirectory() + @"\images\" + getName() + ".png");
        }

        public static void crash(string msg)
        {
            try
            {
                string str = DateTime.Now.ToString(CultureInfo.InstalledUICulture).Replace("/", "-").Replace(":", "-");
                using (StreamWriter writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "crash_" + str + ".txt")))
                {
                    writer.WriteLine(msg);
                }
            }
            catch (Exception)
            {
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        private static string getName()
        {
            string str = DateTime.Now.ToString(CultureInfo.InstalledUICulture).Replace("/", "-").Replace(":", "-");
            int num = 2;
            string[] strArray = new string[] { Directory.GetCurrentDirectory(), @"\images\", last, @"\", str, ".png" };
            if (File.Exists(string.Concat(strArray)))
            {
                while (true)
                {
                    object[] objArray = new object[] { Directory.GetCurrentDirectory(), @"\images\", last, @"\", str, "_", num, ".png" };
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

        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        public static uint GetPixelColor(int x, int y)
        {
            IntPtr dC = GetDC(IntPtr.Zero);
            uint num = GetPixel(dC, x, y);
            ReleaseDC(IntPtr.Zero, dC);
            return num;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd);
        private static void loadConfig()
        {
            foreach (string str in File.ReadAllLines(Directory.GetCurrentDirectory() + @"\config.txt"))
            {
                string[] separator = new string[] { ":" };
                string[] strArray3 = str.Split(separator, Convert.ToChar(5), StringSplitOptions.None);
                if (str.StartsWith("recordInterval"))
                {
                    recInterval = int.Parse(strArray3[1]);
                }
                else if (str.StartsWith("recordSeconds"))
                {
                    recSecconds = int.Parse(strArray3[1]) * 0x3e8;
                }
                else
                {
                    string[] strArray7 = new string[] { "," };
                    if (str.Split(strArray7, Convert.ToChar(5), StringSplitOptions.None).Length == 3)
                    {
                        points.Add(new Dot(str));
                    }
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("1.1.2 new");
            try
            {
                loadConfig();
                if (recSecconds == 0)
                {
                    while (true)
                    {
                        Point position = Cursor.Position;
                        int x = position.X;
                        Point point2 = Cursor.Position;
                        int y = point2.Y;
                        uint pixelColor = GetPixelColor(x, y);
                        Console.WriteLine(string.Concat(new object[] { x, ",", y, ",", pixelColor }));
                        Thread.Sleep(recInterval);
                    }
                }
                while (true)
                {
                    if (start > 0L)
                    {
                        Capture();
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
                                last = DateTime.Now.ToString(CultureInfo.InstalledUICulture).Replace("/", "-").Replace(":", "-");
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\images\" + last);
                                Console.WriteLine("Recording " + last);
                                start = -1L;
                            }
                            Capture();
                        }
                    }
                    Thread.Sleep(recInterval);
                }
            }
            catch (Exception exception)
            {
                crash("1.1.2 new Error: " + exception.Message + " Stack Trace: " + exception.StackTrace);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
    }
}
