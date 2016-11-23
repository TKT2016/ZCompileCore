using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ZLangRT.Utils
{
    public static class DebugUtil
    {
        public static string StackFramesToString(StackFrame[] stacks)
        {
            string result = string.Empty;
            foreach (StackFrame stack in stacks)
            {
                result += string.Format("{0} {1} {2} {3}\r\n", stack.GetFileName(),
                    stack.GetFileLineNumber(),
                    stack.GetFileColumnNumber(),
                    stack.GetMethod().ToString());
            }
            return result;
        }

    }
}
