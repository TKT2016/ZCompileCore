using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.Analys;
using ZLangRT;

namespace ZCompileCore.Analyers
{
    public class PropertyAnalyer
    {
        ClassContext ClassContext;
        List<PropertyAST> PropertyList;
        public int Count { get { return PropertyList.Count; } }

        public PropertyAnalyer(ClassContext classContext, List<PropertyAST> propertyList)
        {
            ClassContext = classContext;
            PropertyList = propertyList;
        }

        public void CompileName( bool isStatic )
        {
            foreach (var ast in PropertyList)
            {
                ast.CompileName(ClassContext, isStatic);
            }
        }


        public bool CompileBody(bool isStatic)
        {
            if (Count == 0) return true;
            TypeBuilder classBuilder = ClassContext.ClassSymbol.ClassBuilder;
            if (ClassContext.InitMemberValueMethod == null)
            {
                string initMemberValueMethodName = "__InitMemberValueMethod";
                if (isStatic)
                {
                    ClassContext.InitMemberValueMethod = classBuilder.DefineMethod(initMemberValueMethodName, MethodAttributes.Private | MethodAttributes.Static, typeof(void), new Type[] { });
                }
                else
                {
                    ClassContext.InitMemberValueMethod = classBuilder.DefineMethod(initMemberValueMethodName, MethodAttributes.Private, typeof(void), new Type[] { });
                }
            }
            EmitMethodContext context = new EmitMethodContext();
            context.ClassContext = ClassContext.EmitContext; ;
            context.SetBuilder( ClassContext.InitMemberValueMethod);
            EmitStmtContext stmtContext = new EmitStmtContext(context);
            EmitExpContext expContext = new EmitExpContext(stmtContext);

            var il = ClassContext.InitMemberValueMethod.GetILGenerator();
            foreach (var ppt in PropertyList)
            {
                ppt.GenerateBody(expContext);
            }
            il.Emit(OpCodes.Ret);
            return true;
        }

    }
}

