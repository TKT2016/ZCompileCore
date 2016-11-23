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
    public class ExternalGcl : IGcl
    {
        public Type MType { get;private set; }
        public CnEnDict WordDict { get; protected set; }

        public string ShowName { get { return GenericUtil.GetGenericTypeShortName(this.MType); } }
        public string RTName { get { return GenericUtil.GetGenericTypeShortName(this.ForType); } }

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
            return GclUtil.SearchConstructor(desc, this.WordDict, this.ForType);
        }

        public ExFieldInfo SearchExField(string name)
        {
            //return this.ForType.GetField(name);
            return GclUtil.SearchExField(name, this.ForType);
        }

        public ExPropertyInfo SearchExProperty(string name)
        {
            return GclUtil.SearchExProperty(name, this.ForType);
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            var methodArray = MType.GetMethods(/*BindingFlags.DeclaredOnly*/ );
            foreach (var method in methodArray)
            {
                if (ReflectionUtil.IsDeclare(MType, method))
                {
                    ExMethodInfo exMethod = new ExMethodInfo(method, method.DeclaringType == MType);
                    TKTProcDesc typeProcDesc = ProcDescHelper.CreateProcDesc(exMethod);
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
