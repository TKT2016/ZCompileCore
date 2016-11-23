using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Tools;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps.Eds
{
    public class VarExp : AnalyedExp , IGenerateSet
    {
        Token VarToken { get; set; }

        public string VarName{ get;private set; }
        public SymbolInfo VarSymbol { get;set; }
        public bool IsNestedField { get; set; }

        public VarExp(Exp srcExp, Token varToken):base(srcExp)
        {
            VarToken = varToken;
        }

        public VarExp(Exp srcExp, Token varToken, bool isAssignedBy, bool isAssignedValue)
            : base(srcExp)
        {
            VarToken = varToken;
            this.IsAssignedBy = isAssignedBy;
            this.IsAssignedValue = isAssignedValue;
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;

            VarName = VarToken.GetText();
            if (!IsNestedField)
            {
                if (VarSymbol == null)
                {
                    VarSymbol = symbols.Get(VarName);
                }

                if (VarSymbol == null)
                {
                    //VarSymbol = symbols.Get(VarName);
                    List<SymbolEnumItem> enumValues = context.ClassContext.SearchEnumItem(VarName);
                    if (enumValues.Count == 1)
                    {
                        VarSymbol = enumValues[0];
                    }
                    if (enumValues.Count > 1)
                    {
                        errorf("'{0}'有多个相同约定值", VarName);
                        return null;
                    }
                }

                if (VarSymbol == null)
                {
                    if (context.ClassContext.ClassSymbol.BaseGcl != null)
                    {
                        ExPropertyInfo property = context.ClassContext.ClassSymbol.BaseGcl.SearchExProperty(VarName);
                        if (property != null)
                        {
                            if (ReflectionUtil.IsPublic(property.Property) ||
                                ReflectionUtil.IsProtected(property.Property))
                            {
                                SymbolDefProperty ps = new SymbolDefProperty(VarName, property.Property.PropertyType,
                                    ReflectionUtil.IsStatic(property.Property));
                                ps.SetProperty(property.Property);
                                VarSymbol = ps;
                            }
                        }
                    }
                }
                if (VarSymbol == null)
                {
                    if (context.ClassContext.ClassSymbol.BaseGcl != null)
                    {
                        ExFieldInfo field = context.ClassContext.ClassSymbol.BaseGcl.SearchExField(VarName);
                        if (field != null)
                        {
                            if (field.Field.IsPublic || field.Field.IsFamily)
                            {
                                SymbolDefField fs = new SymbolDefField(VarName, field.Field.FieldType,
                                    field.Field.IsStatic);
                                fs.SetField(field.Field);
                                VarSymbol = fs;
                            }
                        }
                    }
                }
                if (VarSymbol == null)
                {
                    if (IsAssignedBy)
                    {
                        SymbolVar varSymbol = new SymbolVar(VarName);
                        if (!varSymbol.IsInBlock)
                        {
                            varSymbol.LoacalVarIndex = context.StmtContext.MethodContext.CreateLocalVarIndex(VarName);
                            symbols.Add(varSymbol);
                        }
                        VarSymbol = varSymbol;
                    }
                    else
                    {
                        errorf("'{0}'没有赋值", VarName);
                        return null;
                    }
                }
                RetType = ((InstanceSymbol) VarSymbol).DimType;
               
            }
            else
            {
                
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            if (!EmitHelper.EmitSymbolGet(il, (SymbolInfo)VarSymbol))
            {
                throw new CompileException("VarExp生成失败");
            }
            base.GenerateConv(context);
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            if (VarSymbol != null)
            {
                var symbol = (VarSymbol as SymbolInfo);
                if (symbol is SymbolVar)
                {
                    nestedField.Add(VarSymbol.SymbolName, this);
                    //nestedField.Add(symbol.SymbolName, symbol);
                }
            }
        }

        public void GenerateSet(EmitExpContext context, Exp valueExp)
        {
            ILGenerator il = context.ILout;
            var symbol = (VarSymbol as SymbolInfo);
            EmitHelper.EmitSet_Load(il, symbol);
            valueExp.Generate(context);
            var b = EmitHelper.EmitSet_Storm(il, symbol);
            if (!b)
            {
                throw new CompileException("DirectMemberExp不能GenerateSet");
            }
        }

        public bool CanWrite
        {
            get
            {
                return true;
            }
        }

        #region 覆盖
        public override string ToCode()
        {
            return VarToken.GetText();
        }

        public override CodePostion Postion
        {
            get
            {
                return VarToken.Postion;
            }
        }
        #endregion
    }
}
