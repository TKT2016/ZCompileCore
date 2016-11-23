using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using ZLangRT;

namespace ZCompileCore.Symbols
{
    public abstract class InstanceSymbol:SymbolInfo
    {
       //public abstract  Type GetDimType(/*SymbolTable table*/);
       public abstract  bool CanWrite { get; }
       public virtual Type DimType { get; set; }
       public bool IsAssigned { get; set; }
    }
}
