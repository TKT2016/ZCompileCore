using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZClass]
    public class 常用数学函数 //: TKTSimpleClass
    {
		public static float SIN(float a)
        {
            return (float)Math.Sin(a);
        }

        public static float COS(float a)
        {
            return (float)Math.Cos(a);
        }

        public static float TAN(float a)
        {
            return (float)Math.Tan(a);
        }

        public static int ABS(int a)
        {
            return (int)Math.Abs(a);
        }

        public static float ABS(float a)
        {
            return Math.Abs(a);
        }

		/*
        public static float SIN(float a)
        {
            return (float)Math.C(a);
        }

        public static float SIN(float a)
        {
            return (float)Math.Sin(a);
        }

        public static float SIN(float a)
        {
            return (float)Math.Sin(a);
        }

        public static float SIN(float a)
        {
            return (float)Math.Sin(a);
        }*/
    }
}
