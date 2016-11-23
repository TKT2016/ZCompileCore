using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Analys;

namespace ZCompileCore.AST
{
    public abstract class ASTree
    {
        public int Deep;
        public abstract string ToCode();

        public override string ToString()
        {
            return ToCode();
        }

        protected void error(CodePostion postion, string msg)
        {
            Messager.Error(postion.Line, postion.Col, /*this.GetType().Name + ":" + */ msg);
            TrueAnalyed = false;
        }

        public ASTree()
        {
            TrueAnalyed = true;
        }

        public bool TrueAnalyed { get; set; }

    }
}
