using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analyers;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZLangRT;
using ZLangRT.Attributes;

namespace ZCompileCore.AST.Types
{
    public class StaticClassAST : ClassAST
    {
        public StaticClassAST(FileAST fileAST):base(fileAST)
        {

        }
        
        bool IsStatic = true;

        public Type Compile()
        {
            bool yes = true;
            AnalyImport();
            CompileClassName();
            
            TypeBuilder classBuilder = ClassContext.ClassSymbol.ClassBuilder;
            AnalyPropertyName(IsStatic);
            AnalyMethodName(IsStatic);
            
            yes = yes && !Messager.HasErrorOrWarning();
            yes = yes && compilePropertyBody(IsStatic);
            yes = yes && compileMethodBody(IsStatic);
            if (this.ClassContext.ZeroConstructor == null && ClassContext.InitMemberValueMethod != null)
            {
                ClassContext.ZeroConstructor = ClassContext.EmitContext.CurrentTypeBuilder.DefineConstructor(MethodAttributes.Static, CallingConventions.Standard, new Type[] { });
                var il = ClassContext.ZeroConstructor.GetILGenerator();
                EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod,true);//EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod);
                il.Emit(OpCodes.Ret);
            }
            if (yes)
            {
                Type type = classBuilder.CreateType();
                return type;
            }
            else
            {
                return null;
            }
        }

        void CompileClassName()
        {
            checkName();
            var symbols = this.ClassContext.Symbols;
            ClassContext.ClassSymbol = new SymbolDefClass(ClassName,true);
            SuperGcl = null;
            string fullName = this.ClassContext.ProjectContext.RootNameSpace + "." + ClassName;
            TypeBuilder classBuilder = createTypeBuilder(fullName);
            
            ClassContext.ClassSymbol.ClassBuilder = classBuilder;
            ClassContext.EmitContext.CurrentTypeBuilder = classBuilder;
            ClassContext.EmitContext.IDoc = this.ClassContext.ProjectContext.EmitContext.ModuleBuilder.DefineDocument(this.fileAST.FileName, Guid.Empty, Guid.Empty, Guid.Empty);
            setAttrTktClass(classBuilder);
            symbols.AddSafe(ClassContext.ClassSymbol);
        }

        TypeBuilder createTypeBuilder(string fullName)
        {
            TypeAttributes typeAttrs =  TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed;
            TypeBuilder classBuilder  = this.ClassContext.ProjectContext.EmitContext.ModuleBuilder.DefineType(fullName,typeAttrs);
            return classBuilder;
        }
    }


}
