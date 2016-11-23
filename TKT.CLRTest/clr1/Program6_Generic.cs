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
    class Program6_Generic
    {
        T get_T<T>(object t)
        {
            return (T)(t);
        }
        
        void test_T()
        {
            string x = get_T<string>("AAAAAA");
            //Console.WriteLine(x);
        }
    }
}
