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
    public class TktGcl : IGcl
    {
        public Type MType { get;private set; }
        public CnEnDict WordDict { get; protected set; }

        public string ShowName { get {return ReflectionUtil.GetGenericTypeShortName(this.MType); } }
        public string RTName { get { return ReflectionUtil.GetGenericTypeShortName(this.ForType); } }

        public Type ForType
        {
            get
            {
                return MType;
            }
        }

        public TktGcl(Type mtype,CnEnDict wordDict)
        {
            MType = mtype;
            WordDict = wordDict;
        }

        protected TktGcl()
        {
           
        }

        public IGcl CreateNewFor(Type forType)
        {
            TktGcl gcl = new TktGcl();
            gcl.MType = forType;
            //gcl.ForType = forType;
            gcl.WordDict = this.WordDict;
            return gcl;
        }

        public TKTConstructorDesc SearchConstructor(TKTConstructorDesc desc)
        {
            return GclHelper.SearchConstructor(desc, this.WordDict, this.ForType);
        }

        public FieldInfo SearchField(string name)
        {
            var fieldArray = MType.GetFields();
            foreach (FieldInfo field in fieldArray)
            {
                if (ReflectionUtil.IsDeclare(MType, field))
                {
                    MappingCodeAttribute propertyAttr = Attribute.GetCustomAttribute(field, typeof(MappingCodeAttribute)) as MappingCodeAttribute;
                    if (propertyAttr == null)
                    {
                        if (field.Name == name)
                        {
                            return field;
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return field;
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
                            return property;
                        }
                    }
                    else
                    {
                        if (propertyAttr.Code == name)
                        {
                            return property;
                        }
                    }
                }
            }
            return null;
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods(/*BindingFlags.DeclaredOnly*/ );
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
                            return typeProcDesc;
                        }
                    }
                    else
                    {
                        ProcDescCodeParser parser = new ProcDescCodeParser();
                        ParameterInfo[] paramArray = method.GetParameters();
                        foreach(var param in paramArray)
                        {
                            parser.AddType(param.ParameterType);
                        }
                        TKTProcDesc typeProcDesc = parser.Parser(WordDict,procAttr.Code);
                        if(typeProcDesc.Eq(procDesc))
                        {
                            typeProcDesc.Method = method;
                            return typeProcDesc;
                        }
                    }
                }
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("TKT类({0})", ShowName);
        }
    }
}
