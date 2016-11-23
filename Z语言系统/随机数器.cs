using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZClass]
    public static class 随机数器
    {
        static Random rnd = new Random();

        public static int 生成随机数()
        {
            return rnd.Next();
        }
        /*
        public static int 生成随机数(int 最小值,int 最大值)
        {
            return rnd.Next(最小值, 最大值);
        }*/

        public static int 生成随机数(int 最小值, int 最大值)
        {
            int x= rnd.Next((int)最小值, (int)最大值);
            //Console.WriteLine("最小值=" + 最小值 + ",最大值:" + 最大值+",结果="+x);
            return x;
        }

        public static int 生成随机数(float 最小值, float 最大值)
        {
            int x = rnd.Next((int)最小值, (int)最大值);
            return x;
        }
    }
}
