using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZClass]
    public static class 补语控制
    {
        [ZCode("执行(可运行语句:act)")]
        public static void 执行(Action act)
        {
            act();
        }

        [ZCode("(可运行语句:act)(int:times)次")]
        public static void 执行_次(Action act, int times)
        {
            for(int i=0;i<times;i++)
            {
                //Console.WriteLine("第次"+i);
                act();
            }
        }

        [ZCode("(可运行语句:act)直到(可运行条件:condition)")]
        public static void 执行_直到(Action act, Func<bool> condition)
        {
            while(true)
            {
                if (!condition())
                    act();
            }
        }
    }
}
