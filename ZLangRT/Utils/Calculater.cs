using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLangRT.Utils
{
    public static class Calculater
    {
        public static int AddInt(int a, int b)
        {
            return a + b;
        }

        public static float AddFloat(float a, float b)
        {
            return a + b;
        }

        public static int SubInt(int a, int b)
        {
            return a - b;
        }

        public static float SubFloat(float a, float b)
        {
            return a + b;
        }

        public static int MulInt(int a, int b)
        {
            return a * b;
        }

        public static float MulFloat(float a, float b)
        {
            return a * b;
        }

        public static float DivFloat(float a, float b)
        {
            return a / b;
        }


        public static float DivInt(int a, int b)
        {
            return (float)a / (float)b;
        }
        /*
        private static void debug(string message)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Debug:" + message);
            Console.ForegroundColor = temp;
        }
        */
        public static bool AND(bool a, bool b)
        {
            return a && b;
        }

        public static bool OR(bool a, bool b)
        {
            return a || b;
        }

        public static string ObjToString(object obj)
        {
            if (obj is bool)
            {
                return ((bool)obj) ? "是" : "否";
            }
            else
            {
                return obj.ToString();
            }
        }

        public static string AddString(object a, object b)
        {
            string str1 = ObjToString(a);
            string str2 = ObjToString(b);
            return str1 + str2;
        }

        public static bool GT(object a,object b)
        {
            //return (double)a > (double)b;
            if (IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) > Convert.ToDouble(b);
            throw new RTException("Calculater.GT失败");
        }

        public static bool GE(object a, object b)
        {
            //Console.Write(a);
            //Console.Write( "  ");
            //Console.WriteLine( b);
            /*if(a is int && b is int)
                return (int)a >= (int)b;
            if (a is float && b is float)
                return (float)a >= (float)b;
            if (a is float || b is float)
                return Convert.ToDouble(a) >= Convert.ToDouble(b);*/
            if(IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) >= Convert.ToDouble(b);
            throw new RTException("Calculater.GE失败");
        }

        public static bool EQBool(bool a, bool b)
        {
            return a == b;
        }

        public static bool EQInt(int a, int b)
        {
            return a == b;
        }

        public static bool EQFloat(float a, float b)
        {
            return a == b;
        }

        public static bool EQRef(object a, object b)
        {
            //return a == b;
            if (IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) == Convert.ToDouble(b);
            else if(IsEnumValue(a)&&IsEnumValue(b))
                return (int)a==(int)b;
            else if ((a is string) && (b is string))
                return (string)a == (string)b;
            else
                return a == b;
            //throw new RTException("Calculater.EQ失败");
        }

        public static bool NEBool(bool a, bool b)
        {
            return a != b;
        }


        public static bool NEInt(int a, int b)
        {
            return a != b;
        }

        public static bool NEFloat(float a, float b)
        {
            return a != b;
        }

        public static bool NERef(object a, object b)
        {
            //return a != b;
            if (IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) != Convert.ToDouble(b);
            else if (IsEnumValue(a) && IsEnumValue(b))
                return (int)a != (int)b;
            else if ((a is string) && (b is string))
                return (string)a != (string)b;
            else
                return a != b;
            //throw new RTException("Calculater.NE失败");
        }

        public static bool LTRef(object a, object b)
        {
            //return (double)a < (double)b;
            if (IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) < Convert.ToDouble(b);
            throw new RTException("Calculater.LT失败");
        }

        public static bool LTInt(int a, int b)
        {
            return a < b;
        }

        public static bool LERef(object a, object b)
        {
            //return (double)a <= (double)b;
            if (IsNumber(a) && IsNumber(b))
                return Convert.ToDouble(a) <= Convert.ToDouble(b);
            throw new RTException("Calculater.LE失败");
        }

        public static bool LEInt(int a, int b)
        {
            return a <= b;
        }

        public static bool LEFloat(float a, float b)
        {
            return a <= b;
        }

        public static bool IsNumber(object x)
        {
            return x is byte || x is char || x is float || x is int || x is decimal || x is double;
        }

        public static bool IsEnumValue(object x)
        {
            Type t = x.GetType();
            return t.IsEnum;
        }

    }
}
