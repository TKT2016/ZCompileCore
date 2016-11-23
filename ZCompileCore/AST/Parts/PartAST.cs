using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;

namespace ZCompileCore.AST.Parts
{
    public abstract class PartAST : FileElementAST
    {
        public ClassContext ClassContext { get;protected set; }
    }

}
