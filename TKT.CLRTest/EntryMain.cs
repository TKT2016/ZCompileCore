using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKT.CLRTest.S1;

namespace TKT.CLRTest
{
    public class EntryMain
    {
        public static void Main()
        {
            //TKT.CLRTest.CS3.列表例子.TestEveryOne();
            //Console.ReadKey();
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestTurlet1());*/

            TestBuYu.Out(5);
            Console.ReadKey();
        }

        public static float ToF(int i)
        {
            return i;
        }

        public static float ToF2(int i)
        {
            float x = i;
            return x;
        }
    }
}
