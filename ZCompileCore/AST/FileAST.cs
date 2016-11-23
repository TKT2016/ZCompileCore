using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.AST.Types;
using ZCompileCore.Lex;
using ZCompileCore.Reports;
using ZCompileCore.Analys;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Parts.Fns;
using ZLangRT;

namespace ZCompileCore.AST
{
    public class FileAST : ASTree
    {
        public ProjectContext ProjectContext { get; set; }
        public ImportPackageAST ImportPackage { get; set; }
        public SimpleUseAST SimpleUse { get; set; }
        public List<PropertyAST> PropertyList { get; set; }
        public List<MethodAST> MethodList { get; set; }
        public List<AgreementAST> AgreementList { get; set; }
        public Token ExtendsToken { get; set; }
        public Token NameToken { get; set; }

        public String FileName;

        public FileAST()
        {
            MethodList = new List<MethodAST>();
            PropertyList = new List<PropertyAST>();
            AgreementList = new List<AgreementAST>();
            ImportPackage = new ImportPackageAST() { KeyToken = new Token() { Col=0, Line=0, Kind= TokenKind.Ident, Text="使用包" } };
        }

        public void Init(string filePath, ProjectContext projectContext)
        {
            FileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            ProjectContext = projectContext;
        }   

        public void Add(PartAST part)
        {
            if (part == null) return;
            if(part is ImportPackageAST)
            {
                ImportPackage = (part as ImportPackageAST);
            }
            else if (part is PropertyAST)
            {
                PropertyList.Add(part as PropertyAST);
            }
            else if (part is MethodAST)
            {
                MethodList.Add(part as MethodAST);
            }
            else if (part is AgreementAST)
            {
                AgreementList.Add(part as AgreementAST);
            }
            else if (part is SimpleUseAST)
            {
                SimpleUse=(part as SimpleUseAST);
            }
            else
            {
                throw new CompileException("错误不能识别PartAST");
            }
        }
        /*
        public bool IsStatic()
        {
            return  (this.ExtendsToken != null && this.ExtendsToken.GetText() == "唯一类型");
        }*/

        public Type CompileFile()
        {
            if (this.ExtendsToken != null)
            {
                string extendsText = this.ExtendsToken.GetText();
                if (extendsText == "唯一类型")
                {
                    return compileAsStatic();
                    //StaticClassAST ast = new StaticClassAST(this);
                    //Type type = ast.Compile();
                    //return type;
                }
                else if (extendsText == "约定类型")
                {
                    return compileAsEnum();
                //    EnumAST ast = new EnumAST(this);
                //    Type type = ast.Generate();
                //    return type;
                }
                else
                {
                    return compileAsNormal();
                    //NormalClassAST ast = new NormalClassAST(this);
                    //Type type = ast.Compile();
                    //return type;
                }
            }
            else
            {
                return compileAsStatic();
                /*NormalClassAST ast = new NormalClassAST(this);
                Type type = ast.Compile();
                return type;*/
            }
        }

        Type compileAsStatic()
        {
            StaticClassAST ast = new StaticClassAST(this);
            Type type = ast.Compile();
            return type;
        }

        Type compileAsNormal()
        {
            NormalClassAST ast = new NormalClassAST(this);
            Type type = ast.Compile();
            return type;
        }

        Type compileAsEnum()
        {
            EnumAST ast = new EnumAST(this);
            Type type = ast.Generate();
            return type;
        }

        public string GetBinFilePath()
        {
            string srcFileName = FileName;
            string exeFileName = srcFileName + ".exe";
            return exeFileName;
        }

        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();
            buf.AppendLine();
            if(ExtendsToken!=null)
            {
                buf.AppendFormat("属于:{0};",ExtendsToken.GetText());
                buf.AppendLine();
            }
            if (NameToken != null)
            {
                buf.AppendFormat("名称:{0};", NameToken.GetText());
                buf.AppendLine();
            }
            /*
            if (this.ImportList.Count > 0)
            {
                foreach (var p in ImportList)
                {
                    buf.Append(p.ToCode());
                }
            }*/
            if (this.ImportPackage != null)
            {
                buf.Append(this.ImportPackage.ToCode());
            }

            if(this.SimpleUse!=null)
            {
                buf.Append(this.SimpleUse.ToCode());
            }

            if(PropertyList.Count>0)
            {
                foreach (var p in PropertyList)
                {
                    buf.Append(p.ToCode());
                }
            }
            foreach (var fn in MethodList)
            {
                buf.Append(fn.ToCode());
                buf.AppendLine();
            }

            foreach (var fn in AgreementList)
            {
                buf.Append(fn.ToCode());
                buf.AppendLine();
            }
           
            buf.AppendLine();
            return buf.ToString();
        }
    }
}
