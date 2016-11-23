using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analyers;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.Loads;
using ZCompileCore.Reports;
using ZCompileCore.Analys;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;
using ZCompileCore.Symbols.Defs;
using ZLangRT;
using ZLangRT.Attributes;

namespace ZCompileCore.AST.Types
{
    public abstract class ClassAST : ASTree
    {
        public ClassContext ClassContext { get; set; }
        public List<PropertyAST> PropertyList { get; protected set; }
        public ImportPackageAST ImportPackage { get; set; }
        public SimpleUseAST SimpleUse { get; set; }
        public List<MethodAST> MethodList { get; protected set; }
        public string ClassName { get; protected set; }
        protected FileAST fileAST;
        public IGcl SuperGcl { get; set; }

        protected ImportAnalyer importAnalyer;
        protected PropertyAnalyer propertyAnalyer;

        public ClassAST(FileAST fileAST)     
        {
            this.fileAST = fileAST;
            ImportPackage = fileAST.ImportPackage;
            MethodList = fileAST.MethodList;
            PropertyList = fileAST.PropertyList;
            ClassContext = new ClassContext(fileAST.ProjectContext, ClassName);
            SimpleUse = fileAST.SimpleUse;
        }

        protected bool AnalyImport()
        {
            importAnalyer = new ImportAnalyer(this.ClassContext, this.ImportPackage, this.SimpleUse);
            importAnalyer.Analy();
            return true;
        }

        protected void checkName()
        {
            if (this.fileAST.NameToken != null)
            {
                if (this.fileAST.NameToken.GetText() != this.fileAST.FileName)
                {
                    error(this.fileAST.NameToken.Postion, "名称和文件名称不一致");
                }
                this.ClassName = this.fileAST.NameToken.GetText();
            }
            else
            {
                this.ClassName = this.fileAST.FileName;
            }
        }

        protected void setAttrTktClass(TypeBuilder classBuilder)
        {
            Type myType = typeof(ZClassAttribute);
            ConstructorInfo infoConstructor = myType.GetConstructor(new Type[] { });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(infoConstructor, new object[] { });
            classBuilder.SetCustomAttribute(attributeBuilder);
        }

        protected bool AnalyMethodName(bool isStatic)
        {
            int i = 0;
            foreach (var ast in MethodList)
            {
                i++;
                ast.CompileName(this,i,isStatic);
            }
            return true;
        }

        protected bool AnalyPropertyName(bool isStatic)
        {
            propertyAnalyer = new PropertyAnalyer(this.ClassContext, this.PropertyList);
            propertyAnalyer.CompileName(isStatic);
            return true;
        }

        protected bool compilePropertyBody(bool isStatic)
        {
            if (Messager.HasErrorOrWarning() == false)
            {
                propertyAnalyer.CompileBody(isStatic);
                return true;
            }
            return false;
        }

        protected bool compileMethodBody(bool isStatic)
        {
            TypeBuilder classBuilder = ClassContext.ClassSymbol.ClassBuilder;
            foreach (var item in MethodList)
            {
                item.AnalyBody();
                if (Messager.HasErrorOrWarning() == false)
                {
                    item.Generate(classBuilder, isStatic);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToCode()
        {
            return this.fileAST.ToCode();
        }
    }


}
