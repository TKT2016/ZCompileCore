using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Attributes;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.Loads
{
    public class TktGcl : IGcl
    {
        public Type MType { get;private set; }
        public CnEnDict WordDict { get; protected set; }

        public string ShowName { get { return GenericUtil.GetGenericTypeShortName(this.MType); } }
        public string RTName { get { return GenericUtil.GetGenericTypeShortName(this.ForType); } }

        ZClassAttribute classAttr;
        public IGcl ParentMapping { get; set; }

        public Type ForType{ get{ return MType;} }

        public TktGcl(Type mtype,CnEnDict wordDict)
        {
            MType = mtype;
            WordDict = wordDict;
            classAttr = getTKTAttr(MType);

            if (classAttr.ParentMappingType != null)
            {
                ParentMapping = GclUtil.Load(classAttr.ParentMappingType, wordDict);
            }
            else
            {
                if (MType.BaseType != null)
                {
                    ZClassAttribute attr = getTKTAttr(MType.BaseType);
                    if(attr!=null)
                    {
                        ParentMapping = GclUtil.Load(MType.BaseType, wordDict);
                    }
                }
            }
            if(ParentMapping==null)
            {
                ParentMapping = GclUtil.Load(typeof(事物), wordDict);
            }
        }
        ZClassAttribute getTKTAttr(Type type)
        {
            var attr = Attribute.GetCustomAttribute(MType, typeof(ZClassAttribute));
            if(attr==null) return null;
            return attr as ZClassAttribute;
        }
        
        public IGcl CreateNewFor(Type forType)
        {
            TktGcl gcl = new TktGcl(forType,this.WordDict);
            return gcl;
        }

        public TKTConstructorDesc SearchConstructor(TKTConstructorDesc desc)
        {
            return GclUtil.SearchConstructor(desc, this.WordDict, this.ForType);
        }

        public ExFieldInfo SearchExField(string name)
        {
            var fieldArray = MType.GetFields();
            foreach (FieldInfo field in fieldArray)
            {
                if (ReflectionUtil.IsDeclare(MType, field))
                {
                    ZCodeAttribute propertyAttr = Attribute.GetCustomAttribute(field, typeof(ZCodeAttribute)) as ZCodeAttribute;
                    if (propertyAttr == null)
                    {
                        if (field.Name == name)
                        {
                            return GclUtil.CreatExFieldInfo(field, ForType);
                            //return field;
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return GclUtil.CreatExFieldInfo(field, ForType);
                            //return field;
                        }
                    }
                }
            }
            if(ParentMapping!=null)
            {
                return ParentMapping.SearchExField(name);
            }
            return null;
        }

        public ExPropertyInfo SearchExProperty(string name)
        {
            var propertyArray = MType.GetProperties(/*BindingFlags.DeclaredOnly*/ );
            foreach(var property in propertyArray)
            {
                if (ReflectionUtil.IsDeclare(MType, property))
                {
                    ZCodeAttribute propertyAttr = Attribute.GetCustomAttribute(property, typeof(ZCodeAttribute)) as ZCodeAttribute;
                    if(propertyAttr==null)
                    {
                        if(property.Name==name)
                        {
                            return GclUtil.CreatExPropertyInfo(property, ForType);
                            //return property;
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return GclUtil.CreatExPropertyInfo(property, ForType);
                            //return property;
                        }
                    }
                }
            }
            if (ParentMapping != null)
            {
                return ParentMapping.SearchExProperty(name);
            }
            return null;
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods( );
            foreach (var method in methodArray)
            {
                ZCodeAttribute procAttr = Attribute.GetCustomAttribute(method, typeof(ZCodeAttribute)) as ZCodeAttribute;
                if (procAttr == null)
                {
                    ExMethodInfo exMethod = GclUtil.CreatExMethodInfo(method, this.ForType);
                    TKTProcDesc typeProcDesc = ProcDescHelper.CreateProcDesc(exMethod);
                    if (typeProcDesc.Eq(procDesc))
                    {
                        return typeProcDesc;
                    }
                }
                else
                {
                    ProcDescCodeParser parser = new ProcDescCodeParser();
                    parser.InitType(ForType, method);              
                    TKTProcDesc typeProcDesc = parser.Parser(WordDict, procAttr.Code);
                    if (typeProcDesc.Eq(procDesc))
                    {
                        ExMethodInfo exMethod = GclUtil.CreatExMethodInfo(method, this.ForType);
                        typeProcDesc.ExMethod = exMethod;
                        return typeProcDesc;
                    }
                }
            }
            if (ParentMapping != null)
            {
                return ParentMapping.SearchProc(procDesc);
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("TKT类({0})", ShowName);
        }
    }
}
