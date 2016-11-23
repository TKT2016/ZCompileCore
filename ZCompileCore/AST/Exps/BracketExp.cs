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
using ZCompileCore.Symbols;
using ZCompileCore.Analys;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Exps
{
    public class BracketExp:Exp
    {
        public Token LeftBracketToken { get; set; }
        public Token RightBracketToken { get; set; }

        public List<Exp> InneExps { get; set; }

        public BracketExp()
        {
            InneExps = new List<Exp>();
        }

        public int InnerCount
        {
            get{
                //if (InneExps == null) return 0;
                return InneExps.Count;
            }   
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            this.TrueAnalyed = true;
            for (int i=0;i<InneExps.Count;i++)
            {
                Exp exp = InneExps[i];
                exp = exp.Analy(context);
                if(exp==null)
                {
                    TrueAnalyed = false;
                }
                else
                {
                    InneExps[i] = exp;
                    TrueAnalyed = TrueAnalyed && exp.TrueAnalyed;
                }
            }
            if (InneExps.Count == 1)
            {
                RetType = InneExps[0].RetType; 
            }
            else
            {
                RetType = null;
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            //ILGenerator il = context.ILout;
            foreach (var expr in this.InneExps)
            {
                expr.Generate(context);
            }
            base.GenerateConv(context);
        }

        public List<Type> GetInnerTypes()
        {
            List<Type> list = new List<Type>();
            foreach (var expr in this.InneExps)
            {
                list.Add(expr.RetType);
            }
            return list;
        }

        public List<TKTProcArg> GetDimArgs()
        {
            List<TKTProcArg> args = new List<TKTProcArg>();
            foreach (var exp in this.InneExps)
            {
                bool isGeneric = false;
                Type type = exp.RetType;
                string argName = null;
                TKTProcArg arg = null;
                if (exp is TypeExp)
                {
                    var idExp = exp as TypeExp;
                    isGeneric = idExp.RetType.IsGenericType;
                    arg = new TKTProcArg(type, isGeneric);
                }
                else if(exp is NameValueExp)
                {
                    argName = (exp as NameValueExp).NameToken.GetText();
                    arg = new TKTProcArg(argName,type, isGeneric);
                }
                else
                {
                    arg = new TKTProcArg( type, isGeneric);
                }
                arg.Value = exp;
                args.Add(arg);
            }
            return args;
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            foreach (var expr in this.InneExps)
            {
                expr.GetNestedFields(nestedField);
            }
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("(");
            if (InneExps != null && InneExps.Count > 0)
            {
                List<string> tempcodes = new List<string>();
                foreach (var expr in InneExps)
                {
                    if (expr != null)
                    {
                        tempcodes.Add(expr.ToCode());
                    }
                    else
                    {
                        tempcodes.Add("  ");
                    }
                }
                buf.Append(string.Join(",", tempcodes));
            }
            buf.Append(")");
            return buf.ToString();
            //return string.Format("({0})", InneExpr.ToString());
        }

        public override CodePostion Postion
        {
            get
            {
                return LeftBracketToken.Postion;
            }
        }
        #endregion
    }
}
