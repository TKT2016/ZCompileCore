using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;

namespace ZCompileCore.AST.Exps
{
    public abstract class AnalyedExp:Exp
    {
        protected Exp SrcExp;
        public AnalyedExp(Exp srcExp)
        {
            SrcExp = srcExp;
        }

        public override string ToCode()
        {
            return SrcExp.ToCode();
        }

        public override CodePostion Postion
        {
            get
            {
                return SrcExp.Postion;
            }
        }
    }
}
