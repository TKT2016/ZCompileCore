using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKT.CLRTest.clr1
{
    class Program5_Func
    {
        void RunTimes(Action fn, int times)
        {
            for (int i = 0; i < times; i++)
            {
                fn();
            }
        }

        void RunTimes_test_none()
        {
            Action fn1 = () => { Console.WriteLine("AAAA"); };
            RunTimes(fn1,5);
        }

        string name="NAME";

        void RunTimes_test_field()
        {
            Action fn1 = () => { Console.WriteLine(name); };
            RunTimes(fn1,5);
        }

        void RunTimes_test_local()
        {
            string a= name+"_AAA";
            Action fn1 = () => { Console.WriteLine(a); };
            RunTimes(fn1,5);
        }

        void RunTimes_test_arg(int i)
        {
            Action fn1 = () => { Console.WriteLine("bbbb_"+i); };
            RunTimes(fn1,5);
        }

        void RunTimes_test_Method()
        {
            string a = name + "_AAA";
            Action fn1 = () => { Print(a); };
            RunTimes(fn1, 5);
        }

        void Print(string str)
        {
            Console.WriteLine(str);
        }

        static void SPrint(string str)
        {
            Console.WriteLine(str);
        }

        void SRunTimes_test_Method()
        {
            string a = name + "_AAA";
            Action fn1 = () => { SPrint(a); };
            RunTimes(fn1, 5);
        }
        /*
        delegate int Calculate(int a, int b);
        void test_delegate(string[] args)
        {
            int a = 3;
            int b = 4;
            Calculate result = delegate(int ta, int tb) { return ta + tb; };

            Console.WriteLine(result(a,b));
            Console.Read();
        }*/
    }
}
