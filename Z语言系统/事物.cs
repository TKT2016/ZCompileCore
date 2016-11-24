using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZMapping(typeof(object))]
    public abstract class 事物
    {
        [ZCode("生成文本形式")]
        public abstract override string ToString();
    }

}
