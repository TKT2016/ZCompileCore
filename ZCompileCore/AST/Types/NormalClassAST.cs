using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Loads;
using ZCompileCore.Reports;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Analyers;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;
using ZLangRT;
using ZLangRT.Attributes;
using Z语言系统;

namespace ZCompileCore.AST.Types
{
    public class NormalClassAST : ClassAST
    {
        public NormalClassAST(FileAST fileAST)
            : base(fileAST)
        {

        }

        bool IsStatic = false;

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
            ClassContext.ClassSymbol = new SymbolDefClass(ClassName,false);
            if (this.fileAST.ExtendsToken != null)
            {
                string superName = this.fileAST.ExtendsToken.GetText();
                SuperGcl = this.ClassContext.SearchType(superName);
                if (SuperGcl == null)
                {
                    error(this.fileAST.ExtendsToken.Postion, "没有找到类型'" + superName + "'");
                }
                else
                {
                    Type type = SuperGcl.ForType;
                    if (type.IsSealed)
                    {
                        error(this.fileAST.ExtendsToken.Postion, "类型'" + superName + "'不能继承");
                    }
                }
            }
            else
            {
                SuperGcl = new MappingGcl(typeof(事物), null);
            }

            string fullName = this.ClassContext.ProjectContext.RootNameSpace + "." + ClassName;
            TypeBuilder classBuilder = createTypeBuilder(fullName);
            
            ClassContext.ClassSymbol.ClassBuilder = classBuilder;
            ClassContext.EmitContext.CurrentTypeBuilder = classBuilder;
            ClassContext.ClassSymbol.BaseGcl = SuperGcl;
            ClassContext.EmitContext.IDoc = this.ClassContext.ProjectContext.EmitContext.ModuleBuilder.DefineDocument(this.fileAST.FileName, Guid.Empty, Guid.Empty, Guid.Empty);
            setAttrTktClass(classBuilder);
            symbols.AddSafe(ClassContext.ClassSymbol);
        }

        TypeBuilder createTypeBuilder(string fullName)
        {
            TypeBuilder classBuilder = this.ClassContext.ProjectContext.EmitContext.ModuleBuilder.DefineType(fullName, TypeAttributes.Public);
            if (SuperGcl != null)
            {
                classBuilder.SetParent(SuperGcl.ForType);
                this.ClassContext.ClassSymbol.BaseGcl = SuperGcl;
            }

            return classBuilder;
        }
        
    }


}
