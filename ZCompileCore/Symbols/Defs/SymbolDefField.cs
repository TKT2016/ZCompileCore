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
    public class SymbolDefField : InstanceSymbol
    {
        public Type FieldType { get; private set; }
        //public bool IsAssigned { get; set; }
        //public FieldBuilder Builder { get; set; }
        public bool IsStatic { get; protected set; }

        public SymbolDefField(string name, Type propertyType, bool isStatic)
        {
            this.SymbolName = name;
            FieldType = propertyType;
            IsAssigned = true;
            IsStatic = isStatic;
        }

        FieldInfo Field;
        public void SetField(FieldInfo field)
        {
            Field = field;
        }

        public FieldInfo GetField()
        {
            return Field;
        }

        public override Type DimType
        {
            get{
                return FieldType;
            }
            set{
                FieldType = value;
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
