using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Loads;
using ZCompileCore.Analys;
using ZLangRT;
using ZLangRT.Utils;

namespace ZCompileCore.Analyers
{
    public class TypeNameAnalyer
    {
        ImportContext context;
        Dictionary<string, IGcl> classDict = new Dictionary<string, IGcl>();

        public TypeNameAnalyer(ImportContext context)
        {
            this.context = context;
            addGenericDict(context.GenericTypes);
        }

        void  addGenericDict(List<IGcl> genericTypes)
        {
            foreach(var gcl in genericTypes)
            {
                string name =gcl.ShowName;
                if(!classDict.ContainsKey(name))
                {
                    classDict.Add(name, gcl);
                }
            }
        }

        public IGcl Analy(string name)
        {
            var array = context.SearchGCL(name);
            if (array.Length == 1)
            {
                IGcl gcl0 = array[0];
                int genericArgCount = GenericUtil.GetGenericTypeArgCount(gcl0.ForType);
                if (genericArgCount == 0)
                {
                    return gcl0;
                }
                else if(genericArgCount==1)
                {
                    Type newType = gcl0.ForType.MakeGenericType(new Type[] {typeof(object)});
                    IGcl result = gcl0.CreateNewFor(newType);
                    return result;
                }
                else if (genericArgCount ==2)
                {
                    Type newType = gcl0.ForType.MakeGenericType(new Type[] { typeof(object), typeof(object) });
                    IGcl result = gcl0.CreateNewFor(newType);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else if (array.Length > 1)
            {
                return null;
            }

            List<IGcl> glist = FindGenericEndwith(name);
            if (glist.Count == 0) return null;
            foreach (IGcl mpClass in glist)
            {
                int genericArgCount = GenericUtil.GetGenericTypeArgCount(mpClass.ForType);
                string typeName = mpClass.ShowName;
                string gtypeName = name.Substring(0, name.Length - typeName.Length);
                if (genericArgCount == 1)
                {
                    var gcls = context.SearchGCLFromImport(gtypeName);//searchTypeSymbol(gtypeName);
                    if (gcls.Length == 1)
                    {
                        Type newType = mpClass.ForType.MakeGenericType(new Type[] { gcls[0].ForType });
                        IGcl result = mpClass.CreateNewFor(newType);
                        return result;
                    }
                }
                else if (genericArgCount == 2)
                {
                    List<IGcl> nlist = FindImportEndwith(gtypeName);
                    if (nlist.Count == 0) return null;
                    foreach (var nClass in nlist)
                    {
                        string ntypeName = nClass.ShowName;
                        string ngtypeName = gtypeName.Substring(0, gtypeName.Length - ntypeName.Length); //string ngtypeName = gtypeName.Substring(0, name.Length - ntypeName.Length);
                        //ITypeSymbol ntypeSymbol = searchTypeSymbol(ngtypeName);
                        var gcls = context.SearchGCLFromImport(ngtypeName);//var gcls = context.SearchGCLFromImport(gtypeName);
                        if (gcls.Length == 1)
                        {
                            var gcl0 = gcls[0];
                            Type newType = mpClass.ForType.MakeGenericType(new Type[] { gcl0.ForType, nClass.ForType });
                            IGcl result = mpClass.CreateNewFor(newType); //gcl0.CreateNewFor(newType);
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        List<IGcl> FindGenericEndwith(string name)
        {
            List<IGcl> list = new List<IGcl>();
            foreach (var mpClass in context.GenericTypes)
            {
                string typeName = mpClass.ShowName;
                if (name != typeName && name.EndsWith(typeName))
                {
                    list.Add(mpClass);
                }
            }
            return list;
        }

        List<IGcl> FindImportEndwith(string name)
        {
            List<IGcl> list = new List<IGcl>();
            IGcl typeGcl = searchEndwithFromImportTypes(name);
            if (typeGcl!=null)
                list.Add(typeGcl);
            list.AddRange(searchFromPackage(name));
            return list;
        }

        IGcl searchEndwithFromImportTypes(string name)
        {
            foreach (var igcl in  context.GenericCreatedTypes)
            {
                if (name.EndsWith(igcl.ShowName ))
                    return igcl;
            }
            return null;
        }

        IGcl[] searchFromPackage(string name)
        {
            List<IGcl> list = new List<IGcl>();
            foreach (var igcls in context.importPackages.Values.ToList())
            {
                foreach (var igcl in igcls)
                {
                    if (name.EndsWith(igcl.ShowName))
                        list.Add(igcl);
                }
            }
            return list.ToArray();
        }
    }
}
