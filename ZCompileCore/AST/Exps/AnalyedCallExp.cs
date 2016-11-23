using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;

namespace ZCompileCore.AST.Exps
{
    public abstract class AnalyedCallExp : AnalyedExp
    {
        public AnalyedCallExp(Exp srcExp):base(srcExp)
        {

        }
        public List<Exp> Elements { get; set; }
       
    }
}
