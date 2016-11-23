using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;
using ZCompileCore.Symbols.Defs;
using ZLangRT.Descs;


namespace ZCompileCore.Symbols.Imports
{
    public class SymbolPropertyDirect:  InstanceSymbol
    {
        public ExPropertyInfo ExProperty { get; set; }

        public SymbolPropertyDirect(string name, ExPropertyInfo exProperty)
        {
            this.SymbolName = name;
            ExProperty = exProperty;
        }

        public override Type DimType
        {
            get
            {
                return ExProperty.Property.PropertyType;
            }
            set
            {
                throw new CompileException("外部的Field类型不能赋值");
            }
        }

        public override bool CanWrite
        {
            get
            {
                return ExProperty.Property.CanWrite;
            }
        }
    }
}
