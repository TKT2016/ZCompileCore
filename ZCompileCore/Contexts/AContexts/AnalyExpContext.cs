using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.AContexts
{
    public class AnalyExpContext
    {
        public AnalyStmtContext StmtContext { get; set; }
        public SymbolTable Symbols { get; set; }

        public AnalyExpContext(AnalyStmtContext stmtContext)
        {
            this.StmtContext = stmtContext;
            this.Symbols = stmtContext.Symbols;
        }

        public ClassContext ClassContext
        {
            get
            {
                return this.StmtContext.MethodContext.ClassContext;
            }
        }
    }
}
