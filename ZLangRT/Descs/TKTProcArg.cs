using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Utils;

namespace ZLangRT.Descs
{
    public class TKTProcArg
    {
        public Type ArgType { get; set; }
        public string ArgName { get; set; }
        public bool IsGenericArg { get; set; }
        public object Value { get; set; }

        public TKTProcArg(string argName, Type argType, bool isGenericArg)
        {
            ArgType = argType;
            ArgName = argName;
            IsGenericArg = isGenericArg;
        }

        public TKTProcArg(Type argType, bool isGenericArg)
        {
            ArgType = argType;
            IsGenericArg = isGenericArg;
        }

        static bool eqCondiType(TKTProcArg arg1,TKTProcArg arg2)
        {
            if (arg1.ArgType == TKTLambda.CondtionType)
            {
                if (arg2.ArgType == TKTLambda.CondtionType || arg2.ArgType == typeof(bool))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        bool EqArgType(TKTProcArg anthorArg)
        {
            if (this.ArgType == null || anthorArg.ArgType == null) return true;
            if (this.ArgType == TKTLambda.ActionType || anthorArg.ArgType == TKTLambda.ActionType) return true;
            if ((this.ArgType == TKTLambda.CondtionType) || (anthorArg.ArgType == TKTLambda.CondtionType))
            {
                if (eqCondiType(this, anthorArg) || eqCondiType(anthorArg, this)) 
                    return true;
                return false;
            }
            if (ReflectionUtil.IsNumberType(this.ArgType) && ReflectionUtil.IsNumberType(anthorArg.ArgType))
            {
                return (ReflectionUtil.MoreEqNumberType(this.ArgType, anthorArg.ArgType)) ;
            }
            if (ReflectionUtil.IsExtends(anthorArg.ArgType, this.ArgType))
            {
                return true;
            }
            return false;
        }

        public bool Eq(TKTProcArg anthorArg)
        {
            if (!EqArgType(anthorArg)) return false;
            if (anthorArg.ArgName != null && this.ArgName!=null)
            {
                return anthorArg.ArgName == this.ArgName;
            }
            if (this.IsGenericArg != anthorArg.IsGenericArg) return false;
            return true;
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            buff.Append(ArgType.Name);
            if (ArgName != null)
            {
                buff.Append(":");
                buff.Append(ArgName);
            }
            return buff.ToString();
        }

    }
}
