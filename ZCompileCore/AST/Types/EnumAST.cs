using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.Lex;
using ZCompileCore.Analys;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZLangRT;
using ZLangRT.Attributes;

namespace ZCompileCore.AST.Types
{
    public class EnumAST : FileElementAST
    {
        Token nameToken;
        AgreementAST ast;
        FileAST fileAST;
        ProjectContext ProjectContext { get; set; }

        public EnumAST(FileAST fileAST)
        {
            this.fileAST = fileAST;
            if(fileAST.AgreementList.Count==0)
            {
                errorf("不存在约定值");
            }
            else if (fileAST.AgreementList.Count >1 )
            {
                errorf("一个文件只能定义一个约定");
            }
            else
            {
                ast = fileAST.AgreementList[0];
                nameToken = fileAST.NameToken;
                ProjectContext = fileAST.ProjectContext;
            }
        }

        public void Analy()
        {
            if (ast != null) ast.Analy();
            ast.Analy();
        }
        
        public Type Generate()
        {
            string enumName = nameToken.GetText();
            string enumFullName = ProjectContext.RootNameSpace + "." + enumName;// FileContext.ProjectContext.RootNameSpace + "." + EnumName;
            EnumBuilder enumBuilder = ProjectContext.EmitContext.ModuleBuilder.DefineEnum(enumFullName, TypeAttributes.Public, typeof(int));
            setAttrTktClass(enumBuilder);
            ast.Generate(enumBuilder);
            /*
            int i = 1;
            foreach (Token item in this.EnumItems)
            {
                string name = item.GetText();
                var builder = enumBuilder.DefineLiteral(name,i);
                //this.EnumSymbol.EnumValueDict[name].Builder = builder;
                i++;
            }*/
            Type type= enumBuilder.CreateType();
            return type;
        }

        protected void setAttrTktClass(EnumBuilder classBuilder)
        {
            Type myType = typeof(ZClassAttribute);
            ConstructorInfo infoConstructor = myType.GetConstructor(new Type[] { });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(infoConstructor, new object[] { });
            classBuilder.SetCustomAttribute(attributeBuilder);
        }

        public override string ToCode()
        {
            return this.fileAST.ToCode();
        }

        public override CodePostion Postion
        {
            get { return nameToken.Postion; }
        }
    }


}
