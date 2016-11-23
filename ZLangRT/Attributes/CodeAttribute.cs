using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Attributes
{
    public class CodeAttribute : Attribute
    {
        public CodeAttribute(string procCode)
        {
            Code = procCode;
        }

        public string Code { get; private set; }
    }
}
