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
using ZCompileCore.Reports;
using ZLangRT;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    public class BinExp:Exp
    {
        public Token OpToken { get; set; }
        public Exp LeftExp { get; set; }
        public Exp RightExp { get; set; }
        private MethodInfo OpMethod { get; set; }
        TokenKind OpKind;

        public BinExp()
        {
            
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;
            OpKind = OpToken.Kind;

            if (RightExp==null)
            {
                errorf(OpToken.Postion, "运算符'{0}'右边缺少运算元素", OpToken.GetText());
                return null;
            }
            else if (LeftExp == null && RightExp != null)
            {
                UnaryExp unexp = new UnaryExp(OpToken, RightExp);
                var exp = unexp.Analy(context);
                return exp;
            }

            LeftExp = LeftExp.Analy(context);//LeftExp = AnalyExp(LeftExp);
            RightExp = RightExp.Analy(context);//RightExp = AnalyExp(RightExp);

            if (LeftExp==null || RightExp==null)
            {
                TrueAnalyed = false;
                return null;
            }

            Type ltype = LeftExp.RetType;
            Type rtype = RightExp.RetType;

            OpMethod = BinExpUtil.GetCalcMethod(OpKind, ltype, rtype);
            if (OpMethod != null)
            {
                RetType = OpMethod.ReturnType;
            }
 
            if(RetType==null)
            {
                error("两种类型无法进行'"+OpToken.ToCode() +"'运算");
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            base.GenerateArgsExp(context, OpMethod.GetParameters(), new Exp[] { LeftExp, RightExp });
            EmitHelper.CallDynamic(il, OpMethod, true);
            base.GenerateConv(context);
        }
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(LeftExp != null ? LeftExp.ToString() : "");
            buf.Append(OpToken != null ? OpToken.ToCode() : "");
            buf.Append(RightExp != null ? RightExp.ToString() : "");
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return LeftExp.Postion; ;
            }
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            LeftExp.GetNestedFields(nestedField);
            RightExp.GetNestedFields(nestedField);
        }
    }
}
