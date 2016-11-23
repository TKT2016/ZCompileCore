using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Lex;
using ZCompileCore.Parse;
using ZCompileCore.Reports;
using ZCompileCore.Analys;

namespace ZCompileCore
{
    public class ZLangCompiler
    {
        static void Main(string[] args)
        {/*
            ProjectContext project = new ProjectContext();
            project.RootNameSpace = "测试TKT";


            string src = "";
            //src = "测试.txt";
            //src = "sample/定时器测试.tktj";
            //src = "sample/测试从网页下载图片.tktj";
            //src = "sample/系统辅助测试.tktj";
            src = "sample/测试绘图.tktj";

            if (args.Length > 0 &&  !string.IsNullOrEmpty( args[0]))
            {
                src = args[0];
            }
            Compile(src, project);*/
            string src = "";
            src = "sample/test.tkt";
            //src = "sample/优酷最多播放电影.tkt";
            //src = "sample/循环语句例子.tkt";
            //src = "sample/下载美女图片.tkt";
            //src = "sample/你好.tkt";
            //src = "sample/对应表例子.tkt";
            //src = "sample/测试如果.tkt";
            src = "sample/测试每一个.tkt";

            //src = "sample/绘图窗体测试/项目.tktxm";
            //src = "sample/系统辅助测试.tktj";
            //src = "sample/测试从网页下载图片.tktj";
            //src = "sample/定时器测试.tktj";
            //src = "sample/打飞机/Project.tktxm";
            //src = "sample/打印时间.tktj";
           
            CompilePorject(src);
            Console.ReadKey();
        }

        public static CompileResult CompilePorject(string xmfile)
        {/*
            ProjectCompiler pc = new ProjectCompiler();
            string binfile = pc.Compile(xmfile).BinaryFilePath;
            if(!string.IsNullOrEmpty(binfile))
            {
                Console.WriteLine("编译成功");
                Process.Start(binfile);
            }
            return Messager.Results;*/
            return null;

        }

        public static CompileResult Compile(string srcfile, ProjectContext project)
        {
            Messager.Clear();
            //ContextClass fileContext = new ContextClass(project);
            //fileContext.SourceFileInfo = new FileInfo(srcfile);

            SourceReader reader = new SourceReader();
            reader.ReadFile(srcfile);

            Console.WriteLine("--------------- 词法分析 --------------- ");
            Tokenizer tokenizer = new Tokenizer(reader);
            var tokens = tokenizer.Scan();

            //outTokenList(tokens);//Console.ReadKey(); return null;
            
            Console.WriteLine("--------------- 语法分析 (LEX) -------------");
            Parser lexparser = new Parser(tokens);
            var fileAST = lexparser.ParseFile();
            //fileAST.FileContext = fileContext;
            fileAST.Init(srcfile, project);
            Console.WriteLine(fileAST.ToCode());
            //Console.ReadKey(); return null;

            //Console.WriteLine("--------------- 语义分析 ---------------");
            Console.WriteLine("--------------- 语义分析 文件生成 ---------------");
            //Analysiser angly = new Analysiser(fileAST, project);
            //angly.Analysise();
            project.LoadRefTypes();
            fileAST.CompileFile();

            string exeFileName = fileAST.GetBinFilePath();
            if (Messager.HasErrorOrWarning() == false && !string.IsNullOrEmpty(exeFileName))
            {
                Console.WriteLine("编译成功");
                Process.Start(exeFileName);
            }
            return Messager.Results;
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
