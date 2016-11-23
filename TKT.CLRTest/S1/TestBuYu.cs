using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Z语言系统;

namespace TKT.CLRTest.S1
{
    class TestBuYu
    {
        public static void Out(int y)
        {
            int x = y;
            补语控制.执行_次(() => { Console.Write(" 你好 "+x);},3);
        }

        [STAThread]
        public static void DY()
        {
            int num = 100;
            控制台.打印(num);
        }
    }
}
