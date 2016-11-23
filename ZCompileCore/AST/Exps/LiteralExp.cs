using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Symbols;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using Z语言系统;
using ZLangRT;
using ZCompileCore.Reports;

namespace ZCompileCore.AST.Exps
{
    public class LiteralExp:Exp
    {
        Type intType = typeof(int);
        Type floatType = typeof(float);
        Type stringType = typeof(string);
        Type boolType = typeof(bool);

        public Token LiteralToken { get; set; }

        public override string ToCode()
        {
            return LiteralToken.ToCode();
        }

        public override CodePostion Postion
        {
            get
            {
                return LiteralToken.Postion; ;
            }
        }

        public string IdentName
        {
            get
            {
                return LiteralToken.ToCode();
            }
        }

        TokenKind LiteralKind;
        string LiteralValue;
      
        public LiteralExp( )
        {
           
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            
        }
        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            LiteralKind = LiteralToken.Kind;
            LiteralValue = LiteralToken.GetText();

            if (LiteralKind == TokenKind.LiteralInt)
            {
                RetType = intType;
            }
            else if (LiteralKind == TokenKind.LiteralFloat)
            {
                RetType = floatType;
            }
            else if (LiteralKind == TokenKind.LiteralString)
            {
                RetType = stringType;
            }
            else if (LiteralKind == TokenKind.True || LiteralKind == TokenKind.False)
            {
                RetType = boolType;
            }
            else if (LiteralKind == TokenKind.NULL)
            {
                RetType = null;
            }
            else
            {
                error(LiteralToken.ToCode() + "不是正确的值");
                return null;
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
			
            if (LiteralKind == TokenKind.LiteralString)
            {
                il.Emit(OpCodes.Ldstr, LiteralValue);         
            }
            else if (LiteralKind == TokenKind.NULL)
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (LiteralKind == TokenKind.LiteralInt)
            {
                GenerateInt(context);
            }
            else if (LiteralKind == TokenKind.LiteralFloat)
            {
                GenerateFloat(context);
            }
            else if (LiteralKind == TokenKind.True || LiteralKind == TokenKind.False)
            {
                GenerateBool(context);
            }
            base.GenerateConv(context);
        }

        private void GenerateInt(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            int value = int.Parse(LiteralValue);
            EmitHelper.LoadInt(il, value);
        }

        private void GenerateFloat(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            var value = float.Parse(LiteralValue);
            il.Emit(OpCodes.Ldc_R4, value);
        }

        private void GenerateBool(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            if (LiteralKind == TokenKind.True)
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }
            else if (LiteralKind == TokenKind.False)
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
        }
    }
}
