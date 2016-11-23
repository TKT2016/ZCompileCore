using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(bool))]
    public class 判断符 
    {
        public static string ToText(bool b)
        {
            return b ? "是" : "否";
        }

    }
}
