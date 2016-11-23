
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Attributes;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.Loads
{
    public static class GclUtil
    {
        public static ExMethodInfo MakeGenericExMethod(ExMethodInfo exMethod,Type[] types)
        {
            if (exMethod == null) return null;
            var newMethod = exMethod.Method.MakeGenericMethod(types.ToArray());
            ExMethodInfo newExMethodInfo = new ExMethodInfo(newMethod, exMethod.IsSelf);
            return newExMethodInfo;
        }

        public static ExMethodInfo CreatExMethodInfo(MethodInfo methodInfo, Type forType)
        {
            if (methodInfo == null) return null;
            ExMethodInfo exMethodInfoInfo = new ExMethodInfo(methodInfo, methodInfo.DeclaringType == forType);
            return exMethodInfoInfo;
        }

        public static ExPropertyInfo CreatExPropertyInfo(PropertyInfo propertyInfo, Type forType)
        {
            if (propertyInfo == null) return null;
            ExPropertyInfo exFieldInfo = new ExPropertyInfo(propertyInfo, propertyInfo.DeclaringType == forType);
            return exFieldInfo;
        }

        public static ExFieldInfo CreatExFieldInfo(FieldInfo propertyInfo, Type forType)
        {
            if (propertyInfo == null) return null;
            ExFieldInfo exFieldInfo = new ExFieldInfo(propertyInfo, propertyInfo.DeclaringType == forType);
            return exFieldInfo;
        }

        public static ExPropertyInfo SearchExProperty(string name, Type forType)
        {
            var propertyInfo = forType.GetProperty(name);
            if (propertyInfo == null) return null;
            ExPropertyInfo exPropertyInfo = new ExPropertyInfo(propertyInfo, propertyInfo.DeclaringType == forType);
            return exPropertyInfo;
        }

        public static ExFieldInfo SearchExField(string name, Type forType)
        {
            var fieldInfo = forType.GetField(name);
            if (fieldInfo == null) return null;
            ExFieldInfo exFieldInfo = new ExFieldInfo(fieldInfo, fieldInfo.DeclaringType == forType);
            return exFieldInfo;
        }

        public static IGcl Load(Type type,CnEnDict wordDict)
        {
            ZMappingAttribute mattr = Attribute.GetCustomAttribute(type, typeof(ZMappingAttribute)) as ZMappingAttribute;
            if (mattr!= null)
            {
                return new MappingGcl(type,wordDict);
            }

            ZClassAttribute tcAttr = Attribute.GetCustomAttribute(type, typeof(ZClassAttribute)) as ZClassAttribute;
            if (tcAttr != null)
            {
                return new TktGcl(type, wordDict);
            }

            ExternalGcl egcl = new ExternalGcl(type, wordDict);
            return egcl;
        }


        public static TKTConstructorDesc SearchConstructor(TKTConstructorDesc desc, CnEnDict WordDict, Type ForType)
        {
            TKTConstructorDesc bracket2 = desc;
            if (WordDict != null && desc.Bracket.IsNameValue)
            {
                List<TKTProcArg> args = new List<TKTProcArg>();
                foreach (var arg in desc.Bracket.ListArgs)
                {
                    string newArgName = WordDict.Get(arg.ArgName);
                    TKTProcArg newArg = new TKTProcArg(newArgName, arg.ArgType, arg.ArgType.IsGenericType);
                    args.Add(newArg);
                }
                bracket2 = new TKTConstructorDesc(args);
            }
            ConstructorInfo[] constructorInfoArray = ForType.GetConstructors();
            foreach (ConstructorInfo ci in constructorInfoArray)
            {
                if (ci.IsPublic)
                {
                    TKTConstructorDesc bracketCi = ProcDescHelper.CreateProcBracket(ci);
                    if (bracketCi.Eq(bracket2))
                    {
                        return bracketCi;
                    }
                }
            }
            return null;
        }
    }
}
