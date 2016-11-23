using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;

namespace ZCompileCore.AST.Exps.Eds
{
    public class TypeExp : AnalyedExp
    {
        Token TypeToken { get; set; }
        Type EType { get; set; }
        SymbolDefClass DType { get; set; }
        int type = 0;

        public TypeExp(Exp srcExp,Token typeToken, Type eType):base(srcExp)
        {
            TypeToken = typeToken;
            EType = eType;
            type = 1;
        }

        public TypeExp(Exp srcExp, Token typeToken, SymbolDefClass dType)
            : base(srcExp)
        {
            TypeToken = typeToken;
            DType = dType;
            type = 2;
        }

        public override Exp Analy(AnalyExpContext context)
        {
            if (type == 1)
            {
                RetType = EType;
            }
            else if(type==2)
            {
                RetType = DType.ClassBuilder;
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {

        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {

        }

        #region 覆盖

        public override string ToCode()
        {
            return TypeToken.GetText();
        }

        public override CodePostion Postion
        {
            get
            {
                return TypeToken.Postion;
            }
        }
        #endregion
    }
}
