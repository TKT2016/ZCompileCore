
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TKT.RT.Attributes;
using TKT.RT.Descs;
using TKT.RT.Utils;

namespace TKT.RT.Loads
{
    public static class GclHelper
    {
        public static IGcl Load(Type type,CnEnDict wordDict)
        {
            MappingTypeAttribute mattr = Attribute.GetCustomAttribute(type, typeof(MappingTypeAttribute)) as MappingTypeAttribute;
            if (mattr!= null)
            {
                return new MappingGcl(type,wordDict);
            }

            TKTClassAttribute tcAttr = Attribute.GetCustomAttribute(type, typeof(TKTClassAttribute)) as TKTClassAttribute;
            if (tcAttr != null)
            {
                return new TktGcl(type, wordDict);
            }

            return null;
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
