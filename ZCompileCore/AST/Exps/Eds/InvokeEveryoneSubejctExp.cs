using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Loads;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.AST.Exps.Eds
{
    public class InvokeEveryoneSubejctExp : InvokeEveryoneBaseExp
    {
        //InvokeExp invokeExp;
        public InvokeEveryoneSubejctExp(InvokeExp srcExp)
            : base(srcExp)
        {
            //invokeExp = srcExp;
        }

        public Exp ListExp { get; set; }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            int foreachIndex = context.StmtContext.MethodContext.CreateForeachIndex();
            var symbols = context.Symbols;
            ListExp = invokeExp.SubjectExp;

            ExPropertyInfo countProperty = GclUtil.SearchExProperty("Count", ListExp.RetType);//ListExp.RetType.GetProperty("Count");
            //getCountMethod = countProperty.GetGetMethod();
            var rgetCountMethod = countProperty.Property.GetGetMethod();
            getCountMethod = new ExMethodInfo(rgetCountMethod, countProperty.IsSelf);

            ExPropertyInfo itemProperty = GclUtil.SearchExProperty("Item", ListExp.RetType);
            //PropertyInfo itemProperty = ListExp.RetType.GetProperty("Item");
            diMethod = new ExMethodInfo( itemProperty.Property.GetGetMethod(),itemProperty.IsSelf);

            createForeachSymbols(context, symbols, foreachIndex, ListExp.RetType);
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

            generateList(context, ListExp);
            generateCount(context);
            genInitIndex(context);

            generateCondition(context);
            il.Emit(OpCodes.Brfalse, False_Label);

            //定义一个标签，表示从下面开始进入循环体
            il.MarkLabel(True_Label);
            //generateElement(context);
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
            invokeExp.GenerateArgs(context);
            EmitHelper.CallDynamic(il, invokeExp.CallExMethod);
        }
        /*
        void generateList(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            ListExp.Generate(context);
            EmitHelper.StormVar(il, listSymbol.VarBuilder);
        }*/
        /*
        void generateCount(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, getCountMethod);
            EmitHelper.StormVar(il, countSymbol.VarBuilder);
        }*/
        /*
        void generateCondition(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);
            EmitHelper.LoadVar(il, countSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, compareMethod);
            EmitHelper.LoadInt(il, 1);
            il.Emit(OpCodes.Ceq);
        }*/
        /*
        void generateElement(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, diMethod);
            EmitHelper.StormVar(il, elementSymbol.VarBuilder);
        }*/
        /*
        void genInitIndex(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadInt(il, startIndex);
            EmitHelper.StormVar(il, indexSymbol.VarBuilder);
        }*/
    }
}
