using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using ZLangRT;

namespace ZCompileCore.AST.Exps
{
    public class NewLambdaExp : AnalyedCallExp
    {
        ClosureExp lambdaExp;

        public NewLambdaExp(Exp srcExp, Exp actionExp, Type funcType)
            : base(srcExp)
        {
            var nestedFieldExps = new Dictionary<string, VarExp>();
            actionExp.GetNestedFields(nestedFieldExps);
            
            lambdaExp = new ClosureExp(this);
            lambdaExp.FieldExps = nestedFieldExps;
            lambdaExp.BodyExp = actionExp;
            lambdaExp.FnRetType = funcType;
            if (funcType == TKTLambda.ActionType)
            {
                lambdaExp.RetType = typeof(void);
            }
            else if (funcType == TKTLambda.CondtionType)
            {
                lambdaExp.RetType = typeof(bool);
            }
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            lambdaExp.Analy(context); 
            this.RetType = lambdaExp.FnRetType; 
            return this;
        }
       
        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            var symbols = this.AnalyExpContext.Symbols;
            lambdaExp.Generate(context);
            LocalBuilder lanmbdaLocalBuilder = il.DeclareLocal(lambdaExp.NestedClassContext.EmitContext.CurrentTypeBuilder);
            //il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, lambdaExp.NewBuilder);
            EmitHelper.StormVar(il, lanmbdaLocalBuilder);
          
            EmitOutClass(context, lanmbdaLocalBuilder);
            
            foreach (var field in lambdaExp.Fields.Keys)
            {
                EmitHelper.LoadVar(il, lanmbdaLocalBuilder);
                SymbolInfo inMethodSymbol = symbols.Get(field);
                EmitHelper.EmitSymbolGet(il, inMethodSymbol);
                il.Emit(OpCodes.Stfld, (lambdaExp.Fields[field] as SymbolDefField).GetField());
            }
            EmitHelper.LoadVar(il, lanmbdaLocalBuilder);
            il.Emit(OpCodes.Ldftn, lambdaExp.NestedMethodContext.EmitContext.CurrentMethodBuilder);
            ConstructorInfo[] constructorInfos = lambdaExp.FnRetType.GetConstructors();
            il.Emit(OpCodes.Newobj, constructorInfos[0]);
            base.GenerateConv(context);
        }

        void EmitOutClass(EmitExpContext context, LocalBuilder lanmbdaLocalBuilder)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, lanmbdaLocalBuilder);
            if (context.StmtContext.MethodEmitContext.CurrentMethodBuilder.IsStatic)
            {
                il.Emit(OpCodes.Ldnull);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            
            il.Emit(OpCodes.Stfld, (lambdaExp.OutClassField).GetField());
        }



        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {

        }
       
    }
}
