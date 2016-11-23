using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZLangRT.Descs
{
    public class ExMethodInfo
    {
        public MethodInfo Method { get; private set; }
        public bool IsSelf { get; private set; }

        public ExMethodInfo(MethodInfo method, bool isSelf)
        {
            Method = method;
            IsSelf = isSelf;
        }
    }
}
