using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.AContexts
{
    public class AnalyStmtContext
    {
        public MethodContext MethodContext { get; set; }
        public SymbolTable Symbols { get; set; }
        public string Name { get; private set; }

        public AnalyStmtContext(MethodContext methodContext,string name)
        {
            MethodContext = methodContext;
            Name = name;
            //this.Symbols = methodContext.Symbols.Push(Name);
            this.Symbols = methodContext.Symbols;
        }

        public ClassContext ClassContext
        {
            get
            {
                return this.MethodContext.ClassContext;
            }
        }
    }

}
