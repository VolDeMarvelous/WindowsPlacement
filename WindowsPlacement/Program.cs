using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Microsoft;


namespace WindowsPlacement
{
    class Program
    {

        internal struct RECT
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }
        
        [DllImport("user32.dll")] 
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "TileWindows")]
        static extern  IntPtr TileWindows(IntPtr hWnd, int wHow, RECT[] lpRect, uint cKids, IntPtr[] lpKids);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ShowWindow(IntPtr hWnd, int Parametr);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]

        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        
        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(wnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleTexts"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(List<string> titleTexts)
        {
            int counter = 0;
            List<IntPtr> some = new List<IntPtr>();
            foreach (var text in titleTexts)
            {
                some.AddRange( new List<IntPtr>(FindWindows(delegate (IntPtr wnd, IntPtr param)
                {
                    return GetWindowText(wnd).Contains(text);
                })));
                foreach (var item in some)
                {
                    counter++;
                }
            }
            return some;
        }
        public static int FindNumberOfWindowsWithText(List<string> titleTexts)
        {
            int counter = 0;
            IEnumerable<IntPtr> some;
            foreach (var text in titleTexts)
            {
                some = FindWindows(delegate (IntPtr wnd, IntPtr param)
                {
                    return GetWindowText(wnd).Contains(text);
                });
                foreach (var item in some)
                {
                    counter++;
                }
            }
            return counter;
        }
        static void Main(string[] args)
        {

            List<string> WindowsNames = new List<string>() { "Chrome", "Edge", "Vivaldi", "Brave browser", "Firefox", " - Microsoft Visual Studio", "Visual Studio Code" };
            int numberOfWindows = FindNumberOfWindowsWithText(WindowsNames);
            List<IntPtr> enumerator = new List<IntPtr>(FindWindowsWithText(WindowsNames));

            RECT actualDesktop;
            GetWindowRect(GetDesktopWindow(), out actualDesktop);
            int _height = actualDesktop.height;
            int _width = actualDesktop.width;

            RECT one4 = new RECT { x = 0, y = 0, height = _height / 2, width = _width / 2 };
            RECT two4 = new RECT { x = _width / 2, y = 0, height = _height / 2, width = _width / 2 };
            RECT three4 = new RECT { x = 0, y = _height / 2, height = _height / 2, width = _width / 2 };
            RECT four4 = new RECT { x = _width / 2, y = _height / 2, height = _height / 2, width = _width / 2 };

            RECT one3 = new RECT { x = 0, y = 0, height = _height, width = _width / 2 };
            RECT two3 = new RECT { x = _width / 2, y = 0, height = _height / 2, width = _width / 2 };
            RECT three3 = new RECT { x = _width / 2, y = _height / 2, height = _height / 2, width = _width / 2 };

            RECT one2 = new RECT { x = 0, y = 0, height = _height, width = _width / 2 };
            RECT two2 = new RECT { x = _width / 2, y = 0, height = _height, width = _width / 2 };
            foreach (var item in enumerator)
            {
                
                Console.WriteLine("" + GetWindowText(item));
            }
            Console.WriteLine("" + numberOfWindows.ToString());
            if (numberOfWindows > 1 && numberOfWindows < 5)
            {
                switch (numberOfWindows)
                {
                    case 2:
                        {
                            ShowWindow(enumerator[0], 1);
                            SetWindowPos(enumerator[0], -2, one2.x, one2.y, one2.width, one2.height, 0x0020 | 0x0040);
                            ShowWindow(enumerator[1], 1);
                            SetWindowPos(enumerator[1], -2, two2.x, two2.y, two2.width, two2.height, 0x0020 | 0x0040);
                            // Debug
                            //Console.WriteLine("pos 1 : " + one2.x.ToString() + " |||| " + one2.y.ToString() + " |||| " + (one2.width).ToString() + " |||| " + (one2.height).ToString());
                            //Console.WriteLine("pos 2 : " + two2.x.ToString() + " |||| " + two2.y.ToString() + " |||| " + (two2.width).ToString() + " |||| " + (two2.height).ToString());

                            break;
                        }
                    case 3:
                        {
                            ShowWindow(enumerator[0], 1);
                            SetWindowPos(enumerator[0], -1, one3.x, one3.y, one3.width, one3.height, 0x4000 | 0x2000);
                            ShowWindow(enumerator[1], 1);
                            SetWindowPos(enumerator[1], -1, two3.x, two3.y, two3.width, two3.height, 0x4000 | 0x2000);
                            ShowWindow(enumerator[2], 1);
                            SetWindowPos(enumerator[2], -1, three3.x, three3.y, three3.width, three3.height, 0x4000 | 0x2000);
                            // Debug
                            //Console.WriteLine("pos 1 : " + one3.x.ToString() + " |||| " + one3.y.ToString() + " |||| " + (one3.width).ToString() + " |||| " + (one3.height).ToString());
                            //Console.WriteLine("pos 1 : " + two3.x.ToString() + " |||| " + two3.y.ToString() + " |||| " + (two3.width).ToString() + " |||| " + (two3.height).ToString());
                            //Console.WriteLine("pos 1 : " + three3.x.ToString() + " |||| " + three3.y.ToString() + " |||| " + (three3.width).ToString() + " |||| " + (three3.height).ToString());


                            break;
                        }
                    case 4:
                        {
                            ShowWindow(enumerator[0], 1);
                            SetWindowPos(enumerator[0], -1, one4.x, one4.y, one4.width, one4.height, 0x4000 | 0x2000);
                            ShowWindow(enumerator[1], 1);
                            SetWindowPos(enumerator[1], -1, two4.x, two4.y, two4.width, two4.height, 0x4000 | 0x2000);
                            ShowWindow(enumerator[2], 1);
                            SetWindowPos(enumerator[2], -1, three4.x, three4.y, three4.width, three4.height, 0x4000 | 0x2000); 
                            ShowWindow(enumerator[3], 1);
                            SetWindowPos(enumerator[3], -1, four4.x, four4.y, four4.width, four4.height, 0x4000 | 0x2000);
                            // Debug
                            //Console.WriteLine("pos 1 : " + one4.x.ToString() + " |||| " + one4.y.ToString() + " |||| " + (one4.width).ToString() + " |||| " + (one4.height).ToString());
                            //Console.WriteLine("pos 1 : " + two4.x.ToString() + " |||| " + two4.y.ToString() + " |||| " + (two4.width).ToString() + " |||| " + (two4.height).ToString());
                            //Console.WriteLine("pos 1 : " + three4.x.ToString() + " |||| " + three4.y.ToString() + " |||| " + (three4.width).ToString() + " |||| " + (three4.height).ToString());
                            //Console.WriteLine("pos 1 : " + four4.x.ToString() + " |||| " + four4.y.ToString() + " |||| " + (four4.width).ToString() + " |||| " + (four4.height).ToString());

                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
