using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Parts.Fns
{
    public class FnName : FileElementAST
    {
        public MethodAST MethodAST { get; set; }

        public List<FileElementAST> NameTerms { get; set; }
        public Token RetToken { get; set; }
        public TKTProcDesc ProcDesc { get;private set; }

        public FnName()
        {
            NameTerms = new List<FileElementAST>();
        }

        public bool Analy(MethodAST methodAST, bool isStatic)
        {
            this.MethodAST = methodAST;
            int argIndexStart =  isStatic ? 0 : 1;
            bool b= AnlayNameBody(argIndexStart);
            if (!b) return false;
            b = AnalyRet();
            if (!b) return false;
            return true;
        }

        /// <summary>
        /// 是否是构造函数(-1:否,0:无任何成员,11:括号无参,12:名称无参,20:名称+参数)
        /// </summary>
        public int IsConstructor(string className)
        {
            int count = NameTerms.Count;
            if(count==0)
            {
                return 0;
            }
            else if(count==1)
            {
                if (NameTerms[0] is FnBracket) return 11;
                if (!(NameTerms[0] is FnText)) return -1;
                if ((NameTerms[0] as FnText).TextContent == className) return 12;
                return -1;
            }
            else if(count==2)
            {
                if (!(NameTerms[0] is FnText)) return -1;
                if (!(NameTerms[1] is FnBracket)) return -1;
                if ((NameTerms[0] as FnText).TextContent != className) return -1;
                return 20;
            }
            else
            {
                return -1;
            }
            //if (NameTerms.Count == 0) return true;
            //if (NameTerms.Count >1) return false;
            //if (!(NameTerms[0] is FnBracket)) return false;
            //return true;
        }

        public bool IsMinConstructor()
        {
            if (NameTerms.Count == 0) return true;
            return false;
        }
        
        bool AnlayNameBody(int startArgIndex)
        {
            var symbols = MethodAST.MethodContext.Symbols;
            ProcDesc = new TKTProcDesc();
            int argIndex = startArgIndex;
            for (int i = 0; i < NameTerms.Count; i++)
            {
                var term = NameTerms[i];
                if (term is FnText)
                {
                    var textterm = term as FnText;
                    textterm.Analy(this);
                    ProcDesc.Add(textterm.TextContent);
                }
                else if (term is FnBracket)
                {
                    var argterm = term as FnBracket;
                    List<TKTProcArg> procArgs = new List<TKTProcArg>();
                    foreach (var arg in argterm.Args)
                    {
                        bool b = arg.Analy(this, argIndex);
                        if (b)
                        {
                            TKTProcArg procArg = new TKTProcArg(arg.ArgName, arg.ArgType, arg.IsGenericArg);
                            procArgs.Add(procArg);
                            argIndex++;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    ProcDesc.Add(procArgs);
                }
            }
            if(! checkName()) return false;
            MethodAST.MethodContext.ProcDesc = ProcDesc;
            MethodAST.MethodContext.ClassContext.ClassSymbol.AddMethod(MethodAST.MethodContext);
            return true;
        }

        bool checkName( )
        {
            bool argRight = true;
            bool hasText = false;
            //bool firstIsIdent = (NameTerms[start] is FnText);
            if (NameTerms.Count == 0) return true;
            if (NameTerms.Count == 1) return true;
            for (int i = 0; i < NameTerms.Count; i++)
            {
                var term = NameTerms[i];
                if (term is FnText)
                {
                    hasText = true;
                }
                else if (term is FnBracket)
                {
                    var fb = term as FnBracket;
                    
                    /*if (i != NameTerms.Count - 1)
                    {
                        if(fb.Args.Count!=1)
                        {
                            error("非末尾的参数数量必须是1");
                            argRight = false;
                        }
                    }
                    else
                    {
                        if (NameTerms[i - 1] is FnBracket)
                        {
                            if(fb.Args.Count==0)
                            {
                                error("末尾的参数数量不能没有参数");
                                argRight = false;
                            }
                        }
                    }*/
                }
            }
            return argRight && hasText /*&& firstIsIdent*/;
        }

        public bool AnalyRet( )
        {
            var methodContext = this.MethodAST.MethodContext;
            var symbols = MethodAST.MethodContext.Symbols;
            if (RetToken == null)
            {
                methodContext.RetType = typeof(void);
            }
            else
            {
                string retName = RetToken.GetText();
                SymbolInfo symbol = symbols.Get(retName);
                if(symbol is SymbolArg)
                {
                    SymbolArg argSymbol = symbol as SymbolArg;
                    if(argSymbol.IsGeneric)
                    {
                        IsGenericRet = true;
                        GenericRetIndex = argSymbol.ArgIndex;
                        methodContext.RetType = typeof(object);
                        methodContext.RetIsGeneric = true;
                    }
                    else
                    {
                        error("非‘类型’的参数不能用作返回类型");
                        return false;
                    }
                }
                else if (symbol is SymbolDefClass)
                {
                    methodContext.RetType = (symbol as SymbolDefClass).GetRealType();
                }
                else
                {
                    IGcl gcl = this.MethodAST.ClassContext.SearchType(retName);
                    if (gcl != null)
                    {
                        methodContext.RetType = gcl.ForType;
                    }
                    else
                    {
                        errorf("返回结果'{0}'不是类型", retName);
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsGenericRet { get; set; }
        public int GenericRetIndex { get; set; }

        #region 位置格式化
        public override string ToCode()
        {
            List<string> buflist = new List<string>();
            foreach (var term in NameTerms)
            {
                buflist.Add(term.ToCode());
            }
            if(RetToken!=null)
            {
                buflist.Add("=>");
                buflist.Add(RetToken.ToCode());
            }
            string fnname = string.Join("", buflist);
            return fnname;
        }

        public string ToProcDescCode()
        {
            StringBuilder buflist = new StringBuilder();
            foreach (var term in NameTerms)
            {
                if(term is FnText)
                {
                    buflist.Append((term as FnText).TextContent);
                }
                else if (term is FnBracket)
                {
                    var fnBracket = (term as FnBracket);
                    if (fnBracket.Args.Count > 0)
                    {
                        buflist.Append("(");
                        foreach (var arg in fnBracket.Args)
                        {
                            buflist.Append(string.Format("{0}:{1}", arg.ArgType.Name, arg.ArgName));
                        }
                        buflist.Append(")");
                    }
                }
            }
            string fnname = buflist.ToString();
            return fnname;
        }

        public override CodePostion Postion
        {
            get { return NameTerms[0].Postion; }
        }
        #endregion
        public string CreateMethodName()
        {
            TKTProcDesc ProcDesc = MethodAST.MethodContext.ProcDesc;
            List<string> list = new List<string>();
            for (int i = 0; i < ProcDesc.Parts.Count; i++)
            {
                if (ProcDesc.Parts[i] is string)
                {
                    list.Add(ProcDesc.Parts[i] as string); ;
                }
                else if (ProcDesc.Parts[i] is FnBracket)
                {
                    var fnBracket = (ProcDesc.Parts[i] as FnBracket);
                    if (fnBracket.Args.Count > 0)
                    {
                        foreach (var arg in fnBracket.Args)
                        {
                            list.Add(arg.ArgTypeToken.GetText());
                        }
                    }
                }
            }
            string str = string.Join("", list);
            //if(str!="启动")
            //{
            //    //str = str + "_" + this.MethodAST.MethodContext.MethodIndex;
            //}
            return str;
        }  
    }
}
