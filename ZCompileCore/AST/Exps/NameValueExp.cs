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
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;

namespace ZCompileCore.AST.Exps
{
    public class NameValueExp: Exp
    {
        public Token NameToken;
        public Exp ValueExp;

        public NameValueExp()
        {
            //Elements = new List<Exp>();
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            ValueExp = ValueExp.Analy(context);
            if (ValueExp == null) return null;
            RetType = ValueExp.RetType;
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            ValueExp.Generate(context); 
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            ValueExp.GetNestedFields(nestedField);
        }

        public override string ToCode()
        {
            return string.Format("{0}:{1}", NameToken.GetText(), ValueExp.ToCode());
        }

        public override CodePostion Postion
        {
            get
            {
                return NameToken.Postion;
            }
        }
    }

}
