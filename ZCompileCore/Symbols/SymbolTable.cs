using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.Symbols;
using ZLangRT;

namespace ZCompileCore.Analys
{
    public class SymbolTable
    {
        SymbolTable ParentTable { get; set; }
        string tableName;

        public SymbolTable(string name)
        {
            tableName = name;
        }

        Dictionary<string, SymbolInfo> SymbolDict = new Dictionary<string, SymbolInfo>();

        public bool Contains(string symbolName)
        {
            if (this.SymbolDict.ContainsKey(symbolName))
            {
                return true;
            }
            else if (ParentTable != null)
            {
                return this.ParentTable.Contains(symbolName);
            }
            return false;
        }

        void add(SymbolInfo info)
        {
            //if(info.SymbolName=="次数")
            //{
            //    Console.WriteLine("--- CS");
            //}
            SymbolDict.Add(info.SymbolName, info);
        }

        public void Add(SymbolInfo info)
        {
            add(info);
        }

        public bool AddSafe(SymbolInfo info)
        {
            if (this.Contains(info.SymbolName) == true)
            {
                return false;
            }
            else
            {
                add(info);
                return true;
            }
        }

        public SymbolInfo Get(string symbolName)
        {
            if (this.SymbolDict.ContainsKey(symbolName) == true)
            {
                return SymbolDict[symbolName];
            }
            else if (this.ParentTable != null)
            {
                return this.ParentTable.Get(symbolName);
            }
            return null;
        }

        public T Get<T>(string symbolName) where T : SymbolInfo
        {
            SymbolInfo symbol = Get(symbolName);
            if (symbol == null) return null;
            return symbol as T;
        }
        
        public virtual SymbolTable Push(string name )
        {
            SymbolTable symbolTable = new SymbolTable(name);
            symbolTable.ParentTable = this;
            return symbolTable;
        }

        public SymbolTable Pop()
        {
            return  this.ParentTable;
        }

        public override string ToString()
        {
            return "SymbolTable:" + tableName + "[" + this.SymbolDict.Count + "]";
        }
        /*
        public Type SearchType(string name)
        {
            SymbolInfo symbol = Get(name);
            if (symbol == null) return null;
            Type type = SearchTypeRedirect(name);
            if (type != null) return type;

            if (symbol is SymbolArg)
            {
                //string rname = (symbol as SymbolArg).TypeName;
                //return SearchTypeRedirect(rname);
                return (symbol as SymbolArg).ArgType;
            }
            else if (symbol is SymbolVar)
            {
                return (symbol as SymbolVar).GetDimType(this);
                //string rname = (symbol as SymbolVar).DimSymbol.SymbolName;
                //return SearchTypeRedirect(rname);
            }
            else if (symbol is SymbolDim)
            {
                //string rname = (symbol as SymbolDim).DimTypeName;
                //return SearchTypeRedirect(rname);
                return (symbol as SymbolDim).DimType;
            }
            //return null;
        }*/
        /*
        public Type SearchTypeRedirect(string name)
        {
            SymbolInfo symbol = Get(name);
            if (symbol == null) return null;
            if (symbol is ITypeSymbol)
            {
                return (symbol as ITypeSymbol).GetRealType();
            }
            
            return null;
        }*/

        //public object Model { get; set; }
    }
}
