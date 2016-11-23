using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZLangRT
{
    public class RTException : Exception
    {
        public RTException()
        {
        }

        public RTException(string msg)
            : base(msg)
        {
        }

        public RTException(string format, params object[] args)
            : base(String.Format(format, args))
        {
        }
    }
}
