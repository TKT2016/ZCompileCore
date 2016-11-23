using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Analys;

namespace ZCompileCore.AST.Parts.Fns
{
    public class FnBracket : FileElementAST
    {
        public Token LeftBracketToken { get; set; }
        public Token RightBracketToken { get; set; }
        public int ArgIndex { get; set; }
        public List<FnArg> Args = new List<FnArg>();

        public int Count
        {
            get
            {
                return Args.Count;
            }
        }


        #region 位置格式化
        public override string ToCode()
        {
            List<string> buflist = new List<string>();
            foreach (var term in Args)
            {
                buflist.Add(term.ToCode());
            }
            string fnname = string.Join(",", buflist);
            return string.Format("({0})", fnname);
        }

        public override CodePostion Postion
        {
            get { return LeftBracketToken.Postion; }
        }      
        #endregion
    }
}
