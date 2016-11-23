using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Reports
{
    public class ProjectCompileResult
    {
        public string BinaryFilePath { get; set; }
        public List<CompileMessage> Errors { get; private set; }
        public List<CompileMessage> Warnings { get; private set; }
        public List<Type> CompiledTypes { get; set; }

        public ProjectCompileResult()
        {
            Errors = new List<CompileMessage>();
            Warnings = new List<CompileMessage>();
            CompiledTypes = new List<Type>();
        }

        public bool HasError()
        {
            return this.Errors.Count>0;
        }
    }
}
