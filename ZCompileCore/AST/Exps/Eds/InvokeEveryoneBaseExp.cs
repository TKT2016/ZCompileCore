using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.AST.Exps.Eds
{
    public abstract class InvokeEveryoneBaseExp : AnalyedCallExp
    {
        protected MethodInfo compareMethod = typeof(Calculater).GetMethod("LEInt", new Type[] { typeof(int), typeof(int) });
       protected int startIndex = 1;
       protected InvokeExp invokeExp;
       protected SymbolVar listSymbol;
       protected SymbolVar elementSymbol;
       protected SymbolVar indexSymbol;
       protected SymbolVar countSymbol;
       protected ExMethodInfo getCountMethod;
       protected ExMethodInfo diMethod;

        public InvokeEveryoneBaseExp(InvokeExp srcExp)
            : base(srcExp)
        {
            invokeExp = srcExp;
            TKTProcArg arg = invokeExp.ExpProcDesc.GetArg(0);
            //everyoneExp = (arg.Value as EveryOneExp);
        }

        protected void createForeachSymbols(AnalyExpContext context, SymbolTable symbols, int foreachIndex, Type listType)
        {
            var listSymbolName = "@everyone_list_" + foreachIndex ;
            var count_symbol_name = "@everyone_count_" + foreachIndex ;
            var indexName = "@everyone_index_" + foreachIndex;
            var elementName = "@everyone_element_" + foreachIndex;

            listSymbol = new SymbolVar(listSymbolName, listType);
            listSymbol.LoacalVarIndex = context.StmtContext.MethodContext.CreateLocalVarIndex(listSymbol.SymbolName);
            symbols.Add(listSymbol);

            Type[] genericTypes = GenericUtil.GetInstanceGenriceType(listType, typeof(列表<>));
            Type ElementType = genericTypes[0];

            elementSymbol = new SymbolVar(elementName, ElementType);
            elementSymbol.LoacalVarIndex = context.StmtContext.MethodContext.CreateLocalVarIndex(elementName);
            elementSymbol.IsInBlock = true;
            symbols.Add(elementSymbol);     
            
            countSymbol = new SymbolVar(count_symbol_name, typeof(int));
            countSymbol.LoacalVarIndex = context.StmtContext.MethodContext.CreateLocalVarIndex(count_symbol_name);
            countSymbol.IsInBlock = true;
            symbols.Add(countSymbol);
           
            indexSymbol = new SymbolVar(indexName, typeof(int));
            indexSymbol.LoacalVarIndex = context.StmtContext.MethodContext.CreateLocalVarIndex(indexName);
            indexSymbol.IsInBlock = true;
            symbols.Add(indexSymbol);
        }

        protected void genInitIndex(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadInt(il, startIndex);
            EmitHelper.StormVar(il, indexSymbol.VarBuilder);
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            foreach (var arg in invokeExp.ExpProcDesc.GetSpecialArgs(0))
            {
                (arg.Value as Exp).GetNestedFields(nestedField);
            }
        }
       
        protected void generateList(EmitExpContext context,Exp exp)
        {
            ILGenerator il = context.ILout;
            exp.Generate(context); //everyoneExp.Generate(context);
            EmitHelper.StormVar(il, listSymbol.VarBuilder);
        }

        protected void generateCount(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, getCountMethod);
            EmitHelper.StormVar(il, countSymbol.VarBuilder);
        }

        protected void generateCondition(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);
            EmitHelper.LoadVar(il, countSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, compareMethod,true);
            EmitHelper.LoadInt(il, 1);
            il.Emit(OpCodes.Ceq);
        }

        protected void generateElement(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, diMethod);
            EmitHelper.StormVar(il, elementSymbol.VarBuilder);
            EmitHelper.LoadVar(il, elementSymbol.VarBuilder);
        }

        protected void genInitIndex(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadInt(il, startIndex);
            EmitHelper.StormVar(il, indexSymbol.VarBuilder);
        }
    }
}