using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps.Eds;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Analys;
using ZCompileCore.Tools;
using ZLangRT;
using ZCompileCore.Loads;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    public class EveryOneExp : Exp , IGenerateSet
    {
        public Exp ListExp { get; set; }
        public Token MemberToken { get; set; }

        //string propertyName;
        //PropertyInfo ExProperty;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            //SubjectExp.LoadRefTypes(context);
            Type[] types =  GenericUtil.GetInstanceGenriceType(ListExp.RetType, typeof(Z语言系统.列表<>));
            this.RetType = types[0];
            return this;
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            ListExp.GetNestedFields(nestedField);          
        }

        public override void Generate(EmitExpContext context)
        {
            GenerateGet(context);
            //throw new CompileException("EveryOneExp不应该Generate");
        }
        
        void GenenrateSubject(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            bool isgen = false;
            if ((ListExp is VarExp))
            {
                VarExp varexp = ListExp as VarExp;
                if (ReflectionUtil.IsStruct(varexp.RetType))
                {
                    if (varexp.VarSymbol is SymbolVar)
                    {
                        il.Emit(OpCodes.Ldloca, (varexp.VarSymbol as SymbolVar).VarBuilder);
                        isgen = true;
                    }
                    else if (varexp.VarSymbol is SymbolArg)
                    {
                        il.Emit(OpCodes.Ldarga, (varexp.VarSymbol as SymbolArg).ArgIndex);
                        isgen = true;
                    }
                }
            }
            if(!isgen)
            {
                ListExp.Generate(context);
            }
        }

        public void GenerateGet(EmitExpContext context)
        {
            GenenrateSubject(context);
            base.GenerateConv(context);
        }

        public void GenerateSet(EmitExpContext context,Exp valueExp)
        {
            throw new CompileException("EveryOneExp不应该GenerateSet");
        }
        
        public bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(ListExp != null ? ListExp.ToCode() : "");
            buf.Append("的");
            buf.Append(MemberToken != null ? MemberToken.GetText() : "");
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return ListExp.Postion; ;
            }
        }
    }
}
