using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKT.RT;
using TKT.RT.Utils;
using TKT绘图;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class TEST_Calculater
    {
        static int 计数器 { get; set; }

        static TEST_Calculater()
        {
            计数器 = 0;
        }

        static void TEST()
        {
            Console.WriteLine(Calculater.GE(计数器,0));
        }
    }
}
