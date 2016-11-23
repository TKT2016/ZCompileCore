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
    public class IfStmt:Stmt
    {
        static Type logicType = typeof(bool);

        public List<IfTrueStmt> Parts = new List<IfTrueStmt>();
        public BlockStmt ElsePart { get; set; }

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyStmtContext.Symbols;
            foreach (var elseif in Parts)
            {
                analySubStmt(elseif);
            }

            if (ElsePart != null)
            {
                ElsePart.Analy(this.AnalyStmtContext);
            }
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;

            Label EndLabel = il.DefineLabel();
            Label ElseLabel = il.DefineLabel();
            List<Label> labels = new List<Label>();
            for (int i = 0; i < Parts.Count; i++)
            {
                labels.Add(il.DefineLabel()); 
            }
            labels.Add(ElseLabel);

            for (int i = 0; i < Parts.Count; i++)
            {
                var item = Parts[i];
                item.EndLabel = EndLabel;
                item.CurrentLabel = labels[i];
                item.NextLabel = labels[i+1];
                item.Generate(context);
            }
            il.MarkLabel(ElseLabel);
            if(ElsePart!=null)
                ElsePart.Generate(context);
            il.MarkLabel(EndLabel);
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            foreach (var tpart in Parts)
            {
                buf.AppendLine(tpart.ToCode());
            }
            if (ElsePart != null)
            {
                buf.Append(getStmtPrefix());
                buf.Append("否则");
                buf.AppendLine();
                buf.Append(ElsePart.ToCode());
                buf.AppendLine();
            }
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return Parts[0].Postion;
            }
        }
        #endregion

        public class IfTrueStmt : Stmt
        {
            public Token KeyToken { get; set; }
            public Exp Condition { get; set; }
            public BlockStmt Body { get; set; }

             public Label CurrentLabel { get; set; }
            public Label NextLabel { get; set; }
            public Label EndLabel { get; set; }

            public override void Analy(AnalyStmtContext context)
            {
                base.Analy(context);
                Condition = AnalyExp(Condition);
                if(Condition==null)
                {
                    TrueAnalyed =false;
                }
                else if( Condition.RetType == null || Condition.RetType != logicType)
                {
                    error(Condition.Postion, "如果语句的条件表达式不是判断表达式");
                }
                analySubStmt(Body); 
            }

            public override void Generate(EmitStmtContext context)
            {
                ILGenerator il = context.ILout;
                EmitExpContext expContext = new EmitExpContext(context);
                il.MarkLabel(CurrentLabel);
                Condition.Generate(expContext);
                EmitHelper.LoadInt(il, 1);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Brfalse, NextLabel);
                Body.Generate(context);
                il.Emit(OpCodes.Br, EndLabel);               
            }

            #region 覆盖
            public override string ToCode()
            {
                StringBuilder buf = new StringBuilder();
                buf.Append(getStmtPrefix());
                buf.AppendFormat("{0}{1}", KeyToken.ToCode(), this.Condition.ToString());
                buf.AppendLine();
                buf.Append(Body.ToString());
                buf.AppendLine();

                return buf.ToString();
            }

            public override CodePostion Postion
            {
                get
                {
                    return KeyToken.Postion;
                }
            }
            #endregion
        }
     
    }
}
