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
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Analys;
using ZLangRT;

namespace ZCompileCore.AST.Exps
{
    public class FCallExp : Exp
    {
        public List<Exp> Elements { get; set; }

        public FCallExp()
        {
            Elements = new List<Exp>();
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;
            Exp exp = null;
            exp = analyNewExp(context);

            if (exp == null)
                exp = analyDiExp();

            if (exp == null)
            {
                InvokeExp invokeexp = new InvokeExp(this);
                invokeexp.Elements = this.Elements;
                exp = invokeexp;
            }

            if(exp!=null)
            {
                exp = exp.Analy(context);//exp = AnalyExp(exp);
                return exp;
            }

            throw new CompileException("FCallExp无法分析完成");
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            throw new CompileException("应该不存在这一步");
        }

        DiExp analyDiExp()
        {
            int ecount = Elements.Count;
            if (ecount != 3) return null;
            Exp first = Elements[0];
            if (!(Elements[0] is BracketExp)) return null;
            if (!(Elements[1] is FTextExp)) return null;
            if (!(Elements[2] is BracketExp)) return null;
            FTextExp keyIdent = Elements[1] as FTextExp;
            Token keyToken = keyIdent.IdentToken;
            if (keyToken.GetText() != "第") return null;
            DiExp exp = new DiExp(this);
            exp.SubjectExp = (Elements[0] as BracketExp);
            exp.ArgExp = (Elements[2] as BracketExp);
            return exp;
        }

        bool isNewFormat()
        {
            int ecount = Elements.Count;
            if (ecount != 2) return false;
            Exp first = Elements[0];
            if (!(first is FTextExp)) return false;
            if (!(Elements[1] is BracketExp)) return false;
            return true;
        }

        NewExp analyNewExp(AnalyExpContext context)
        {
            var symbols = this.AnalyExpContext.Symbols;
            if (!isNewFormat()) return null;
            Exp first = Elements[0];
            string name = (first as FTextExp).IdentToken.GetText();
            IGcl gcl = context.StmtContext.MethodContext.ClassContext.SearchType(name);
            if (gcl == null) return null;
            NewExp newExp = new NewExp(this);
            newExp.SubjectGCL = gcl;
            newExp.IsAssignedValue = this.IsAssignedValue;
            newExp.BrackestArgs = (Elements[1] as BracketExp);
            return newExp;
        }

        public override void Generate(EmitExpContext context)
        {
            throw new CompileException("CallExp分析错误");
        }

        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            List<string> tempcodes = new List<string>();
            foreach (var expr in Elements)
            {
                if (expr != null)
                {
                    tempcodes.Add(expr.ToCode());
                }
                else
                {
                    tempcodes.Add(" ");
                }
            }
            buf.Append(string.Join("", tempcodes));
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return Elements[0].Postion; ;
            }
        }
    }

}
