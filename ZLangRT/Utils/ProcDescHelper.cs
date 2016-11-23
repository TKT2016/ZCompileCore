using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Descs;

namespace ZLangRT.Utils
{
    public static class ProcDescHelper
    {
        public static TKTConstructorDesc CreateProcBracket(ConstructorInfo ci)
        {
            List<TKTProcArg> args = new List<TKTProcArg>();
            foreach (ParameterInfo param in ci.GetParameters())
            {
                TKTProcArg arg = new TKTProcArg(param.Name, param.ParameterType, false);
                args.Add(arg);
            }
            TKTConstructorDesc desc = new TKTConstructorDesc(args);
            desc.Constructor=ci;
            return desc;
        }

        public static TKTProcDesc CreateProcDesc(ExMethodInfo exMethod)
        {
            var method = exMethod.Method;
            TKTProcDesc desc = new TKTProcDesc();
            desc.Add(method.Name);
            if(method.IsGenericMethod)
            {
                foreach (Type paramType in method.GetGenericArguments())
                {
                    TKTProcArg arg = new TKTProcArg(paramType, true);
                    desc.Add(arg);
                }
            }
            if (method.GetParameters().Length > 0)
            {
                List<TKTProcArg> args = new List<TKTProcArg>();
                foreach (ParameterInfo param in method.GetParameters())
                {
                    TKTProcArg arg = new TKTProcArg(param.Name, param.ParameterType, false);
                    args.Add(arg);
                }
                desc.Add(args);
            }
            desc.ExMethod = exMethod;
            return desc;
        }

        public static Tuple<string, Type[]> CreateMethodDesc(TKTProcDesc procDesc)
        {
            if (procDesc.Parts.Count > 0 && procDesc.Parts[0] is string)
            {
                string methodName = procDesc.Parts[0] as string;
                if (methodName == null) return null;
                Type[] types = procDesc.GetArgTypes(0).ToArray();
                return new Tuple<string, Type[]>(methodName, types);
            }
            else
            {
                return null;
            }
        }

        public static MethodInfo SearchMethod(Type type,TKTProcDesc procDesc)
        {
            foreach(var method in type.GetMethods())
            {
                ExMethodInfo exMethod = new ExMethodInfo(method, method.DeclaringType == type);
                TKTProcDesc methodDesc = CreateProcDesc(exMethod);
                if (methodDesc.Eq(procDesc))
                    return method;
            }
            return null;
        }
    }
}
