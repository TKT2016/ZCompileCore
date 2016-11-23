using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;


namespace ZCompileCore.Symbols.Imports
{
    public class SymbolEnumItem :  InstanceSymbol
    {
        //public SymbolEnumMap MapEnum;
        public object EnumValue { get; set; }

        public SymbolEnumItem(/*SymbolEnumMap mapEnum,*/string name,object enumValue)
        {
            this.SymbolName = name;
            //MapEnum = mapEnum;
            EnumValue = enumValue;
            IsAssigned = true;
        }

        public override Type DimType
        {
            get
            {
                return EnumValue.GetType();
            }
            set
            {
                throw new CompileException("枚举的类型不能赋值");
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
    }
}
