using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT
{
    public static class TKTLambda
    {
        public static readonly Type ActionType = typeof(Action);
        public static readonly Type CondtionType = typeof(Func<bool>);

        public static bool IsFn(Type type)
        {
            return type == ActionType || type == CondtionType;
        }
    }
}
