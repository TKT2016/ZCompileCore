using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Analys;
using ZCompileCore.Tools;
using Z语言系统;
using ZLangRT;
using ZCompileCore.AST.Exps;


namespace ZCompileCore.AST.Stmts
{
    public class TryStmt:Stmt
    {
        public override void Analy(AnalyStmtContext context)
        {
            //base.LoadRefTypes(context);
            //var symbols = this.AnalyStmtContext.Symbols;
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            Label tryLabel = il.BeginExceptionBlock();
        }

        #region 覆盖
        public override string ToCode()
        {
            return (getStmtPrefix() + "try");
        }

        public override CodePostion Postion
        {
            get
            {
                return new CodePostion(0,0);
            }
        }
        #endregion

    }
}
