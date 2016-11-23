using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Symbols;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Exps
{
    public abstract class Exp : FileElementAST
    {
        public Stmt Stmt { get; set; }
        public AnalyExpContext AnalyExpContext { get; set; }
        public EmitExpContext EmitExpContext { get; set; }

        public Type RetType { get; set; }

        public abstract void Generate(EmitExpContext context);

        /// <summary>
        /// 表达式是否是被赋值
        /// </summary>
        public bool IsAssignedBy { get; set; }
        /// <summary>
        /// 表达式是否是赋值表达式
        /// </summary>
        public bool IsAssignedValue { get; set; }

        public virtual Exp Analy(AnalyExpContext context)
        {
            this.AnalyExpContext = context;
            return this;
        }

        public abstract void GetNestedFields(Dictionary<string, VarExp> nestedField);

        /// <summary>
        /// 期望的类型
        /// </summary>
        public Type RequireType { get; set; }


        
        protected void GenerateConv(EmitExpContext context)
        {
            EmitHelper.EmitConv(context.ILout, RequireType, RetType);
        }

        protected void GenerateArgsExp(EmitExpContext context, ParameterInfo[] paramInfos, Exp[] args)
        {
            var size = paramInfos.Length;

            for (int i = 0; i < size; i++)
            {
                Exp argExp = args[i];
                ParameterInfo parameter = paramInfos[i];
                argExp.RequireType = parameter.ParameterType;
                argExp.Generate(context);
            }
        }

        protected void GenerateArgsExp(EmitExpContext context, TKTProcDesc expProcDesc, TKTProcDesc searchedProcDesc)
        {
            var size = expProcDesc.ArgCount; ;

            for (int i = 0; i < size; i++)
            {
                var arg = expProcDesc.GetArg(i);
                var exp = (arg.Value as Exp);
                TKTProcArg procArg = searchedProcDesc.GetArg(i);
                exp.RequireType = procArg.ArgType;
                exp.Generate(context);
            }
        }

        protected void GenerateArgsExp(EmitExpContext context, TKTProcDesc expProcDesc, TKTProcDesc searchedProcDesc,int size)
        {
            for (int i = 0; i < size; i++)
            {
                var arg = expProcDesc.GetArg(i);
                var exp = (arg.Value as Exp);
                TKTProcArg procArg = searchedProcDesc.GetArg(i);
                exp.RequireType = procArg.ArgType;
                exp.Generate(context);
            }
        }
    }
}
