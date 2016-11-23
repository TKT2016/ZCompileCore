using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZLangRT.Descs
{
    public class ExFieldInfo
    {
        public FieldInfo Field { get; private set; }
        public bool IsSelf { get; private set; }

        public ExFieldInfo(FieldInfo field, bool isSelf)
        {
            Field = field;
            IsSelf = isSelf;
        }
    }
}
