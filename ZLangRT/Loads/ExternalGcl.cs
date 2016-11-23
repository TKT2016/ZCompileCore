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
    public class ExternalGcl : IGcl
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

        public ExternalGcl(Type mtype,CnEnDict wordDict)
        {
            MType = mtype;
            WordDict = wordDict;
        }

        protected ExternalGcl()
        {
           
        }

        public IGcl CreateNewFor(Type forType)
        {
            ExternalGcl gcl = new ExternalGcl();
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
            return this.ForType.GetField(name);
        }

        public PropertyInfo SearchProperty(string name)
        {
            return this.ForType.GetProperty(name);
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods(/*BindingFlags.DeclaredOnly*/ );
            foreach (var method in methodArray)
            {
                if (ReflectionUtil.IsDeclare(MType, method))
                {

                    TKTProcDesc typeProcDesc = ProcDescHelper.CreateProcDesc(method);
                    if (typeProcDesc.Eq(procDesc))
                    {
                        return typeProcDesc;
                    }

                }
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("ExternalGcl({0})", ShowName);
        }
    }
}
