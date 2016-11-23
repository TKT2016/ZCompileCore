using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using Z语言系统;
using ZLangRT;
using ZCompileCore.AST.Exps;

namespace ZCompileCore.AST.Stmts
{
    public class CatchStmt:Stmt
    {
        public Token CatchToken { get; set; }
        public Token ExceptionTypeToken { get; set; }
        public Token ExceptionNameToken { get; set; }
        public BlockStmt CatchBody { get; set; }

        string exTypeName;
        string exName;
        Type exType;
        SymbolVar exSymbol;

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyStmtContext.Symbols;
            exTypeName = ExceptionTypeToken.GetText();
            exName = ExceptionNameToken.GetText();
            exType = context.MethodContext.ClassContext.SearchType(exTypeName).ForType;
            if(exType==null)
            {
                errorf(ExceptionTypeToken.Postion,"类型'{0}'不存在",exTypeName);
            }
            var exSymbol2 = symbols.Get(exName);
            if (exSymbol2 == null)
            {
                exSymbol = new SymbolVar(exName, exType);
                exSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(exName);
            }
            else
            {
                if(exSymbol2 is SymbolVar)
                {
                    exSymbol = exSymbol2 as SymbolVar;
                    if(exSymbol.DimType!=exType)
                    {
                        errorf(ExceptionNameToken.Postion, "变量'{0}'的类型与异常的类型不一致", exName);
                    }
                }
                else
                {
                    errorf(ExceptionNameToken.Postion, "变量名称'{0}'已经使用过", exName);
                }
            }
            symbols.Add(exSymbol);
            analySubStmt(CatchBody);
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            MarkSequencePoint(context);
            il.BeginCatchBlock(exType);
            EmitHelper.StormVar(il, exSymbol.VarBuilder);
            CatchBody.Generate(context);
            il.EndExceptionBlock();
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(getStmtPrefix());
            buf.AppendFormat("{0}({1}:{2})", CatchToken.GetText(),ExceptionTypeToken.GetText(),ExceptionNameToken.GetText());
            buf.AppendLine();
            buf.Append(CatchBody.ToString());
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return CatchToken.Postion;
            }
        }
        #endregion

    }
}
