using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using Z语言系统;
using ZLangRT;


namespace ZCompileCore.AST.Stmts
{
    public class WhileStmt:Stmt
    {
        public Token WhileToken { get; set; }
        public Exp Condition { get; set; }
        public BlockStmt Body { get; set; }

        Type logicType = typeof(bool);

        public WhileStmt()
        {
            
        }

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyStmtContext.Symbols;
            Condition=AnalyExp(Condition);
            if (Condition == null)
            {
                TrueAnalyed = false;
            }
            else if (Condition.RetType == null || Condition.RetType != logicType)
            {
                error(Condition.Postion, "循环当语句的条件表达式不是判断表达式");
            }
            analySubStmt(Body);
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitExpContext expContext = new EmitExpContext(context);
            var True_Label = il.DefineLabel();
            var False_Label = il.DefineLabel();

            il.MarkLabel(True_Label);
            Condition.Generate(expContext);
            EmitHelper.LoadInt(il, 1);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, False_Label);
            Body.Generate(context);
            il.Emit(OpCodes.Br, True_Label);
            il.MarkLabel(False_Label);
        }

        #region 位置格式化
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(getStmtPrefix());
            buf.AppendFormat("循环当( {0} )", this.Condition.ToCode());
            buf.AppendLine();
            buf.Append(Body.ToString());
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return WhileToken.Postion;
            }
        }
        #endregion
    }
}
