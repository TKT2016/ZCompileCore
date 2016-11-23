using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Tools;

namespace ZCompileCore.AST.Exps.Eds
{
    public class DirectMemberExp : AnalyedExp, IGenerateSet
    {
        Token MemberToken { get; set; }
        SymbolInfo memberSymbol { get; set; }

        public DirectMemberExp(Exp srcExp, Token varToken, SymbolInfo mebmerSymbol, bool isAssignedBy, bool isAssignedValue)
            : base(srcExp)
        {
            MemberToken = varToken;
            memberSymbol = mebmerSymbol;
            this.IsAssignedBy = IsAssignedBy;
            this.IsAssignedValue = IsAssignedValue;
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;
            RetType = (memberSymbol as InstanceSymbol).DimType;
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            if (!EmitHelper.EmitSymbolGet(il, (SymbolInfo)memberSymbol))
            {
                throw new CompileException("DirectMemberExp生成失败");
            }
            base.GenerateConv(context);
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            //nestedField.Add(memberSymbol.SymbolName, memberSymbol);
        }

        public void GenerateSet(EmitExpContext context, Exp valueExp)
        {
            ILGenerator il = context.ILout;
            var symbol = (memberSymbol as SymbolInfo);
            EmitHelper.EmitSet_Load(il, symbol);
            valueExp.Generate(context);
            var b= EmitHelper.EmitSet_Storm(il, symbol);
            if (!b)
            {
                throw new CompileException("DirectMemberExp不能GenerateSet");
            }
        }

        public bool CanWrite
        {
            get
            {
                if (memberSymbol is SymbolPropertyDirect)
                {
                    return (memberSymbol as SymbolPropertyDirect).CanWrite;
                }
                else if (memberSymbol is SymbolFieldDirect)
                {
                    return (memberSymbol as SymbolFieldDirect).CanWrite;
                }
                else
                {
                    return false;
                }
            }
        }

        #region 覆盖
        public override string ToCode()
        {
            return MemberToken.GetText();
        }

        public override CodePostion Postion
        {
            get
            {
                return MemberToken.Postion;
            }
        }
        #endregion
    }
}
