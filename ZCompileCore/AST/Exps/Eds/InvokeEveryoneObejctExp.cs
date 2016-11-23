using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Loads;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.AST.Exps.Eds
{
    public class InvokeEveryoneObejctExp : InvokeEveryoneBaseExp
    {
        protected EveryOneExp everyoneExp;

        public InvokeEveryoneObejctExp(InvokeExp srcExp)
            : base(srcExp)
        {
            //invokeExp = srcExp;
            TKTProcArg arg = invokeExp.ExpProcDesc.GetArg(0);
            everyoneExp = (arg.Value as EveryOneExp);
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            int foreachIndex = context.StmtContext.MethodContext.CreateForeachIndex();
            var symbols = context.Symbols;
            Exp listExp = everyoneExp.ListExp;
            //PropertyInfo countProperty = listExp.RetType.GetProperty("Count");
            ExPropertyInfo countProperty = GclUtil.SearchExProperty("Count", listExp.RetType);

            var rgetCountMethod = countProperty.Property.GetGetMethod();
            getCountMethod = new ExMethodInfo(rgetCountMethod, countProperty.IsSelf);
            //PropertyInfo itemProperty = listExp.RetType.GetProperty("Item");
            ExPropertyInfo itemProperty = GclUtil.SearchExProperty("Item", listExp.RetType);

            diMethod = new ExMethodInfo(itemProperty.Property.GetGetMethod(), itemProperty.IsSelf);

            createForeachSymbols(context, symbols, foreachIndex, everyoneExp.ListExp.RetType);
            this.RetType = invokeExp.CallExMethod.Method.ReturnType;
            return this;
        }
    
        public override void Generate(EmitExpContext context)
        {
            generateEveryOne(context);
            base.GenerateConv(context);
        }

        void generateEveryOne(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            var True_Label = il.DefineLabel();
            var False_Label = il.DefineLabel();

            generateList(context, everyoneExp);
            generateCount(context);
            genInitIndex(context);

            generateCondition(context);
            il.Emit(OpCodes.Brfalse, False_Label);

            //定义一个标签，表示从下面开始进入循环体
            il.MarkLabel(True_Label);
            genBody(context);
            EmitHelper.Inc(il, indexSymbol.VarBuilder);
            generateCondition(context);
            il.Emit(OpCodes.Brtrue, True_Label);
            il.MarkLabel(False_Label); 
        }

        void genBody(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            invokeExp.GenerateSubject(context);
            generateElement(context);
            invokeExp.GenerateFirstArgs(context);//invokeExp.GenerateArgILBox(context, 0);
            EmitHelper.CallDynamic(il, invokeExp.CallExMethod);
        }
      
    }
}
