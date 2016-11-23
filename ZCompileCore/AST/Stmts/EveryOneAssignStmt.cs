using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZLangRT;
using System.Reflection;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZLangRT.Utils;
using ZCompileCore.AST.Exps.Eds;
using ZCompileCore.Symbols;
using ZLangRT.Descs;
using Z语言系统;

namespace ZCompileCore.AST.Stmts
{
    public class EveryOneAssignStmt:Stmt
    {
        public EveryOneExp ToExp { get; set; }
        public Exp ValueExp { get; set; }
        public bool IsAssignTo { get; set; }
        public ExPropertyInfo ExProperty;
        int startIndex = 1;
        MethodInfo compareMethod = typeof(Calculater).GetMethod("LEInt", new Type[] { typeof(int), typeof(int) });
        SymbolVar listSymbol;
        SymbolVar elementSymbol;
        SymbolVar indexSymbol;
        SymbolVar countSymbol;
        MethodInfo getCountMethod;
        MethodInfo setMethod = null;

        public EveryOneAssignStmt(AssignStmt assignStmt)
        {
            this.ToExp = assignStmt.LeftToExp as EveryOneExp;
            this.ValueExp = assignStmt.RightValueExp;
            this.IsAssignTo = assignStmt.IsAssignTo;
        }

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyStmtContext.Symbols;
            int foreachIndex = context.MethodContext.CreateForeachIndex();
            var propertyName = "Item";
            var subjType = ToExp.ListExp.RetType;
            ExProperty = GclUtil.SearchExProperty(propertyName, subjType);//subjType.GetExProperty(propertyName);
            PropertyInfo countProperty = subjType.GetProperty("Count");
            getCountMethod = countProperty.GetGetMethod();
            createForeachSymbols(context, symbols, foreachIndex);
            setMethod = ExProperty.Property.GetSetMethod();
        }

        void createForeachSymbols(AnalyStmtContext context, SymbolTable symbols, int foreachIndex)
        {
            var listSymbolName = "@everyone_list_" + foreachIndex;
            var count_symbol_name = "@everyone_count_" + foreachIndex;
            var indexName = "@everyone_index_" + foreachIndex;
            var elementName = "@everyone_element_" + foreachIndex;

            listSymbol = new SymbolVar(listSymbolName, ToExp.ListExp.RetType);
            listSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(listSymbol.SymbolName);
            symbols.Add(listSymbol);

            Type[] genericTypes = GenericUtil.GetInstanceGenriceType(ToExp.ListExp.RetType, typeof(列表<>));
            Type ElementType = genericTypes[0];

            elementSymbol = new SymbolVar(elementName, ElementType);
            elementSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(elementName);
            elementSymbol.IsInBlock = true;
            symbols.Add(elementSymbol);

            countSymbol = new SymbolVar(count_symbol_name, typeof(int));
            countSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(count_symbol_name);
            countSymbol.IsInBlock = true;
            symbols.Add(countSymbol);

            indexSymbol = new SymbolVar(indexName, typeof(int));
            indexSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(indexName);
            indexSymbol.IsInBlock = true;
            symbols.Add(indexSymbol);
        }
        ILGenerator IL =null;
        public override void Generate(EmitStmtContext context)
        {
            IL = context.ILout;
            MarkSequencePoint(context);

            EmitExpContext expContext = new EmitExpContext(context);

            var True_Label = IL.DefineLabel();
            var False_Label = IL.DefineLabel();

            generateList(expContext);
            generateCount(context);
            genInitIndex(context);

            generateCondition(context);
            IL.Emit(OpCodes.Brfalse, False_Label);

            //定义一个标签，表示从下面开始进入循环体
            IL.MarkLabel(True_Label);
            generateSet(expContext,ValueExp);
            EmitHelper.Inc(IL, indexSymbol.VarBuilder);
            generateCondition(context);
            IL.Emit(OpCodes.Brtrue, True_Label);
            IL.MarkLabel(False_Label);
        }
        
        void generateSet(EmitExpContext context, Exp valueExpr)
        {
            EmitHelper.LoadVar(IL, listSymbol.VarBuilder);
            EmitHelper.LoadVar(IL, indexSymbol.VarBuilder);
            ValueExp.Generate(context);
            EmitHelper.CallDynamic(IL, setMethod,ExProperty.IsSelf);
        }

        void generateList(EmitExpContext context)
        {
            ToExp.ListExp.Generate(context);
            EmitHelper.StormVar(IL, listSymbol.VarBuilder);
        }

        void generateCount(EmitStmtContext context)
        {
            EmitHelper.LoadVar(IL, listSymbol.VarBuilder);
            EmitHelper.CallDynamic(IL, getCountMethod,ExProperty.IsSelf);
            EmitHelper.StormVar(IL, countSymbol.VarBuilder);
        }

        void generateCondition(EmitStmtContext context)
        {
            EmitHelper.LoadVar(IL, indexSymbol.VarBuilder);
            EmitHelper.LoadVar(IL, countSymbol.VarBuilder);
            EmitHelper.CallDynamic(IL, compareMethod,true);
            EmitHelper.LoadInt(IL, 1);
            IL.Emit(OpCodes.Ceq);
        }

        void genInitIndex(EmitStmtContext context)
        {
            EmitHelper.LoadInt(IL, startIndex);
            EmitHelper.StormVar(IL, indexSymbol.VarBuilder);
        }

        #region 覆盖
        public override string ToCode()
        {
            string valueCode = ValueExp == null ? "" : ValueExp.ToCode();
            string toCode = ToExp == null ? "" : ToExp.ToCode();

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
                return ToExp.Postion;
            }
        }
        #endregion
    }
}
