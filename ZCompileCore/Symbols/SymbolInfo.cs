using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.Lex;
using System.Reflection.Emit;

namespace ZCompileCore.Symbols
{
    public abstract class SymbolInfo
    {
        public String SymbolName
        {
            get;
            protected set;
        }
    }
    /*
    public abstract class SymbolDef : SymbolInfo
    {
        //public Type SymbolType { get; set; }
    }*/
    /*
    public abstract class SymbolImport : SymbolInfo
    {
        //public abstract Type SymbolType { get;  }
    }*/
    /*
    public abstract class SymbolCommon : SymbolInfo
    {
        //public Type SymbolType { get; set; }
    }*/
}
