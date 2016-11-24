using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(Console))]
    public abstract class 控制台
    {
        [ZCode("响铃")]
        public abstract void Beep();

        [ZCode("换行")]
        public abstract void WriteLine();

        [ZCode("换(int:k)行")]
        public static void ReadLine(int k)
        {
            for (int i = 0; i < k; i++)
            {
                Console.WriteLine();
            }
        }

        [ZCode("读取文本")]
        public abstract string ReadLine();


        [ZCode("等待按键")]
        public abstract void ReadKey();

        public static int 读取整数()
        {
            string str = Console.ReadLine();
            int value = int.Parse(str);
            return value;
        }

        public static int 读取整数(string 提示语)
        {
            Console.Write(提示语);
            return 读取整数();
        }

        public static float 读取数字()
        {
            string str = Console.ReadLine();
            float value = float.Parse(str);
            return value;
        }

        public static float 读取数字(string 提示语)
        {
            Console.Write(提示语);
            return 读取数字();
        }

        public static string 读取文本(string 提示语)
        {
            Console.Write(提示语);
            return Console.ReadLine();
        }

        public static void 打印(object obj)
        {
            if (obj is bool)
            {
                bool b = (bool)obj;
                Console.Write(判断符.ToText(b));
            }
            else
            {
                Console.Write(obj);
            }
        }

        public static void 换行打印(object obj)
        {
            Console.WriteLine();
            打印(obj);
        }
    }
}
