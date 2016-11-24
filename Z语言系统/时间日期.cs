using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(DateTime))]
    public class 时间日期
    {
        [ZCode("年")]
        public int Year { get; set; }

        [ZCode("月")]
        public int Month { get; set; }

        [ZCode("日")]
        public int Day { get; set; }

        [ZCode("时")]
        public int Hour { get; set; }

        [ZCode("分")]
        public int Minute { get; set; }

        [ZCode("秒")]
        public int Second { get; set; }

        [ZCode("毫秒")]
        public int Millisecond { get; set; }

        [ZCode("(DateTime:dt1)大于(DateTime:dt2)")]
        public static bool 大于(DateTime dt1,DateTime dt2)
        {
            return dt1 > dt2;
        }

        [ZCode("(DateTime:dt1)等于(DateTime:dt2)")]
        public static bool 等于(DateTime dt1, DateTime dt2)
        {
            return dt1 == dt2;
        }

        [ZCode("(DateTime:dt1)小于(DateTime:dt2)")]
        public static bool 小于(DateTime dt1, DateTime dt2)
        {
            return dt1 < dt2;
        }

        [ZCode("(DateTime:dt)加(int:ms)毫秒")]
        public static DateTime 加毫秒(DateTime dt, int ms)
        {
            return dt.AddMilliseconds(ms);
        }

        [ZCode("(DateTime:dt)加(int:s)秒")]
        public static DateTime 加秒(DateTime dt, int s)
        {
            return dt.AddSeconds(s);
        }

        [ZCode("(DateTime:dt)加(int:m)分钟")]
        public static DateTime 加分(DateTime dt, int m)
        {
            return dt.AddMinutes(m);
        }

        [ZCode("(DateTime:dt)加(int:h)小时")]
        public static DateTime 加小时(DateTime dt, int h)
        {
            return dt.AddHours(h);
        }

        [ZCode("(DateTime:dt)加(int:d)天")]
        public static DateTime 加天(DateTime dt, int d)
        {
            return dt.AddDays(d);
        }

        [ZCode("(DateTime:dt)加(int:d)天(int:h)小时(int:m)分钟(int:s)秒")]
        public static DateTime 加天时分秒(DateTime dt, int d,int h,int m,int s)
        {
            DateTime d2= dt.AddDays(d);
            d2 = d2.AddHours(h);
            d2 = d2.AddMinutes(m);
            d2 = d2.AddSeconds(s);
            return d2;
        }

        [ZCode("(DateTime:dt)转化为文本")]
        public static string 转化为文本(DateTime dt)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
