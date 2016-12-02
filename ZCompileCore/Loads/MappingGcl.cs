using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;
using ZLangRT;
using ZLangRT.Attributes;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.Loads
{
    public class MappingGcl : IGcl
    {
        public Type MType { get; protected set; }
        public Type ForType { get; protected set; }
        public CnEnDict WordDict { get; protected set; }
        public string ShowName { get { return GenericUtil.GetGenericTypeShortName(this.MType); } }
        public string RTName { get { return GenericUtil.GetGenericTypeShortName(this.ForType); } }

        public MappingGcl ParentMapping { get; set; }
        ZMappingAttribute attr;
        ProcDescCodeParser parser = new ProcDescCodeParser();

        public MappingGcl(Type mappingedType, CnEnDict wordDict)
        {
            MType = mappingedType;
            WordDict = wordDict;
            initByAttr(wordDict);
        }

        void initByAttr(CnEnDict wordDict)
        {
            var tattr = Attribute.GetCustomAttribute(MType, typeof(ZMappingAttribute));
            if (tattr != null)
            {
                attr = tattr as ZMappingAttribute;
                ForType = attr.ForType;
                if (MType != typeof(事物))
                {
                    if (attr.BaseMappingType != null)
                    {
                        ParentMapping = new MappingGcl(attr.BaseMappingType, wordDict);
                    }
                    else
                    {
                        ParentMapping = new MappingGcl(typeof(事物), wordDict);
                    }
                }
            }
            else
            {
                throw new CompileException("没有找到MappingTypeAttribute");
            }
        }

        public IGcl CreateNewFor(Type forType)
        {
            MappingGcl gcl = new MappingGcl(MType, this.WordDict);
            //gcl.MType = this.MType;
            gcl.ForType = forType;
            //gcl.WordDict = this.WordDict;
            return gcl;
        }

        bool isExFieldInfoByAttr(string name, MemberInfo member)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(member, typeof(ZCodeAttribute));
            if (attrs.Length == 0)
            {
                if (member.Name == name)
                {
                    return true;// return GclUtil.CreatExFieldInfo(ForType.GetField(member.Name), ForType);
                }
            }
            else
            {
                foreach (Attribute attr in attrs)
                {
                    ZCodeAttribute zCodeAttribute = attr as ZCodeAttribute;
                    if (zCodeAttribute.Code == name)
                    {
                        return true;// return GclUtil.CreatExFieldInfo(ForType.GetField(member.Name), ForType);
                    }
                }
            }
            return false;  // return null;
        }

        public ExFieldInfo SearchExField(string name)
        {
            if (this.ForType.IsEnum)
            {
                var fieldArray = this.ForType.GetFields(BindingFlags.Static | BindingFlags.Public );
                foreach (FieldInfo field in fieldArray)
                {
                    if (isExFieldInfoByAttr(name, field))
                    {
                        return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                    }
                    /*ExFieldInfo exFieldInfo = searchExFieldInfoByAttr(name, field);
                    if (exFieldInfo != null)
                    {
                        return exFieldInfo;
                    }*/
                    /*Attribute[] attrs = Attribute.GetCustomAttributes(field, typeof(ZCodeAttribute));
                    if (propertyAttr == null)
                    {
                        if (field.Name == name)
                        {
                            return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                            //return ForType.GetField(field.Name);
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                            //return ForType.GetField(field.Name);
                        }
                    }*/
                }
                return null;
            }
            else
            {
                var fieldArray = MType.GetFields();
                foreach (FieldInfo field in fieldArray)
                {
                    if (ReflectionUtil.IsDeclare(MType, field))
                    {
                        if (isExFieldInfoByAttr(name, field))
                        {
                            return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                        }
                        /*ExFieldInfo exFieldInfo = searchExFieldInfoByAttr(name, field);
                        if (exFieldInfo != null)
                        {
                            return exFieldInfo;
                        }*/
                        /*
                        ZCodeAttribute propertyAttr = Attribute.GetCustomAttribute(field, typeof(ZCodeAttribute)) as ZCodeAttribute;
                        if (propertyAttr == null)
                        {
                            if (field.Name == name)
                            {
                                return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                            }
                        }
                        else
                        {
                            if (propertyAttr.Code == name)
                            {
                                return GclUtil.CreatExFieldInfo(ForType.GetField(field.Name), ForType);
                            }
                        }*/
                    }
                }
                return null;
            }
        }

        public ExPropertyInfo SearchExProperty(string name)
        {
            var propertyArray = MType.GetProperties(/*BindingFlags.DeclaredOnly*/ );
            foreach (PropertyInfo property in propertyArray)
            {
                if (ReflectionUtil.IsDeclare(MType, property))
                {
                    if (isExFieldInfoByAttr(name, property))
                    {
                        return GclUtil.CreatExPropertyInfo(ForType.GetProperty(property.Name), ForType);
                    }
                    /*ExFieldInfo exFieldInfo = searchExFieldInfoByAttr(name, property);
                    if (exFieldInfo != null)
                    {
                        return exFieldInfo;
                    }*/
                    /*ZCodeAttribute propertyAttr = Attribute.GetCustomAttribute(property, typeof(ZCodeAttribute)) as ZCodeAttribute;
                    if (propertyAttr == null)
                    {
                        if (property.Name == name)
                        {
                            //return ForType.GetExProperty(property.Name);
                            return GclUtil.CreatExPropertyInfo(ForType.GetProperty(property.Name), ForType);
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            //return ForType.GetExProperty(property.Name);
                            return GclUtil.CreatExPropertyInfo(ForType.GetProperty(property.Name), ForType);
                        }
                    }*/
                }
            }
            if (isRootMapping())
            {
                return null;
            }
            else
            {
                ExPropertyInfo property = ParentMapping.SearchExProperty(name);
                return property;
            }
        }

        public TKTConstructorDesc SearchConstructor(TKTConstructorDesc bracket)
        {
            return GclUtil.SearchConstructor(bracket, this.WordDict, this.ForType);
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods();
            foreach (var method in methodArray)
            {
                if (ReflectionUtil.IsDeclare(MType, method))
                {
                    //ZCodeAttribute procAttr = Attribute.GetCustomAttribute(method, typeof(ZCodeAttribute)) as ZCodeAttribute;
                    Attribute[] attrs = Attribute.GetCustomAttributes(method, typeof(ZCodeAttribute));
                    if (attrs.Length == 0) // if (procAttr == null)
                    {
                        ExMethodInfo exMethod = GclUtil.CreatExMethodInfo(method, this.ForType);
                        TKTProcDesc typeProcDesc = ProcDescHelper.CreateProcDesc(exMethod);
                        if (typeProcDesc.Eq(procDesc))
                        {
                            MethodInfo rmethod = method;
                            if (rmethod.IsAbstract)
                            {
                                rmethod = searchMethodByMethod(method);
                            }
                            if (rmethod == null)
                            {
                                return null;
                            }
                            else
                            {
                                TKTProcDesc rdesc = ProcDescHelper.CreateProcDesc(exMethod);
                                return rdesc;
                            }
                        }
                    }
                    else
                    {
                        ParameterInfo[] paramArray = method.GetParameters();

                        parser.InitType(ForType, method);
                        foreach (Attribute attr in attrs)
                        {
                            ZCodeAttribute zCodeAttribute = attr as ZCodeAttribute;
                            TKTProcDesc typeProcDesc = parser.Parser(WordDict, zCodeAttribute.Code);
                            if (method.IsStatic && !method.IsAbstract && typeProcDesc.HasSubject() &&
                                typeProcDesc.GetSubjectArg().ArgType == this.ForType)
                            {
                                typeProcDesc = typeProcDesc.CreateTail();
                            }
                            if (typeProcDesc.Eq(procDesc))
                            {
                                MethodInfo rmethod = method;
                                if (rmethod.IsAbstract)
                                {
                                    rmethod = searchMethodByMethod(method);
                                }
                                if (rmethod == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    ExMethodInfo exMethod = GclUtil.CreatExMethodInfo(rmethod, this.ForType);
                                    typeProcDesc.ExMethod = exMethod;
                                    return typeProcDesc;
                                }
                            }
                        }
                    }
                }
            }
            if (isRootMapping())
            {
                return null;
            }
            else
            {
                return ParentMapping.SearchProc(procDesc);
            }
        }

        bool isRootMapping()
        {
            return this.MType== typeof(事物);
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
