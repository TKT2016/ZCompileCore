using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(string))]
    public class 文本 
    {
        [ZCode("长度")]
        public int Length { get; set; }

        [ZCode("(string:str)以(string:value)结尾")]
        public static bool 以_结尾(string str,string value)
        {
            return str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
