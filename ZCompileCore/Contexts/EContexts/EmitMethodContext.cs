using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Analys.EContexts
{
    public class EmitMethodContext
    {
        public EmitClassContext ClassContext { get; set; }
        //public EmitProjectContext ProjectContext { get { return this.ClassContext.ProjectContext; } }
        //public MethodBuilder CurrentMethodBuilder { get; set; }
        public MethodBuilder CurrentMethodBuilder { get;private set; }
        public ConstructorBuilder CurrentConstructorBuilder { get; private set; }

        public void SetBuilder(MethodBuilder methodBuilder)
        {
            CurrentMethodBuilder = methodBuilder;
        }

        public void SetBuilder(ConstructorBuilder constructorBuilder)
        {
            CurrentConstructorBuilder = constructorBuilder;
        }
        
        //public EmitMethodContext ParentContext { get; set; }
        //private ILGenerator _ILout;
        public ILGenerator ILout
        {
            get {
                if (CurrentMethodBuilder!=null)
                    return this.CurrentMethodBuilder.GetILGenerator(); 
                else
                    return this.CurrentConstructorBuilder.GetILGenerator(); 
            }
        }
        //{
        //    get
        //    {
        //        if (_ILout == null)
        //        {
        //            _ILout = CurrentMethodBuilder.GetILGenerator();
        //        }
        //        return _ILout;
        //    }
        //}
        //public TypeBuilder CurrentTypeBuilder { get; set; }

        //public TypeBuilder NestedTypeBuilder { get; set; }
        /*
        public EmitMethodContext PushNew()
        {
            EmitMethodContext emitContext = new EmitMethodContext();
            emitContext.ProjectContext = this.ProjectContext;
            //emitContext.ILout = this.ILout;
            emitContext.ParentContext = this;
            //emitContext.CurrentTypeBuilder = this.CurrentTypeBuilder;
            //emitContext.NestedTypeBuilder = this.NestedTypeBuilder;
            //emitContext.CurrentMethodBuilder = this.CurrentMethodBuilder;
            return emitContext;
        }*/
    }
}
