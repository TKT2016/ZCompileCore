using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;
using ZCompileCore.Analys;
using ZCompileCore.Tools;
using ZLangRT;

namespace ZCompileCore.AST.Stmts
{
    public class CallStmt:Stmt
    {
        public Exp CallExpr { get; set; }
        Exp resultExp;

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            resultExp = AnalyExp(CallExpr);   
            if(resultExp==null)
            {
                TrueAnalyed = false;
            }
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            MarkSequencePoint(context);
            EmitExpContext expContext = new EmitExpContext(context);
            resultExp.Generate(expContext);
            if (resultExp.RetType != typeof(void))
            {
                il.Emit(OpCodes.Pop);
            }
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buff = new StringBuilder();
            if (CallExpr != null)
            {
                buff.Append(getStmtPrefix());
                buff.Append(CallExpr.ToCode());
            }
            buff.Append(";");
            return buff.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return CallExpr.Postion;
            }
        }
        #endregion
    }
}
