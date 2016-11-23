using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitConstructorContext
    {
        public EmitClassContext ClassContext { get; set; }
        public ConstructorBuilder CurrentBuilder { get; set; }
        public ILGenerator ILout { get { return this.CurrentBuilder.GetILGenerator(); } }
    }
}
