using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys;
using ZCompileCore.Tools;
using ZLangRT;
using System.Reflection;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.AST.Exps.Eds;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZLangRT.Utils;
using ZCompileCore.Symbols.Defs;

namespace ZCompileCore.AST.Stmts
{
    public class AssignStmt:Stmt
    {
        public Exp LeftToExp { get; set; }
        public Exp RightValueExp { get; set; }
        public bool IsAssignTo { get; set; }

        bool isEveryOneAssign = false;

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyStmtContext.Symbols;
            RightValueExp.IsAssignedValue = true;
            LeftToExp.IsAssignedBy = true;
            RightValueExp = AnalyExp(RightValueExp);
            LeftToExp = AnalyExp(LeftToExp);
            
            if (LeftToExp == null || !LeftToExp.TrueAnalyed || RightValueExp == null || !RightValueExp.TrueAnalyed)
            {
                TrueAnalyed = false;
                return;
            }
            else
            {
                RightValueExp.RequireType = LeftToExp.RetType;
            }
            
            if (LeftToExp is VarExp)
            {
                var identExpr = LeftToExp as VarExp;
                string idname = identExpr.VarName;
                SymbolInfo symbol = identExpr.VarSymbol;
                if(symbol!=null)
                {
                    if (symbol is InstanceSymbol)
                    {
                        InstanceSymbol varSymbol = (symbol as InstanceSymbol);
                        if(varSymbol.DimType==null)
                        {
                            varSymbol.DimType = RightValueExp.RetType;
                            varSymbol.IsAssigned = true;
                        }                       
                    }
                }
                else
                {
                    throw new CompileException(idname+"没有分析出正确类型");
                }
            }
            if(!(LeftToExp is IGenerateSet))
            {
                error("不能赋值");
            }
            else
            {
                var gs = (LeftToExp as IGenerateSet);
                if(gs.CanWrite==false)
                {
                    errorf("'{0}'是只读的，不能赋值",gs.ToString());
                }
            }

            if (TKTLambda.IsFn(LeftToExp.RetType))// (!isFn())
            {
                analyArgLanmbda(_AnalyExpContext);
            }
            else
            {
                if (RightValueExp.RetType != null && LeftToExp.RetType != null)
                {
                    if (!ReflectionUtil.IsExtends(RightValueExp.RetType, LeftToExp.RetType))
                    {
                        error(LeftToExp.Postion, "左右赋值类型不匹配,不能赋值");
                    }
                }
            }

             if(LeftToExp is EveryOneExp)
             {
                 isEveryOneAssign = true;
                 everyOneAssignStmt = new EveryOneAssignStmt(this);
                 everyOneAssignStmt.Analy(context);
             }
        }

        EveryOneAssignStmt everyOneAssignStmt = null;

        void analyArgLanmbda(AnalyExpContext context)
        {
            var totype = LeftToExp.RetType;
            NewLambdaExp newLambdaExp = new NewLambdaExp(RightValueExp, RightValueExp, totype);
            RightValueExp = newLambdaExp;
            newLambdaExp.Analy(context);
        }

        public override void Generate(EmitStmtContext context)
        {
            MarkSequencePoint(context);
            if (isEveryOneAssign)
            {
                everyOneAssignStmt.Generate(context);
            }
            else
            {
                generateNormal(context);
            }
        }

        void generateNormal(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
           
            EmitExpContext expContext = new EmitExpContext(context);
            (LeftToExp as IGenerateSet).GenerateSet(expContext, RightValueExp);
        }

        void generateEveryOne(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;

            EveryOneExp everyoneExp = LeftToExp as EveryOneExp;
            EmitExpContext expContext = new EmitExpContext(context);
            (LeftToExp as IGenerateSet).GenerateSet(expContext, RightValueExp);
        }

        #region 覆盖
        public override string ToCode()
        {
            string valueCode = RightValueExp == null ? "" : RightValueExp.ToCode();
            string toCode = LeftToExp == null ? "" : LeftToExp.ToCode();

            if (IsAssignTo)
            {
                return string.Format("{0}{1} => {2} ;",getStmtPrefix(), valueCode, toCode );
            }
            else
            {
                return string.Format("{0}{1} = {2} ;", getStmtPrefix(), toCode, valueCode);
            }
        }

        public override CodePostion Postion
        {
            get
            {
                return LeftToExp.Postion;
            }
        }
        #endregion
    }
}
