using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Reports;

namespace ZLangSingleCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string srcFile = "sample/test.zyy";
            srcFile = "sample/对应表例子.zyy";
            srcFile = "sample/你好.zyy";
            srcFile = "sample/列表例子.zyy";
            
            if (args.Length > 0)
            {
                srcFile = args[0];
            }
            FileInfo srcFileInfo = new FileInfo(srcFile);
            ZSingleCompiler compiler = new ZSingleCompiler(srcFileInfo);
            compiler.Compile();
            if (compiler.CompileResult.HasError())
            {
                StringBuilder buffBuilder = new StringBuilder();
                buffBuilder.AppendFormat("文件'{0}'有以下错误:\n", srcFile);
                foreach (CompileMessage compileMessage in compiler.CompileResult.Errors)
                {
                    if (compileMessage.Line > 0 || compileMessage.Col > 0)
                    {
                        buffBuilder.AppendFormat("第{0}行,第{1}列", compileMessage.Line, compileMessage.Col);
                    }
                    buffBuilder.AppendFormat("错误:{0}\n", compileMessage.Text);
                }
                Console.WriteLine(buffBuilder.ToString());
            }
            else
            {
                compiler.Run();
            }
        }
    }
}
