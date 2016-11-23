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
using ZCompileCore.Loads;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    public class DeExp : Exp , IGenerateSet
    {
        public Exp SubjectExp { get; set; }
        public Token MemberToken { get; set; }

        string propertyName;
        ExPropertyInfo ExProperty;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            if( MemberToken==null)
            {
                error("'的'字前面没有对象");
                return null;
            }
            if ( MemberToken == null)
            {
                error("'的'字后面没有属性名称");
                return null;
            }
            var symbols = this.AnalyExpContext.Symbols;
            SubjectExp.IsAssignedBy = this.IsAssignedBy;
            SubjectExp.IsAssignedValue = this.IsAssignedValue;
            SubjectExp = SubjectExp.Analy(context);
            propertyName = MemberToken.GetText();
            var subjType = SubjectExp.RetType;
            if (subjType == null) return null;
            if (propertyName == "每一个" && subjType.IsGenericType && subjType.GetGenericTypeDefinition() == typeof(Z语言系统.列表<>))
            {
                EveryOneExp eoexp = new EveryOneExp();
                eoexp.ListExp = this.SubjectExp;
                eoexp.MemberToken = this.MemberToken;
                Exp eoExp = eoexp.Analy(context);
                return eoExp;
            }
            else
            {
                if (!(subjType is TypeBuilder))
                {
                    IGcl obj = context.StmtContext.MethodContext.ClassContext.ImportContext.SearchGCL(subjType);
                    if (obj == null)
                    {
                        //ExProperty = subjType.GetExProperty(propertyName);
                        ExProperty = GclUtil.SearchExProperty(propertyName, subjType);
                    }
                    else
                    {
                        ExProperty = obj.SearchExProperty(propertyName);
                    }
                }
                else
                {
                    string name = subjType.Name;
                    SymbolInfo symbol = symbols.Get(name);
                    if (symbol is SymbolDefClass)
                    {
                        ExProperty = (symbol as SymbolDefClass).GetExProperty(propertyName);
                        //ExProperty = GclUtil.SearchExProperty(propertyName, symbol as SymbolDefClass);
                    }
                }
            }
            if (ExProperty == null)
            {
                error(SubjectExp.Postion, "不存在" + propertyName + "属性");
                return null;
            }
            else
            {
                RetType = ExProperty.Property.PropertyType;
            }
            return this;
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            SubjectExp.GetNestedFields(nestedField);          
        }

        public override void Generate(EmitExpContext context)
        {
            GenerateGet(context);
            base.GenerateConv(context);
        }

        void GenenrateSubject(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            bool isgen = false;
            if ((SubjectExp is VarExp))
            {
                VarExp varexp = SubjectExp as VarExp;
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
                SubjectExp.Generate(context);
            }
        }

        public void GenerateGet(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            MethodInfo getMethod = ExProperty.Property.GetGetMethod();
            GenenrateSubject(context);
            EmitHelper.CallDynamic(il, getMethod,ExProperty.IsSelf); 
        }

        public void GenerateSet(EmitExpContext context, Exp valueExpr)
        {
            ILGenerator il = context.ILout;
            MethodInfo setMethod = ExProperty.Property.GetSetMethod();
            GenenrateSubject(context);
            valueExpr.Generate(context);

            EmitHelper.CallDynamic(il, setMethod, ExProperty.IsSelf); 
        }

        public bool CanWrite
        {
            get
            {
                return ExProperty.Property.CanWrite;
            }
        }

        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(SubjectExp != null ? SubjectExp.ToCode() : "");
            buf.Append("的");
            buf.Append(MemberToken != null ? MemberToken.GetText() : "");
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return SubjectExp.Postion; ;
            }
        }
    }
}
