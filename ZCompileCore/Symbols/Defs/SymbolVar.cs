using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace ZCompileCore.Symbols.Defs
{
    public class SymbolVar : InstanceSymbol
    {
        //public bool IsAssigned { get; set; }
        public int LoacalVarIndex { get; set; }
        public LocalBuilder VarBuilder { get; set; }
        //public Type DimType { get; set; }

        public SymbolVar(string name)
        {
            this.SymbolName = name;
        }

        public SymbolVar(string name, Type type)
        {
            this.SymbolName = name;
            DimType = type;
        }
        /*
        public override Type GetDimType()
        {
            return DimType;
        }*/

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return "变量(" + DimType.Name + ":" + SymbolName + ")";
        }

        public bool IsInBlock { get; set; }

        
    }
}
