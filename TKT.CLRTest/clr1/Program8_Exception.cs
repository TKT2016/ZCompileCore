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
    class Program8_Exception
    {
        public static void Test()
        {
            try
            {
                int x = 控制台.读取整数();
                控制台.打印(x);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
