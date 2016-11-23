using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitClassContext
    {
        public EmitProjectContext ProjectContext { get; set; }
        //public TypeBuilder ParentTypeBuilder { get; set; }
        public TypeBuilder CurrentTypeBuilder { get; set; }
        public ISymbolDocumentWriter IDoc { get; set; }
        //public bool IsNested { get { return ParentTypeBuilder != null; } }

        public EmitClassContext(EmitProjectContext projectContext)
        {
            ProjectContext = projectContext;
        }
    }
}
