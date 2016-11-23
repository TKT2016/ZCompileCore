using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Descs;
using ZCompileCore.Loads;
using ZLangRT.Utils;
using System.Reflection.Emit;
using ZCompileCore.Lex;

namespace ZCompileCore.AST.Parts
{
    public class AgreementAST : PartAST
    {
        public Token KeyToken { get; set; }
        public List<Token> ValueList { get; set; }

        public AgreementAST()
        {
            ValueList = new List<Token>();
        }

        public void Analy()
        {
            Dictionary<string, Token> dict = new Dictionary<string, Token>();
            foreach (Token item in this.ValueList)
            {
                string name = item.GetText();
                if(dict.ContainsKey(name))
                {
                    errorf(item.Postion, "'{0}'已经定义", name);
                }
                else
                {
                    dict.Add(name, item);
                }
            }
        }

        public void Generate(EnumBuilder enumBuilder)
        {
            int i = 1;
            foreach (Token item in this.ValueList)
            {
                string name = item.GetText();
                var builder = enumBuilder.DefineLiteral(name, i);
                i++;
            }
        }
        #region 位置格式化
        public override string ToCode()
        {
            Deep = 1;
            StringBuilder buf = new StringBuilder();
            buf.AppendLine("约定:");
            foreach(var token in ValueList)
            {
                buf.Append("	");
                buf.AppendLine(token.GetText());
            }
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get { return KeyToken.Postion; }
        }
        #endregion
    }
}
