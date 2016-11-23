using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;
using ZLangRT.Descs;


namespace ZCompileCore.Symbols.Imports
{
    public class SymbolFieldDirect:  InstanceSymbol
    {
        public ExFieldInfo ExField { get; set; }

        public SymbolFieldDirect(string name, ExFieldInfo exField)
        {
            this.SymbolName = name;
            ExField = exField;
        }

        public override Type DimType
        {
            get
            {
                return ExField.Field.FieldType;
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
                return true;
            }
        }
    }
}
