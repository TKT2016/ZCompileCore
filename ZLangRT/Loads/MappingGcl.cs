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
    public class MappingGcl : IGcl
    {
        public Type MType { get; protected set; }
        public Type ForType { get; protected set; }
        public CnEnDict WordDict { get; protected set; }
        public string ShowName { get { return ReflectionUtil.GetGenericTypeShortName(this.MType); } }
        public string RTName { get { return ReflectionUtil.GetGenericTypeShortName(this.ForType); } }

        ProcDescCodeParser parser = new ProcDescCodeParser();

        public MappingGcl(Type mappingedType, CnEnDict wordDict)
        {
            MType = mappingedType;
            ForType = GetForType();
            WordDict = wordDict;
        }

        protected MappingGcl()
        {
           
        }

        public IGcl CreateNewFor(Type forType)
        {
            MappingGcl gcl = new MappingGcl();
            gcl.MType = this.MType;
            gcl.ForType = forType;
            gcl.WordDict = this.WordDict;
            return gcl;
        }

        Type GetForType()
        {
            MappingTypeAttribute attr = Attribute.GetCustomAttribute(MType, typeof(MappingTypeAttribute)) as MappingTypeAttribute;
            if (attr == null) return null;
            return attr.ForType;
        }

        public FieldInfo SearchField(string name)
        {
            var fieldArray = MType.GetFields( );
            foreach (FieldInfo field in fieldArray)
            {
                if (ReflectionUtil.IsDeclare(MType, field))
                {
                    MappingCodeAttribute propertyAttr = Attribute.GetCustomAttribute(field, typeof(MappingCodeAttribute)) as MappingCodeAttribute;
                    if (propertyAttr == null)
                    {
                        if (field.Name == name)
                        {
                            return ForType.GetField(field.Name);
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return ForType.GetField(field.Name);
                        }
                    }
                }
            }
            return null;
        }

        public PropertyInfo SearchProperty(string name)
        {
            var propertyArray = MType.GetProperties(/*BindingFlags.DeclaredOnly*/ );
            foreach(var property in propertyArray)
            {
                if (ReflectionUtil.IsDeclare(MType, property))
                {
                    MappingCodeAttribute propertyAttr = Attribute.GetCustomAttribute(property, typeof(MappingCodeAttribute)) as MappingCodeAttribute;
                    if(propertyAttr==null)
                    {
                        if(property.Name==name)
                        {
                            //return property;
                            return ForType.GetProperty(property.Name);
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return ForType.GetProperty(property.Name);
                        }
                    }
                }
            }
            return null;
        }

        public TKTConstructorDesc SearchConstructor(TKTConstructorDesc bracket)
        {
            return GclHelper.SearchConstructor(bracket, this.WordDict, this.ForType);
            /*
            TKTProcBracket bracket2= bracket;
            if (WordDict != null && bracket.IsNameValue)
            {
                List<TKTProcArg> args = new List<TKTProcArg>();
                foreach (var arg in bracket.ListArgs)
                {
                    string newArgName = WordDict.Get(arg.ArgName);
                    TKTProcArg newArg = new TKTProcArg(newArgName, arg.ArgType, arg.ArgType.IsGenericType);
                    args.Add(newArg);
                }
                bracket2 = new TKTProcBracket(args);
            }
            foreach(ConstructorInfo ci in ForType.GetConstructors())
            {
                if(ci.IsPublic)
                {
                    TKTConstructorDesc bracketCi = ProcDescHelper.CreateProcBracket(ci);
                    return bracketCi;
                }
            }
            return null;*/
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods();
            foreach (var method in methodArray)
            {
                if (ReflectionUtil.IsDeclare(MType, method))
                {
                    MappingCodeAttribute procAttr = Attribute.GetCustomAttribute(method, typeof(MappingCodeAttribute)) as MappingCodeAttribute;
                    if (procAttr == null)
                    {
                        TKTProcDesc typeProcDesc = ProcDescHelper.CreateProcDesc(method);
                        if(typeProcDesc.Eq(procDesc))
                        {
                            MethodInfo rmethod = method;
                            if (rmethod.IsAbstract)
                            {
                                rmethod = searchMethodByMethod(method);
                            }
                            if(rmethod==null)
                            {
                                throw new RTException("方法与被翻译类型的方法不一致");
                            }
                            else
                            {
                                TKTProcDesc rdesc = ProcDescHelper.CreateProcDesc(rmethod);
                                return rdesc;
                            }                              
                        }
                    }
                    else
                    {
                        
                        ParameterInfo[] paramArray = method.GetParameters();
                        foreach(var param in paramArray)
                        {
                            parser.AddType(param.ParameterType);
                        }
                        TKTProcDesc typeProcDesc = parser.Parser(WordDict,procAttr.Code);
                        if (method.IsStatic && !method.IsAbstract && typeProcDesc.HasSubject() && typeProcDesc.GetSubjectArg().ArgType == this.ForType)
                        {
                            typeProcDesc = typeProcDesc.CreateTail();
                        }
                        if(typeProcDesc.Eq(procDesc))
                        {
                            MethodInfo rmethod = method;
                            if (rmethod.IsAbstract)
                            {
                                rmethod = searchMethodByMethod(method);
                            }
                            if (rmethod == null)
                            {
                                throw new RTException("过程描述标注错误");
                            }
                            else
                            {
                                typeProcDesc.Method = rmethod;
                                return typeProcDesc;
                            }             
                        }
                    }
                }
            }
            return null;
        }

        private MethodInfo searchMethodByMethod(MethodInfo method)
        {
            ParameterInfo[] paramArray = method.GetParameters();
            Type[] types = new Type[paramArray.Length];
            for (int i = 0; i < paramArray.Length; i++)
            {
                types[i] = paramArray[i].ParameterType;
            }
            MethodInfo rmethod = ForType.GetMethod(method.Name, types);
            return rmethod;
        }

        public override string ToString()
        {
            return string.Format("TKT映射({0}->{1})",ShowName,RTName);
        }
    }
}
