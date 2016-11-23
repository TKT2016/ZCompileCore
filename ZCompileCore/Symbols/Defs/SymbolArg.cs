using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;

namespace ZCompileCore.Symbols.Defs
{
    public class SymbolArg : InstanceSymbol
    {
        public Type ArgType { get; set; }
        public int ArgIndex { get; set; }
        public bool IsGeneric { get; set; }

        public SymbolArg(string name, Type argType,int argIndex,bool isGeneric)
        {
            this.SymbolName = name;
            ArgType = argType;
            ArgIndex = argIndex;
            IsGeneric = isGeneric;
        }

        public override Type DimType
        {
            get
            {
                return ArgType;
            }
            set
            {
                throw new CompileException("参数的类型不能赋值");
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return "参数(" + ArgType.Name + ":" + SymbolName + ")";
        }
    }
}
