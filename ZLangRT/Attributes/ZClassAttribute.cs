using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Attributes
{
    public class ZClassAttribute : Attribute
    {
        public ZClassAttribute()
        {
            
        }

        public ZClassAttribute(Type baseMappingType)
        {
            ParentMappingType = baseMappingType;
        }

        public Type ParentMappingType { get; private set; }
    }
}
