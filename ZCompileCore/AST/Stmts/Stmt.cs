using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.AST.Parts;
using ZCompileCore.Analys;

namespace ZCompileCore.AST.Stmts
{
    public abstract class Stmt : FileElementAST
    {
        public MethodAST Method { get; set; }
        public AnalyStmtContext AnalyStmtContext { get; set; }

        public abstract void Generate(EmitStmtContext context);

        public virtual void Analy(AnalyStmtContext context)
        {
            this.AnalyStmtContext = context;
        }

        protected string getStmtPrefix()
        {
            StringBuilder buff = new StringBuilder();
            int temp = Deep;
            while (temp > 0)
            {
                buff.Append("  ");
                temp--;
            }
            return buff.ToString();
        }
        protected AnalyExpContext _AnalyExpContext;
        protected Exp AnalyExp(Exp exp)
        {
            exp.Stmt = this;
            if(_AnalyExpContext==null)
             _AnalyExpContext = new AnalyExpContext(this.AnalyStmtContext);
            return exp.Analy(_AnalyExpContext);
        }

        protected void analySubStmt(Stmt stmt)
        {
            stmt.Method = this.Method;
            stmt.Analy(this.AnalyStmtContext);
        }

        protected void MarkSequencePoint(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            if (context.MethodEmitContext.ClassContext.IDoc != null)
            {
                il.MarkSequencePoint(context.MethodEmitContext.ClassContext.IDoc, this.Postion.Line, this.Postion.Col, this.Postion.Line, this.Postion.Col+this.ToCode().Length);
            }
        }
    }
}
