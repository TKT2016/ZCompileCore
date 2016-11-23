using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKT.RT;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class Program7
    {
        public static void Test()
        {
            列表<int> list = new 列表<int>();
            list.Add(100);
            list.Add(200);
            list.Add(300);
           控制台.打印(list[1]);
            Console.ReadKey();
        }
    }
}
