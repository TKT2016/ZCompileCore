using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitExpContext
    {
        public EmitStmtContext StmtContext { get; set; }

        public ILGenerator ILout { get { return this.StmtContext.ILout; } }

        public EmitExpContext(EmitStmtContext stmtContext)
        {
            StmtContext = stmtContext;
        }
    }
}
