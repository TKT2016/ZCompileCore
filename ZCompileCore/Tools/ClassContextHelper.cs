
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Loads;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.Tools
{
    public static class ClassContextHelper
    {
        public static TKTProcDesc[] SearchDirectProc(ClassContext context, TKTProcDesc procDesc)
        {
            List<TKTProcDesc> list = new List<TKTProcDesc>();
            foreach (var tktClass in context.ImportContext.DirectClasses)
            {
                var temp = tktClass.SearchProc(procDesc);
                if (temp != null)
                {
                    list.Add(temp);
                }
            }
            return list.ToArray();
        }

        public static TKTProcDesc[] SearchProc(ClassContext context,TKTProcDesc procDesc)
        {
            var symbols = context.Symbols;
            //1.寻找当前CLASS内的PROC
            var method = context.ClassSymbol.SearchProc(procDesc);
            if (method != null) return new TKTProcDesc[] { method };
            //2.寻找简略使用的PROC
            var directProcArray = SearchDirectProc(context,procDesc);
            if (directProcArray.Length > 0) return directProcArray;
            return new TKTProcDesc[] { };
        }

        public static TKTProcDesc[] SearchSuppleProc(ClassContext context, TKTProcDesc procDesc)
        {
            var symbols = context.Symbols;
            //1.寻找当前CLASS内的PROC
            var method = context.ClassSymbol.SearchProc(procDesc);
            if (method != null) return new TKTProcDesc[] { method };
            //2.寻找简略使用的PROC
            var directProcArray = SearchDirectProc(context, procDesc);
            if (directProcArray.Length > 0) return directProcArray;
            return new TKTProcDesc[] { };
        }

        public static TKTProcDesc[] SearchProc(ClassContext context, Type subjType, TKTProcDesc procDesc)
        {
            var symbols = context.Symbols;
            //3.根据主语对象寻找当前项目的PROC
            if (!(subjType is TypeBuilder))
            {
                var gcl = context.ImportContext.SearchGCL(subjType);
                if (gcl != null)
                {
                    TKTProcDesc getedDesc = gcl.SearchProc(procDesc);
                    if (getedDesc == null)
                    {
                        return new TKTProcDesc[] { };
                    }
                    else
                    {
                        return new TKTProcDesc[] { getedDesc };
                    }
                }
            }
            else//3.根据主语对象寻找开发包的PROC
            {
                string name = subjType.Name;
                SymbolInfo symbol = symbols.Get(name);
                if (symbol is SymbolDefClass)
                {
                    var proc3 = (symbol as SymbolDefClass).SearchProc(procDesc);
                    if (proc3 != null)
                    {
                        return new TKTProcDesc[] { proc3 };
                    }
                }
            }
            return new TKTProcDesc[] { };
        }

        public static object[] SearchDirectIdent(ClassContext context,string name)
        {
            List<object> result = new List<object>();
            
            foreach (IGcl tkt in context.ImportContext.DirectClasses)
            {
                ExPropertyInfo property = tkt.SearchExProperty(name);// TKTMappingUtil.SearchExProperty(name, tkt);
                if (property != null)
                {
                    if (ReflectionUtil.IsStatic(property.Property) && ReflectionUtil.IsPublic(property.Property))
                    {
                        result.Add(property);
                    }
                }
                ExFieldInfo field = tkt.SearchExField(name);
                if (field != null)
                {
                    if (field.Field.IsPublic && field.Field.IsStatic)
                    {
                        result.Add(field);
                    }
                }
            }

            return result.ToArray();
        }
    }
}
