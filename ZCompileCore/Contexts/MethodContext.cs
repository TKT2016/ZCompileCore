using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.EContexts;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.Analys.AContexts
{
    public class MethodContext
    {
        public ClassContext ClassContext { get; set; }
        public TKTProcDesc ProcDesc { get; set; }
        public SymbolTable Symbols { get; set; }
        public int MethodIndex { get; set; }

        public string Name { get; private set; }

        public MethodContext(ClassContext fileContext,string name)
        {
            ClassContext = fileContext;
            EmitContext = new EmitMethodContext();
            Name = name;
            Symbols = fileContext.Symbols.Push(name);
        }

        int LoacalVarIndex = -1;
        public List<string> LoacalVarList = new List<string>();
        public int CreateLocalVarIndex(string name)
        {
            LoacalVarIndex++;
            LoacalVarList.Add(name);
            return LoacalVarIndex;
        }

        public EmitMethodContext EmitContext { get; set; }
        public Type RetType { get; set; }
        public bool RetIsGeneric { get; set; }

        int foreachIndex = -1;
        public int CreateForeachIndex()
        {
            foreachIndex++;
            return foreachIndex;
        }
    }
}
