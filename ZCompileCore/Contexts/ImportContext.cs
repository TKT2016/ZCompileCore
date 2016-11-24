using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Loads;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Analyers;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Attributes;
using ZLangRT.Utils;

namespace ZCompileCore.Analys.AContexts
{
    public class ImportContext
    {
        public Dictionary<string, List<IGcl>> importPackages { get;private set; }
        public List<IGcl> GenericCreatedTypes { get; set; }
        public List<IGcl> DirectClasses { get; private set; }
        public List<IGcl> GenericTypes { get; private set; }
        public List<IGcl> EnumTypes { get; private set; }

        public ImportContext()
        {
            this.importPackages =new Dictionary<string,List<IGcl>> ();
            this.GenericCreatedTypes = new List<IGcl>();
            this.DirectClasses = new List<IGcl>();
            this.GenericTypes = new List<IGcl>();
            EnumTypes = new List<IGcl>();
        }

        public IGcl[] SearchGCL(string name)
        {
            IGcl[] gcls = SearchGCLFromImport(name);
            if (gcls.Length > 0) return gcls;
            return new IGcl[] { };
        }

        public IGcl[] SearchGCLFromImport(string name)
        {
            IGcl gcl = searchFromImportTypes(name);
            if (gcl != null)
            {
                return new IGcl[] { gcl };
            }

            var packageIgcls = searchFromPackage(name);
            if (packageIgcls.Length > 0)
            {
                return packageIgcls;
            }
            return new IGcl[] { };
        }

        IGcl searchFromImportTypes(string name)
        {
            foreach (var igcl in GenericCreatedTypes)
            {
                if (igcl.ShowName == name)
                    return igcl;
            }
            return null;
        }

        IGcl[] searchFromPackage(string name)
        {
            List<IGcl> list = new List<IGcl>();
            foreach (var igcls in importPackages.Values.ToList())
            {
                foreach (var igcl in igcls)
                {
                    if (igcl.ShowName == name)
                        list.Add(igcl);
                }
            }
            return list.ToArray();
        }

        public IGcl SearchGCL(Type type)
        {
            foreach (var igcl in GenericCreatedTypes)
            {
                if (igcl.ForType == type)
                    return igcl;
            }
            /* 实现和包装在同一个项目里导致找不到正确的GCL */
            List<IGcl> gcls = new List<IGcl>();
            foreach (var igcls in importPackages.Values.ToList())
            {
                foreach (var igcl in igcls)
                {
                    if (igcl.ForType == type)
                    {
                        //return igcl;
                        gcls.Add(igcl);
                    }
                }
                if (gcls.Count > 0)
                {
                    break;
                }
            }

            if (gcls.Count > 0)
            {
                foreach (var gcl in gcls)
                {
                    if (!(gcl is ExternalGcl))
                    {
                        return gcl;
                    }
                }

                foreach (var gcl in gcls)
                {
                     return gcl;
                }
            }

            if(type.IsGenericType)
            {
                Type baseGenericType = type.GetGenericTypeDefinition();
                IGcl baseGCL= SearchGCL(baseGenericType);
                if(baseGCL==null) return null;
                IGcl newGCL = baseGCL.CreateNewFor(type);
                this.GenericCreatedTypes.Add(newGCL);
                return newGCL;
            }

            return null;
        }

        public List<SymbolEnumItem> SearchEnumItem(string name)
        {
            List<SymbolEnumItem> list = new List<SymbolEnumItem>();
            foreach(var gcl in this.EnumTypes)
            {
                if (gcl is TktGcl)
                {
                    var fields = gcl.ForType.GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var fi in fields)
                    {
                        if(fi.Name==name)
                        {
                            object value = fi.GetValue(null);
                            SymbolEnumItem symbol = new SymbolEnumItem(fi.Name, value);
                            list.Add(symbol);
                        }
                    }
                }
                else
                {
                    var fields = gcl.ForType.GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var fi in fields)
                    {
                        object value = fi.GetValue(null);
                        string rname = fi.Name;
                        var mapAttrObj = Attribute.GetCustomAttribute(fi, typeof(ZCodeAttribute));
                        if (mapAttrObj != null)
                        {
                            ZCodeAttribute mapAttr = mapAttrObj as ZCodeAttribute;
                            rname = mapAttr.Code;
                        }
                        if (rname == name)
                        {
                            SymbolEnumItem symbol = new SymbolEnumItem(rname, value);
                            list.Add(symbol);
                        }
                    }
                }
            }
            return list;
        }
    }
}
