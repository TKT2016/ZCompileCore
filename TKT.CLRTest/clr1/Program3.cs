using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKT.CLRTest.Lei;
using TKT.RT;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class Program3
    {
        static void Main4(string[] args)
        {
            TesClassA t1 = new TesClassA();
            t1.Name = "aabb";
            t1.Out("1000", "1001");
        }

        static void test()
        {
            var A = 求和(100, 50);
            Console.WriteLine(A);
            Console.ReadKey();
        }

        static int 求和(int a,int b)
        {
            var 结果 = a + b;
            return 结果;
        }

        static void Test_打印()
        {
            int a = 100;
            控制台.打印(a);
            float b = 1.002f;
            控制台.打印(b);
        }

        static void Test_TEnum()
        {
            Console.WriteLine(TEnum.MUL);
        }

        static List<int> testList()
        {
            List<int> list = new List<int>();
            list.Add(16);
            list.Add(32);
            return list;
        }

        static string getIndex(List<int> list,Dictionary<string,string> dict)
        {
            list[5] = 100;
            string str = list[4] + dict["aaa"];
            return str;
        }

        enum TEnum
        {
            ADD,
            SUB,
            MUL
        }
    }
}
