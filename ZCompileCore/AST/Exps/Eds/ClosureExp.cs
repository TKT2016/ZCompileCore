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
using ZCompileCore.AST.Types;
using ZLangRT;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    public class ClosureExp : AnalyedCallExp
    {
        public ClosureExp(Exp srcExp)
            : base(srcExp)
        {

        }

        /// <summary>
        /// 内部的外部类
        /// </summary>
        public SymbolDefField OutClassField
        {
            get { return this.NestedClassContext.OutClassField; }
        }

        public Dictionary<string, SymbolDefField> Fields { get;private set; }
        public Dictionary<string, VarExp> FieldExps { get; set; }

        public Exp BodyExp { get; set; }
        public Type FnRetType { get; set; }

        public ClassContext NestedClassContext { get; set; }

        public MethodContext NestedMethodContext { get; set; }
        AnalyStmtContext nestedStmtContext { get; set; }
        AnalyExpContext nestedExpContext { get; set; }
        SymbolVar retSymbol;
        Type NestedType { get; set; }
        public ConstructorBuilder NewBuilder { get; private set; }
        //private List<SymbolDefField> nestedSymbolInfos;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            createContext(this.AnalyExpContext);
            var symbols = this.AnalyExpContext.Symbols;
            analyOutClassField(this.AnalyExpContext, false, context.ClassContext.ClassSymbol.ClassBuilder);
            analyFields(false);
            if (RetType != typeof(void))
            {
                retSymbol = new SymbolVar("结果", RetType);
                retSymbol.IsAssigned = true;
                retSymbol.LoacalVarIndex = this.NestedMethodContext.CreateLocalVarIndex("结果");
                symbols.AddSafe(retSymbol);
            }
            BodyExp = BodyExp.Analy(this.AnalyExpContext);
            if (FnRetType != typeof(Action))
            {
                if (RetType!=typeof(bool))
                {
                    error(BodyExp.Postion,"返回结果的类型不匹配");
                    return null;
                }
            }
            return this;
        }

        void analyFields(bool isStatic)
        {
            Fields = new Dictionary<string, SymbolDefField>();
            var symbols = NestedClassContext.Symbols;
            var names = FieldExps.Keys.ToList();
            for (int i = 0; i < names.Count; i++)
            {
                var name = names[i];
                VarExp exp = FieldExps[name];
                SymbolVar symbol = exp.VarSymbol as SymbolVar;

                //if (symbol is InstanceSymbol)
                //{
                    var propertyType = symbol.DimType;
                    FieldBuilder field = NestedClassContext.EmitContext.CurrentTypeBuilder.DefineField("_" + name, propertyType, FieldAttributes.Public);
                    SymbolDefField fieldSymbol = new SymbolDefField(name, propertyType, isStatic);
                    fieldSymbol.SetField( field);
                    symbols.AddSafe(fieldSymbol);

                    //Fields.Remove(name);
                    Fields.Add(name, fieldSymbol);
                exp.IsNestedField = true;
                SymbolDefField fs = new SymbolDefField(name, field.FieldType, field.IsStatic);
                fs.SetField(field);
                exp.VarSymbol = fs;
                //nestedSymbolInfos.Add(fieldSymbol);
                //}
            }
        }
        /*
        void analyFields(bool isStatic)
        {
            var symbols = NestedClassContext.Symbols;
            var names = Fields.Keys.ToList();
            for (int i = 0; i < names.Count; i++)
            {
                var name = names[i];
                VarExp varExp = Fields[name];
                SymbolVar symbol = varExp.VarSymbol as SymbolVar;
                if (symbol is InstanceSymbol)
                {
                    var propertyType = (symbol as InstanceSymbol).DimType;
                    FieldBuilder field = NestedClassContext.EmitContext.CurrentTypeBuilder.DefineField("_" + name, propertyType, FieldAttributes.Private | FieldAttributes.Static);
                    var fieldSymbol = new SymbolDefField(name, propertyType, isStatic);
                    fieldSymbol.SetField(field);
                    symbols.AddSafe(fieldSymbol);

                    Fields.Remove(name);
                    Fields.Add(name, fieldSymbol);
                    //nestedSymbolInfos.Add(fieldSymbol);
                }
            }
        }*/

        private FieldBuilder outClassFieldBuilder;
        //private SymbolDefField outClassFieldSymbol;

        void analyOutClassField(AnalyExpContext context, bool isStatic,Type outClassType)
        {
            var name = "__$__Nested_This";
            var symbols = NestedClassContext.Symbols;
            //var curContext = context.StmtContext.MethodContext.ClassContext;
            var propertyType = outClassType;
            outClassFieldBuilder = NestedClassContext.EmitContext.CurrentTypeBuilder.DefineField( name, propertyType, FieldAttributes.Public);
            NestedClassContext.OutClassField = new SymbolDefField(name, propertyType, isStatic);
            NestedClassContext.OutClassField.SetField(outClassFieldBuilder);
            symbols.AddSafe(NestedClassContext.OutClassField);
        }

        void createContext(AnalyExpContext context)
        {
            string className = context.StmtContext.MethodContext.ClassContext.CreateNestedClassName();
            NestedClassContext = context.StmtContext.MethodContext.ClassContext.CreateNested(className);
            NestedClassContext.ClassSymbol = new SymbolDefClass(context.StmtContext.MethodContext.ClassContext.ClassSymbol,className);
            NestedMethodContext = new MethodContext(NestedClassContext, "NestedMethod");
            NestedMethodContext.EmitContext.SetBuilder( NestedClassContext.EmitContext.CurrentTypeBuilder.DefineMethod("$$CALL", MethodAttributes.Public, RetType, new Type[] { }));
            nestedStmtContext = new AnalyStmtContext(NestedMethodContext, "NestedStmt");
            nestedExpContext = new AnalyExpContext(nestedStmtContext);
            this.AnalyExpContext = nestedExpContext;
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            BodyExp.GetNestedFields(nestedField);
        }

        public override void Generate(EmitExpContext context)
        {
            var il = NestedMethodContext.EmitContext.CurrentMethodBuilder.GetILGenerator();
            EmitStmtContext stmtContext = new EmitStmtContext(NestedMethodContext.EmitContext);
            EmitExpContext expContext = new EmitExpContext(stmtContext);
            if(retSymbol!=null)
            {
                retSymbol.VarBuilder = il.DeclareLocal(retSymbol.DimType);
            }
            //GenerateBodyExpHead(context);
            
            BodyExp.Generate(expContext);
            if (retSymbol == null)
            {
                if (BodyExp.RetType != typeof(void))
                {
                    il.Emit(OpCodes.Pop);
                }
            }
            else
            {
                EmitHelper.StormVar(il, retSymbol.VarBuilder);
            }

            if (RetType != typeof(void))
            {
                il.Emit(OpCodes.Ldloc, retSymbol.VarBuilder);
            }
            il.Emit(OpCodes.Ret);
            generateConstructor(context);
            NestedType = NestedClassContext.EmitContext.CurrentTypeBuilder.CreateType();
            base.GenerateConv(context);
        }

        void generateConstructor(EmitExpContext context)
        {
             var symbols = this.AnalyExpContext.Symbols;
            List<Type> types = new List<Type>();
            //types.Add(outClassFieldSymbol.FieldType);
            /*foreach (SymbolDefField symbol in Fields.Values.ToList())
            {
                types.Add(symbol.FieldType);
            }*/
            NewBuilder = NestedClassContext.EmitContext.CurrentTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, types.ToArray());
            var il = NewBuilder.GetILGenerator();
            il.Emit(OpCodes.Ret);
        }
        
    }
}
