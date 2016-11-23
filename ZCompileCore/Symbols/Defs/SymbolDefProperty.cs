using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using ZCompileCore.AST;
using ZCompileCore.AST.Parts;
using ZCompileCore.Tools;
using ZLangRT;

namespace ZCompileCore.Symbols.Defs
{
    public class SymbolDefProperty : InstanceSymbol
    {
        public Type PropertyType { get; private set; }
        //public bool IsAssigned { get; set; }
        //public PropertyBuilder Builder { get; set; }
        public bool IsStatic { get; protected set; }

        PropertyInfo Property;
        public void SetProperty(PropertyInfo property)
        {
            Property = property;
        }

        public PropertyInfo GetProperty()
        {
            return Property;
        }

        public SymbolDefProperty(string name, Type propertyType,bool isStatic)
        {
            this.SymbolName = name;
            PropertyType = propertyType;
            IsAssigned = true;
            IsStatic = isStatic;
        }

        public override Type DimType
        {
            get
            {
                return PropertyType;
            }
            set
            {
                PropertyType = value;
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
