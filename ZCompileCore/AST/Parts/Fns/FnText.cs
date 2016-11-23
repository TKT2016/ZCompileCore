using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Analys;

namespace ZCompileCore.AST.Parts.Fns
{
    public class FnText : FileElementAST
    {
        public Token TextToken { get; set; }
        public string TextContent { get { return TextToken.GetText(); } }

        public void Analy(FnName fnName)
        {
            //this.symbols = parentTable;
            foreach(var ch in TextContent)
            {
                if(ch>='0'&& ch<='9')
                {
                    error("过程名称中不能出现数字");
                    break;
                }
            }
        }
        #region 位置格式化
        public override string ToCode()
        {
            return TextToken.GetText();
        }

        public override CodePostion Postion
        {
            get { return TextToken.Postion; }
        }
        #endregion
    }
}
