using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Attributes
{
    public class ZMappingAttribute : Attribute
    {
        public ZMappingAttribute(Type type)
        {
            ForType = type;
        }

        public ZMappingAttribute(Type type, Type baseMappingType)
        {
            ForType = type;
            BaseMappingType = baseMappingType;
        }

        public Type ForType { get; private set; }

        public Type BaseMappingType { get; private set; }
    }
}
