using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TKT操作系统;

namespace TKT.CLRTest.clr1
{
    class TestUser32
    {
        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindow(string ClassName, string CaptionName);
        [DllImport("User32.dll ")]
        public static extern int SendMessage(IntPtr hwad, int wMsg, int lParam, int wParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hwnd2);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parenthW, IntPtr child, string s1, string s2);

        public const int WM_SETTEXT = 0x000C;
        public const int WM_CHAR = 0x0102;

        public static void RunNotePad()
        {
            var process = 进程辅助.启动程序("C:\\Windows\\system32\\notepad.exe");
            string className = "Notepad";
            string captionName = "无标题 - 记事本";
            IntPtr hwnd = FindWindow(className, captionName);//找主窗口.
            IntPtr hwnd2 = FindWindowEx(hwnd, IntPtr.Zero, "Edit", "");  //  找子窗体
            //SendMessage(hwnd22,256,97,0);
            SendMessage(hwnd2, WM_CHAR, (int)'h', 0);
            SendMessage(hwnd2, WM_CHAR, (int)'e', 0);
            SendMessage(hwnd2, WM_CHAR, (int)'l', 0);
            SendMessage(hwnd2, WM_CHAR, (int)'l', 0);
            SendMessage(hwnd2, WM_CHAR, (int)'o', 0);
            Console.ReadLine();
        }
    }
}
