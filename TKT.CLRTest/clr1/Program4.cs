using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKT.RT;
using TKT.RT.Descs;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class Program4
    {
        public static int 次数 { get; set; }

        static Program4()
        {
            次数 = 0;
        }

        static void Main6(string[] args)
        {
            Print(次数);
            Console.WriteLine(typeof(List<int>).Name);
            Console.ReadKey();
        }

        static void Print(object obj)
        {
            Console.WriteLine(obj);
        }

        static TKTProcArg TestProcArg()
        {
            TKTProcArg arg = new TKTProcArg ("GET",typeof(string),true);
            TKTProcArg arg2 = new TKTProcArg("GET", typeof(string), false);
            return arg;
        }

        static T getT<K,T>(K i,T t)
        {
            if (i is string) return t;
            else return default(T);
        }

        static void getT_Test()
        {
            string k = getT<int,string>(100,"AASS");
            Console.WriteLine(k);
        }
    }
}
