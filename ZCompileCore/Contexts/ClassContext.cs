using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analyers;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Loads;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Tools;
using ZLangRT;

namespace ZCompileCore.Analys.AContexts
{
    public class ClassContext
    {
        public ProjectContext ProjectContext { get; private set; }
        public EmitClassContext EmitContext { get; set; }

        public SymbolDefClass ClassSymbol { get; set; }
        public SymbolTable Symbols { get; set; }

        public FileInfo SourceFile { get; set; }
        public string Name { get; private set; }

        public ImportContext ImportContext { get; set; }

        public ConstructorBuilder ZeroConstructor { get; set; }
        public List<ConstructorBuilder> ConstructorBuilderList { get; set; }
        public MethodBuilder InitMemberValueMethod { get; set; }

        /// <summary>
        /// 内部类所在的ClassContext(内部类专用)
        /// </summary>
        public ClassContext NestedOutClass { get; set; }
        public SymbolDefField OutClassField { get; set; }
        
        private ClassContext()
        {
            ConstructorBuilderList = new List<ConstructorBuilder>();
            ImportContext = new ImportContext();
        }

        public ClassContext(ProjectContext contextProject,string name)
        {
            ProjectContext = contextProject;
            EmitContext = new EmitClassContext(ProjectContext.EmitContext);
            Name = name;
            Symbols = new SymbolTable(name);
            ConstructorBuilderList = new List<ConstructorBuilder>();
            ImportContext = new ImportContext();
        }

        public ClassContext CreateNested(string nestedName)
        {
            ClassContext classContext = new ClassContext ();
            classContext.ProjectContext = this.ProjectContext;

            classContext.Name = classContext.Name;
            classContext.SourceFile = this.SourceFile;
            classContext.Symbols = this.Symbols.Push(nestedName);
            
            classContext.EmitContext = new EmitClassContext(ProjectContext.EmitContext);
            classContext.typeNameAnalyer = this.typeNameAnalyer;
            classContext.ImportContext = this.ImportContext;
            classContext.ClassSymbol = new SymbolDefClass(nestedName,false);
            classContext.EmitContext.CurrentTypeBuilder = this.EmitContext.CurrentTypeBuilder.DefineNestedType(nestedName);
            classContext.NestedOutClass = this;
            return classContext;
        }

        TypeNameAnalyer typeNameAnalyer;

        public IGcl SearchType(string name)
        {
            if (typeNameAnalyer==null)
            {
                typeNameAnalyer = new TypeNameAnalyer(this.ImportContext);
            }
            IGcl gcl= typeNameAnalyer.Analy(name);
            if(gcl!=null && gcl.ForType.IsGenericType)
            {
                if(this.ImportContext.GenericCreatedTypes.IndexOf(gcl)==-1)
                {
                    this.ImportContext.GenericCreatedTypes.Add(gcl);
                }
            }
            return gcl;
        }

        public string GetSourceFileName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(SourceFile.FullName);
        }

        public string GetSourceFolder()
        {
            return SourceFile.Directory.FullName;
        }

        int nestedClassIndex = 0;
        public string CreateNestedClassName()
        {
            nestedClassIndex++;
            return EmitContext.CurrentTypeBuilder.Name + "$" + nestedClassIndex;
        }

        public List<SymbolEnumItem> SearchEnumItem(string name)
        {
            return this.ImportContext.SearchEnumItem(name);
        }
    }
}
