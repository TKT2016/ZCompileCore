using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Analyers;
using ZCompileCore.Analys;
using ZCompileCore.Loads;

namespace ZCompileCore.AST.Parts.Fns
{
    public class FnArg : FileElementAST
    {
        public Token ArgTypeToken { get; set; }
        public Token ArgNameToken { get; set; }

        public string ArgName { get; set; }
        public Type ArgType { get; set; }
        public bool IsGenericArg { get; set; }

        public bool Analy(FnName fn,int argIndex)
        {
            var symbols = fn.MethodAST.MethodContext.Symbols;

            if (ArgTypeToken != null && ArgNameToken != null)
            {
                ArgName = ArgNameToken.GetText();
                if (ArgTypeToken.IsKeyIdent("类型"))
                {
                    ArgType =typeof(object);
                    IsGenericArg = true;
                }
                else
                {
                    IsGenericArg = false;
                    string argTypeName = ArgTypeToken.GetText();
                    SymbolInfo symbol = symbols.Get(argTypeName);
                    if(symbol!=null)
                    {
                        SymbolDefClass defClass = symbol as SymbolDefClass;
                        if(defClass!=null)
                        {
                            ArgType = defClass.ClassBuilder;
                        }
                    }

                    if (ArgType == null)
                    {
                        //var gcls = fn.MethodAST.MethodContext.ClassContext.ImportContext.SearchGCL(argTypeName);
                        //if (gcls.Length == 1)
                        //{
                        //    ArgType = gcls[0].ForType;
                        //}
                        var gcl = fn.MethodAST.MethodContext.ClassContext.SearchType(argTypeName); //
                        if(gcl!=null)
                        {
                            ArgType = gcl.ForType;
                        }
                        if (ArgType == null)
                        {
                            error(string.Format("不存在或者不确定'{0}'类型", argTypeName));
                            return false;
                        }
                    }
                }
                if (ArgType != null)
                {
                    if (symbols.Contains(ArgName))
                    {
                        errorf("'{0}'已经存在相同的名称", ArgName);
                        return false;
                    }
                    else
                    {
                        SymbolArg argSymbol = new SymbolArg(ArgName, ArgType, argIndex, IsGenericArg);
                        symbols.AddSafe(argSymbol);
                    }
                }
            }
            else
            {
                error("参数不正确");
                return false;
            }
            return true;
        }

        #region 位置格式化
        public override string ToCode()
        {
            return string.Format("{0}:{1}", ArgTypeToken.GetText(), ArgNameToken.GetText());
        }

        public override CodePostion Postion
        {
            get { return ArgTypeToken.Postion; }
        }
        #endregion
    }
}
