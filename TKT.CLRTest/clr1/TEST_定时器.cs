using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TKT.CLRTest.clr1
{
    class TEST_定时器
    {
        static int i = 0;

        static void Main2(string[] args)
        {
            test_定时器();

            Console.ReadKey();
        }

        static void test_定时器()
        {
            定时器 T = new 定时器(1000); 
            T.运行内容 = () => { Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); i++; };
            T.停止条件 = () => { return i >= 5; };
            T.启动();
        }
    }
}
