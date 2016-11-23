using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Z语言系统;
using ZLangRT;
using ZLangRT.Utils;
using System.Reflection;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZLangRT.Descs;


namespace ZCompileCore.AST.Stmts
{
    public class ForeachStmt:Stmt
    {
        public Token ForeachToken { get; set; }
        public Exp ListExp { get; set; }
        public Token ElementToken { get; set; }
        public Token IndexToken { get; set; }
        public BlockStmt Body { get; set; }

        string elementName;
        string indexName;

        SymbolVar listSymbol;
        SymbolVar elementSymbol;
        SymbolVar indexSymbol;
        SymbolVar countSymbol;
        ExMethodInfo getCountMethod;
        ExMethodInfo diMethod;
        MethodInfo compareMethod;

        int startIndex;

        public ForeachStmt()
        {
            
        }

        public override void Analy(AnalyStmtContext context)
        {
            //base.LoadRefTypes(context);
            int foreachIndex = context.MethodContext.CreateForeachIndex();
            if (ListExp == null)
            {
                error("'循环每一个语句'不存在要循环的列表");
            }
            if (ElementToken == null)
            {
                error("'循环每一个语句'不存在成员名称");
            }
            if (ListExp == null || ElementToken==null)
            {
                return;
            }

            this.AnalyStmtContext = context;
            var symbols = context.Symbols;
            ListExp = AnalyExp(ListExp);

            if (ListExp == null)
            {
                TrueAnalyed = false;
                return;
            }
            if (ListExp.RetType == null)
            {
                TrueAnalyed = false;
                return;
            }
            else if ( !canForeach(ListExp.RetType))
            {
                error(ListExp.Postion, "该结果不能用作循环每一个");
                return;
            }

            if (ReflectionUtil.IsExtends(ListExp.RetType, typeof(列表<>)))
            {
                startIndex = 1;
                compareMethod = typeof(Calculater).GetMethod("LEInt", new Type[] { typeof(int), typeof(int) });
            }
            else
            {
                startIndex = 0;
                compareMethod = typeof(Calculater).GetMethod("LTInt", new Type[] { typeof(int), typeof(int) });
            }

            //PropertyInfo countProperty = ListExp.RetType.GetProperty("Count");
            ExPropertyInfo countProperty = GclUtil.SearchExProperty("Count", ListExp.RetType);
            getCountMethod = new ExMethodInfo(countProperty.Property.GetGetMethod(), countProperty.IsSelf);
            //PropertyInfo itemProperty = ListExp.RetType.GetProperty("Item");
            ExPropertyInfo itemProperty = GclUtil.SearchExProperty("Item", ListExp.RetType);
            diMethod = new ExMethodInfo(itemProperty.Property.GetGetMethod(), itemProperty.IsSelf);

            elementName = ElementToken.GetText();

            if(IndexToken!=null)
            {
                indexName = IndexToken.GetText();
            }
            else
            {
                indexName = "$$$foreach_" + foreachIndex+"_index";
            }
            createForeachSymbols(context, symbols, foreachIndex, ListExp.RetType);
            analySubStmt(Body);
        }

        void createForeachSymbols(AnalyStmtContext context, SymbolTable symbols, int foreachIndex, Type listType)
        {
            var listSymbolName = "$$$foreach_" + foreachIndex + "_list";
            var symbol = symbols.Get(listSymbolName);
            if(symbol==null)
            {
                listSymbol = new SymbolVar(listSymbolName, listType);
                listSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(listSymbol.SymbolName);
                symbols.Add(listSymbol);
            }
            else
            {
                listSymbol = symbol as SymbolVar;
                if(listSymbol==null)
                {
                    errorf("'{0}'不是变量", listSymbolName);
                }
                else
                {
                    if(listSymbol.DimType!=ListExp.RetType)
                    {
                        errorf("'{0}'类型不一致", listSymbolName);
                    }
                }
            }

            Type[] genericTypes = GenericUtil.GetInstanceGenriceType(listType, typeof(列表<>));
            if (genericTypes.Length == 0)
            {
                genericTypes = GenericUtil.GetInstanceGenriceType(listType, typeof(IList<>));
            }

            Type ElementType = genericTypes[0];
            symbol = symbols.Get(elementName);
            if (symbol == null)
            {
                elementSymbol = new SymbolVar(elementName, ElementType);
                elementSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(elementName);
                elementSymbol.IsInBlock = true;
                symbols.Add(elementSymbol);
            }
            else
            {
                elementSymbol = symbol as SymbolVar;
                if (elementSymbol == null)
                {
                    errorf(ElementToken.Postion, "'{0}'不是变量", elementName);
                }
                else
                {
                    if (elementSymbol.DimType != ElementType)
                    {
                        errorf(ElementToken.Postion, "'{0}'类型不一致", listSymbolName);
                    }
                }
            }

            symbol = symbols.Get(indexName);
            if (symbol == null)
            {
                indexSymbol = new SymbolVar(indexName, typeof(int));
                indexSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(indexName);
                indexSymbol.IsInBlock = true;
                symbols.Add(indexSymbol);
            }
            else
            {
                indexSymbol = symbol as SymbolVar;
                if (elementSymbol == null)
                {
                    errorf("'{0}'不是变量", indexName);
                }
                else
                {
                    if (elementSymbol.DimType != typeof(int))
                    {
                        errorf("'{0}'不是整数类型", indexName);
                    }
                }
            }
           
            //int foreachIndex = context.MethodContext.CreateLocalVarIndex("$$$foreach_count");
            var count_symbol_name="$$$foreach_" + foreachIndex + "_count";
            countSymbol = new SymbolVar(count_symbol_name, typeof(int));
            countSymbol.LoacalVarIndex = context.MethodContext.CreateLocalVarIndex(count_symbol_name);
            countSymbol.IsInBlock = true;
            symbols.Add(countSymbol);
        }

        bool canForeach(Type type)
        {
            PropertyInfo countProperty = type.GetProperty("Count");
            if (countProperty == null) return false;
            var GetCountMethod = countProperty.GetGetMethod();
            if (GetCountMethod == null) return false;
            PropertyInfo itemProperty = type.GetProperty("Item");
            if (itemProperty == null) return false;
            var DiMethod = itemProperty.GetGetMethod();
            if (DiMethod == null) return false;
            return true;
        }

        public override void Generate(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            var True_Label = il.DefineLabel();
            var False_Label = il.DefineLabel();

            generateList(context);
            generateCount(context);
            genInitIndex(context);

            generateCondition(context);
            il.Emit(OpCodes.Brfalse, False_Label);

            //定义一个标签，表示从下面开始进入循环体
            il.MarkLabel(True_Label);
            generateElement(context);
            Body.Generate(context);
            EmitHelper.Inc(il, indexSymbol.VarBuilder);
            generateCondition(context);
            il.Emit(OpCodes.Brtrue, True_Label);
            il.MarkLabel(False_Label);
        }

        void generateList(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitExpContext expContext = new EmitExpContext(context);
            ListExp.Generate(expContext);
            EmitHelper.StormVar(il, listSymbol.VarBuilder);
        }

        void generateCount(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, getCountMethod);
            EmitHelper.StormVar(il, countSymbol.VarBuilder);
        }

        void genInitIndex(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadInt(il, startIndex);
            EmitHelper.StormVar(il, indexSymbol.VarBuilder);
        }

        void generateCondition(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);   
            EmitHelper.LoadVar(il, countSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, compareMethod,true);
            EmitHelper.LoadInt(il, 1);
            il.Emit(OpCodes.Ceq);
        }

        void generateElement(EmitStmtContext context)
        {
            ILGenerator il = context.ILout;
            EmitHelper.LoadVar(il, listSymbol.VarBuilder);
            EmitHelper.LoadVar(il, indexSymbol.VarBuilder);
            EmitHelper.CallDynamic(il, diMethod);
            EmitHelper.StormVar(il, elementSymbol.VarBuilder);
        }

        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(getStmtPrefix());
            buf.AppendFormat("循环每一个( {0},{1} , {2} )", this.ListExp.ToCode(), ElementToken.ToCode(),IndexToken!=null?(","+IndexToken.GetText()):"");
            buf.AppendLine();
            buf.Append(Body.ToString());
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return ForeachToken.Postion;
            }
        }
    }
}
