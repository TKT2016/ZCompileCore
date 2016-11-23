using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLangRT;
using ZLangRT.Attributes;
using ZLangRT.Descs;

namespace Z语言系统
{
    [ZClass]
    public static class 类型辅助
    {
        [Code("(object:obj)是(类型:T)")]
        public static bool 是<T>(object obj)
        {
            return (obj is T);
        }

        [Code("(object:obj)强制转换为(类型:T)")]
        public static T 强制转换<T>(object obj)
        {
            return (T)(obj);
        }

        [Code("创建一个新的(类型:T)")]
        public static T 创建<T>()
        {
            Type type = typeof(T);
            object obj = Activator.CreateInstance(type);
            return (T)obj;
        }
    }
}
