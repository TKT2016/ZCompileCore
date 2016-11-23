using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.Lex;

namespace ZCompileCore.Reports
{
    public class CompileMessage
    {
        public string FileName { get; set; }

        public int Line { get; set; }

        public int Col { get; set; }

        public string Text { get; set; }
    }

    public class CompileResult
    {
        public List<CompileMessage> Errors { get; private set; }
        public List<CompileMessage> Warnings { get; private set; }

        public CompileResult()
        {
            Errors = new List<CompileMessage>();
            Warnings = new List<CompileMessage>();
        }

        public void Clear()
        {
            Errors.Clear();
            Warnings.Clear();
        }
    }

    public static class Messager
    {
        public static CompileResult Results{ get;set;}
        public static string FileName { get; set; }
        static bool IsErrorOrWarning = false;
        static Messager()
        {
            Results = new CompileResult();
        }

        public static void Clear()
        {
            Results.Clear();
            IsErrorOrWarning = false;
        }

        public static bool HasErrorOrWarning()
        {
            return Results.Errors.Count > 0 && !IsErrorOrWarning;
        }
        
        public static void Error( string message)
        {
            CompileMessage ei = new CompileMessage() { Text=message  };
            Results.Errors.Add(ei);

            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("错误:" + message);
            Console.ForegroundColor = temp;
            IsErrorOrWarning = false;
        }

        public static void Error(int line,int col,string message)
        {
            CompileMessage ei = new CompileMessage() { FileName = Messager.FileName, Col = col, Line = line, Text = message };
            Results.Errors.Add(ei);

            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (line == -1 || col == -1)
            {
                Console.WriteLine("" + Messager.FileName + " 错误:" + message);
            }
            else
            {
                Console.WriteLine("" +  Messager.FileName + "第" + line + "行,第" + col + "列错误:" + message);
            }
            Console.ForegroundColor = temp;
            IsErrorOrWarning = false;
        }

        public static void Warning(string message)
        {
            CompileMessage ei = new CompileMessage() { Text = message };
            Results.Warnings.Add(ei);

            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("错误:" + message);
            Console.ForegroundColor = temp;
            IsErrorOrWarning = false;
        }

        public static void Warning(Token token, string messageFormat)
        {
            Warning(token.Line, token.Col, string.Format(messageFormat, token.ToCode()));
        }

        public static void Warning(int line, int col, string message)
        {
            CompileMessage ei = new CompileMessage() { FileName = Messager.FileName, Col = col, Line = line, Text = message };
            Results.Warnings.Add(ei);

            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (line == -1 || col == -1)
            {
                Console.WriteLine("警告:" + message);
            }
            else
            {
                Console.WriteLine("文件" +  Messager.FileName + " 第" + line + "行,第" + col + "列错误:" + message);
            }
            Console.ForegroundColor = temp;
            IsErrorOrWarning = false;
        }

        public static void Error(Token token, string messageFormat)
        {
            Error(token.Line, token.Col, string.Format(messageFormat, token.ToCode()));
        }

        public static void DeugOut(string message)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Debug:" + message);
            Console.ForegroundColor = temp;
            IsErrorOrWarning = false;
        }

    }
}
