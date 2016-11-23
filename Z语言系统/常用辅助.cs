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
    public static class 常用辅助
    {
        [Code("执行(可运行语句:act)")]
        public static void 执行(Action act)
        {
            act();
        }

        [Code("(可运行语句:act)(int:times)次")]
        public static void 执行_次(Action act, int times)
        {
            for(int i=0;i<times;i++)
            {
                act();
            }
        }

        [Code("(可运行语句:act)直到(可运行条件:condition)")]
        public static void 执行_直到(Action act, Func<bool> condition)
        {
            while(true)
            {
                if (!condition())
                    act();
            }
        }

        public static bool 非(bool b)
        {
            return !b;
        }

        [Code("系统创建新(类型:T)")]
        public static T CreateNew<T>()
        {
            Type type = typeof(T);
            try
            {
                object obj = System.Activator.CreateInstance(type);
                return (T)obj;
            }
            catch (Exception ex)
            {
                throw new RTException("类型" + type.Name + "创建过程要求的参数个数不为0");
            }
        }

        [Code("暂停(int:t)毫秒")]
        public static void 暂停(int t)
        {
            Thread.Sleep(t);
        }
    }
}
