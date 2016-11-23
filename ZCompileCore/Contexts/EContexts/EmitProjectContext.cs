using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitProjectContext
    {
        public AssemblyName AssemblyName { get; set; }
        public AppDomain CurrentAppDomain { get; set; }
        public AssemblyBuilder AssemblyBuilder { get; set; }
        public ModuleBuilder ModuleBuilder { get; set; }
    }
}
