using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitStmtContext
    {
        public EmitMethodContext MethodEmitContext { get;private set; }
      
        public EmitStmtContext(EmitMethodContext methodContext)
        {
            MethodEmitContext = methodContext;
        }

        public EmitConstructorContext ConstructorEmitContext { get; private set; }

        public EmitStmtContext(EmitConstructorContext constructorContext)
        {
            ConstructorEmitContext = constructorContext;
        }

        public ILGenerator ILout
        {
            get
            {
                if (this.MethodEmitContext != null)
                    return this.MethodEmitContext.ILout;
                else
                    return this.ConstructorEmitContext.ILout;
            }
        }

    }
}
