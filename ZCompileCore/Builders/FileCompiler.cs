using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Builders;
using ZCompileCore.Lex;
using ZCompileCore.Parse;
using ZCompileCore.Reports;
using ZCompileCore.Analys;

namespace ZCompileCore.Builders
{
    public class FileCompiler
    {
        public Type Compile(ProjectContext context, string file)
        {
            FileInfo fi = new FileInfo(file);
            Messager.FileName = fi.Name;
            //this.context = context;
           // this.file = file;

            SourceReader reader = new SourceReader();
            reader.ReadFile(file);
            //Console.WriteLine("--------------- 词法分析 " + Messager.FileName + "--------------- ");
            Tokenizer tokenizer = new Tokenizer(reader);
            var tokens = tokenizer.Scan();
            outTokenList(tokens);//Console.ReadKey(); return null;
            //Console.WriteLine("--------------- 语法分析 (LEX) " + Messager.FileName + "-------------");
            Parser lexparser = new Parser(tokens);
            var fileAST = lexparser.ParseFile();
            fileAST.Init(file, context);
            //Console.WriteLine(fileAST.ToCode());
            //Console.ReadKey(); return null;
            //Console.WriteLine("--------------- 语义分析 文件生成 " + Messager.FileName + "---------------");
            Type type = fileAST.CompileFile();
            return type;
        }


        public Type Compile(ProjectContext context, ZCompileClassModel zCompileClassModel)
        {
            string srcPath = zCompileClassModel.SourceFileInfo.FullName;// zCompileClassModel.GetSrcFullPath();
            if (!File.Exists(srcPath))
            {
                Messager.Error("文件"+zCompileClassModel.SourceFileInfo+"不存在");
                return null;
            }
            FileInfo fi = new FileInfo(srcPath);
            Messager.FileName = fi.Name;

            //Console.WriteLine("--------------- 词法分析 " + Messager.FileName + "--------------- ");
            List<Token> tokens = new List<Token>( );
            List<Token> preTokens = scanPreCode(zCompileClassModel);
            tokens.AddRange(preTokens);

            List<Token> fileTokens = scanFileCode(zCompileClassModel);
            tokens.AddRange(fileTokens);
           
            //outTokenList(tokens);//Console.ReadKey(); return null;
            //Console.WriteLine("--------------- 语法分析 (LEX) " + Messager.FileName + "-------------");
            Parser lexparser = new Parser(tokens);
            var fileAST = lexparser.ParseFile();
            fileAST.Init(srcPath, context);
            //Console.WriteLine(fileAST.ToCode());
            //Console.ReadKey(); return null;
            //Console.WriteLine("--------------- 语义分析 文件生成 " + Messager.FileName + "---------------");
            Type type = fileAST.CompileFile();
            return type;
        }

        private List<Token> scanFileCode(ZCompileClassModel zCompileClassModel)
        {
            SourceReader reader = new SourceReader();
            string srcFile = zCompileClassModel.SourceFileInfo.FullName;// zCompileClassModel.GetSrcFullPath();
            reader.ReadFile(srcFile);
            List<Token> tokens2 = scanTokens(reader);
            return tokens2;
        }

        private List<Token> scanPreCode(ZCompileClassModel zCompileClassModel)
        {
            if (string.IsNullOrEmpty(zCompileClassModel.PreSourceCode)) return new List<Token>();
            SourceReader reader = new SourceReader();
            reader.ReadString(zCompileClassModel.PreSourceCode);
            List<Token> tokens2 = scanTokens(reader);
            foreach (var token in tokens2)
            {
                token.Line = -token.Line - 1;
                token.Col = token.Col - 1000;//方法体以行列区分，所以减去一些。
            }
            return tokens2;
        }

        private List<Token> scanTokens(SourceReader reader)
        {
            Tokenizer tokenizer = new Tokenizer(reader);
            List<Token> tokens = tokenizer.Scan();
            return tokens;
        }

        static void outTokenList(List<Token> list)
        {
            foreach (var s in list)
            {
                Console.WriteLine(s.ToCode());
            }
        }

        static void outList_Text(List<Token> list)
        {
            foreach (var s in list)
            {
                Console.Write(s.ToCode());
                Console.Write(" ");
            }
        }
    }
}
