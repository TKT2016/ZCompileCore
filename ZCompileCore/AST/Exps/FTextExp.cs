using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using ZLangRT;
using System;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps.Eds;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Parse;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Tools;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    public class FTextExp:Exp
    {
        public Token IdentToken { get; set; }
        string IdentName;
        ClassContext classContext;
        SymbolTable symbols ;

        public FTextExp( )
        {

        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            identParser = new IdentParser();
            symbols = this.AnalyExpContext.Symbols;

            IdentName = IdentToken.GetText();
            classContext = context.StmtContext.MethodContext.ClassContext;

            Exp exp = null;

            exp = analyCurrentClass(context);

            if (exp == null)  exp = analyParentClass(context);

            if(exp==null)  exp = analyAsDirectMember(context);

            if (exp == null) exp = analyCall(context).Item1;

            if (exp == null)
            {
                if (IdentName.IndexOf('的') != -1 || IdentName.IndexOf('第') != -1)
                {
                    exp = analyDiDe(context);
                }
                else
                {
                    exp = analyType(context);
                }
            }
            if (exp == null)
                exp = dimVar(context);

            if (exp != null)
            {
                exp = exp.Analy(context);//
                return exp;
            }
            return null;
        }

        Exp analyType(AnalyExpContext context)
        {
            IGcl gcl = context.StmtContext.MethodContext.ClassContext.SearchType(IdentName);
            if(gcl!=null)
            {
                Type type = gcl.ForType;
                TypeExp exp = new TypeExp(this, IdentToken, type);
                return exp;
            }
            return null;
        }

        Tuple<Exp,int> analyCall(AnalyExpContext context)
        {
            TKTProcDesc procdesc = new TKTProcDesc();
            procdesc.Add(IdentName);
            var procArray = ClassContextHelper.SearchProc(classContext, procdesc);
            if(procArray.Length==1)
            {
                InvokeSimplestExp exp = new InvokeSimplestExp(this, IdentToken, procArray[0]);
                return new Tuple<Exp,int> (exp,1);
            }
            else if(procArray.Length==0)
            {
                return new Tuple<Exp, int>(null, 0);
            }
            else
            {
                errorf("有多个相同名称'{0}'过程，不能确定是哪个过程",IdentName);
                return new Tuple<Exp, int>(null, procArray.Length);
            }
        }

        Exp analyCurrentClass(AnalyExpContext context)
        {
            var IdentSymbol = symbols.Get(IdentName);
            if (IdentSymbol == null)
            {
                return null;
            }
            if (IdentSymbol is InstanceSymbol)
            {
                VarExp exp = new VarExp(this, IdentToken, this.IsAssignedBy, this.IsAssignedValue);
                return exp;
            }
            else if (IdentSymbol is SymbolDefClass)
            {
                TypeExp exp = new TypeExp(this, IdentToken, (IdentSymbol as SymbolDefClass));
                return exp;
            }
            else
            {
                throw new CompileException("类型非IVarSymbol、SymbolDefClass");
            }
        }

        Exp analyParentClass(AnalyExpContext context)
        {
            ClassContext classContext = context.ClassContext;
            SymbolDefClass classSymbol = classContext.ClassSymbol;
            if (classSymbol.BaseGcl == null) return null;
            ExPropertyInfo property = classSymbol.BaseGcl.SearchExProperty(IdentName);
            //if (property == null) return null;
            if (property != null)
            {
                if (ReflectionUtil.IsPublic(property.Property) || ReflectionUtil.IsProtected(property.Property))
                {
                    SymbolDefProperty ps = new SymbolDefProperty(IdentName, property.Property.PropertyType, ReflectionUtil.IsStatic(property.Property));
                    ps.SetProperty(property.Property);

                    VarExp exp = new VarExp(this, this.IdentToken);
                    exp.VarSymbol = ps;
                    return exp;
                }
            }
            return null;
        }

        IdentParser identParser;
        Exp analyDiDe(AnalyExpContext context)
        {
            Exp exp = identParser.Parse(IdentToken,this);
            if (exp == null) return null;
            exp.IsAssignedBy = this.IsAssignedBy;
            exp.IsAssignedValue = this.IsAssignedValue;
            return exp;
        }

        Exp dimVar(AnalyExpContext context)
        {
            VarExp varexp = new VarExp(this, IdentToken, this.IsAssignedBy, this.IsAssignedValue);
            return varexp;
        }

        Exp analyAsDirectMember(AnalyExpContext context)
        {
            object[] objs = ClassContextHelper.SearchDirectIdent(classContext, IdentName);
            if (objs.Length == 0)
            {
                //error("属性或者规定的值'" + IdentName + "'不存在");
                return null;
            }
            else if (objs.Length > 1)
            {
                error("属性或者规定的值'" + IdentName + "'不明确");
                return null;
            }
            else
            {
                var obj = objs[0];
                SymbolInfo directSymbol;
                if (obj is ExPropertyInfo)
                {
                    var pvar = obj as ExPropertyInfo;
                    directSymbol = new SymbolPropertyDirect(IdentName, pvar);
                    RetType = pvar.Property.PropertyType;
                }
                else if (obj is ExFieldInfo)
                {
                    var pvar = obj as ExFieldInfo;
                    directSymbol = new SymbolFieldDirect(IdentName, pvar);
                    RetType = pvar.Field.FieldType;
                }
                else
                {
                    var enumSymbol = new SymbolEnumItem(IdentName, obj);
                    directSymbol = enumSymbol;
                    RetType = enumSymbol.DimType;
                }
                if(directSymbol!=null)
                {
                    DirectMemberExp exp = new DirectMemberExp(this, IdentToken, directSymbol, this.IsAssignedBy, this.IsAssignedValue);
                    return exp;
                }
            }
            return null;
        }


        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {

        }

        public override void Generate(EmitExpContext context)
        {
            throw new CompileException("FTextExp不能Generate");
        }

        #region 覆盖
        public override string ToCode()
        {
            return IdentToken.GetText();
        }

        public override CodePostion Postion
        {
            get
            {
                return IdentToken.Postion;
            }
        }
        #endregion

    }
}
