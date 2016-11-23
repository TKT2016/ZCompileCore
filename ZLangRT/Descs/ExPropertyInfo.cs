using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZLangRT.Descs
{
    public class ExPropertyInfo
    {
        public PropertyInfo Property { get; private set; }
        public bool IsSelf { get; private set; }

        public ExPropertyInfo(PropertyInfo property, bool isSelf)
        {
            Property = property;
            IsSelf = isSelf;
        }
    }
}
