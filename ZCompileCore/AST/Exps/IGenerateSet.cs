using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.EContexts;

namespace ZCompileCore.AST.Exps
{
    public interface IGenerateSet
    {
        void GenerateSet(EmitExpContext context, Exp valueExp);
        bool CanWrite { get; }
    }
}
