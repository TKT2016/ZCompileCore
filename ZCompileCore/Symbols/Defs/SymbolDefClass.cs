using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Loads;
using ZCompileCore.AST;
using ZCompileCore.AST.Parts;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Collections;
using ZLangRT.Descs;

namespace ZCompileCore.Symbols.Defs
{
    public class SymbolDefClass : SymbolInfo //SymbolDef//, ITypeSymbol
    {
        public TKTProcDescDictionary<MethodContext> MethodDict { get; private set; }

        public Dictionary<string, SymbolDefProperty> PropertyDict { get;private set; }
        public TypeBuilder ClassBuilder { get; set; }
        public IGcl BaseGcl { get; set; }
        public bool IsStatic { get; protected set; }

        //public TKTProcDesc ZeroContructor { get; set; }

        public SymbolDefClass(string name, bool isStatic)
        {
            this.SymbolName = name;
            //MethodDict = new Dictionary<TKTProcDesc, ContextMethod>();
            MethodDict = new TKTProcDescDictionary<MethodContext>();
            PropertyDict = new Dictionary<string, SymbolDefProperty>();
            //ConstructorDict = new Dictionary<TKTProcDesc, ContextConstructor>();
            IsStatic = isStatic;
        }
        
        SymbolDefClass parentClass;
        public SymbolDefClass(SymbolDefClass parentClass,string name)
        {
            this.parentClass = parentClass;
            this.SymbolName = name;
            MethodDict = new TKTProcDescDictionary<MethodContext>();
            PropertyDict = new Dictionary<string, SymbolDefProperty>();
        }

        public Type GetRealType()
        {
            return ClassBuilder;
        }

        public ExPropertyInfo GetExProperty(string name)
        {
            if (PropertyDict.ContainsKey(name))
            {
                return new ExPropertyInfo(  PropertyDict[name].GetProperty(),true);
            }
            return null;
        }

        public bool AddMethod(MethodContext method)
        {
            if (this.MethodDict.Contains(method.ProcDesc))
                return false;
            MethodDict.Add(method.ProcDesc, method);
            return true;
        }

        public TKTProcDesc SearchProc(/*SymbolTable table, */TKTProcDesc procDesc)
        {
            TKTProcDesc proc = MethodDict.SearchProc(procDesc);
            if(proc!=null)
            {
                proc.ExMethod = new ExMethodInfo( MethodDict.Get(proc).EmitContext.CurrentMethodBuilder,true);
            }
            else if (parentClass!=null)
            {
                proc = parentClass.SearchProc(procDesc);
            }
            else if (BaseGcl!=null)
            {
                proc = BaseGcl.SearchProc(procDesc);
            }
            return proc;
        }
        
        
    }
}
