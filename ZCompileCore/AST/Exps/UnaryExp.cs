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
using ZCompileCore.Symbols;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using ZLangRT;

namespace ZCompileCore.AST.Exps
{
    public class UnaryExp:Exp
    {
        public Token OpToken { get; set; }
        public Exp RightExp { get; set; }

        TokenKind OpKind;
        
        public UnaryExp(Token token ,Exp rightExp)
        {
            OpToken = token;
            RightExp = rightExp;
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;
            OpKind = OpToken.Kind;

           if(OpKind!= TokenKind.ADD && OpKind!= TokenKind.SUB)
           {
               errorf(OpToken.Postion, "运算符'{0}'缺少表达式", OpToken.GetText());
               return null;
           }

           RightExp = RightExp.Analy(context);//RightExp = AnalyExp(RightExp);
           if(RightExp==null)
           {
               TrueAnalyed = false;
               return null;
           }
     
            Type rtype = RightExp.RetType;
            RetType = rtype;
            if(rtype!= typeof(int)&& rtype!= typeof(float) && rtype!= typeof(double) && rtype != typeof(decimal))
            {
                errorf(RightExp.Postion,"不能进行'{0}'运算",OpToken.GetText());
                return null;
            }

            if (OpKind == TokenKind.ADD)
            {
                return RightExp;
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;

            EmitHelper.LoadInt(il,0);
            if(RetType== typeof(float))
            {
                 il.Emit(OpCodes.Conv_R4);
            }
            RightExp.Generate(context);
            il.Emit(OpCodes.Sub);
            base.GenerateConv(context);
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(OpToken != null ? OpToken.ToCode() : "");
            buf.Append(RightExp != null ? RightExp.ToString() : "");
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return OpToken.Postion; ;
            }
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            RightExp.GetNestedFields(nestedField);
        }
        #endregion

    }
}
