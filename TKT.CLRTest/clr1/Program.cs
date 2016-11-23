using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKT.CLRTest.Lei;
using TKT.RT;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class Program
    {
        static void Main2(string[] args)
        {
            //testPrint("AAAA");
            //Console.ReadKey();
            /*文本 wb = new 文本("123456");
            控制台.打印(wb);*/
            List<int> list = new List<int>();
            list.Add(1); list.Add(2); list.Add(3); list.Add(4);
            List<int> list2 = new List<int>();
            list2.Add(10); list2.Add(20); list2.Add(30); list2.Add(40);
            list.RemoveAt(1);
            list.InsertRange(1, list2);
            string str = string.Join(",",list);
            Console.WriteLine(str);
            Console.ReadKey();
        }

        void testDecimal()
        {
            //decimal a = 1.23M;
        }

        int testIfElse(int i)
        {
            if(i==1)
            {
                return 10;
            }
            else if (i == 2)
            {
                return 20;
            }
            else if (i == 3)
            {
                return 30;
            }
            else if (i == 4)
            {
                return 40;
            }
            else if (i == 5)
            {
                return 50;
            }
            return -1;
        }
        /*
        static void testPrint(string str)
        {
            文本 wb = new 文本(str);
            控制台.打印(wb);
        }

        static void testRead()
        {
            文本 T = 控制台.读取();
            控制台.打印(T);
        }*/

        public string testGetProperty(TesClassA t1)
        {
            return t1.Name;
        }

        public void setSetProperty(TesClassA t1,string value)
        {
            t1.Value = value;
        }
        /*
        void testsub()
        {
            整数 I = new 整数(9);
            I = Calculater.SUB(I, new 整数(8888));
        }*/

        void testCall()
        {
            TesClassA t1 = new TesClassA();
            t1.Name = "testCall";
            t1.Out("很好","结束");
        }

        void testCallStatic()
        {
            TesClassA.Wr("aaa","bbb");
        }
    }

    
}
