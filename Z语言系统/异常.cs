using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(Exception))]
    public abstract class 异常
    {
        [Code("异常信息")]
        public string Message { get; set; }

    }
}
