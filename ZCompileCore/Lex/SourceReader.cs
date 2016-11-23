using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ZCompileCore.Lex
{
    public class SourceReader
    {
        private string _sourceText = null;
        private int pointer = -1;

        private const char END = '\0';
        //private StreamReader _reader = null;

        public SourceReader()
        {

        }

        public void ReadFile(string fileName)
        {
            //_reader = File.OpenText(fileName);
            _sourceText = File.ReadAllText(fileName, System.Text.Encoding.Default /*, Encoding.UTF8*/);
            pointer = 0;
        }

        public void ReadString(string text)
        {
            _sourceText = text;
            pointer = 0;
        }

        public void Close()
        {
            //if (_reader != null)
            //{
            //    _reader.Close();
            //}
        }

        /// <summary>
        /// 读取下一个字符，而不更改读取器状态或字符源。返回下一个可用字符，而实际上并不从输入流中读取此字符。
        /// 一个整数，它表示下一个要读取的字符，或者如果没有更多的可用字符或此流不支持查找，则为 -1。
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            if (_sourceText != null && pointer < _sourceText.Length)
            {
                return _sourceText[pointer];
            }
            //else if (_reader != null)
            //{
            //    return _reader.Peek();
            //}
            return -1;
        }

        public char PeekChar()
        {
            return (char)(Peek());
        }

        /// <summary>
        /// 读取输入流中的下一个字符并使该字符的位置提升一个字符。
        /// 输入流中的下一个字符，或者如果没有更多的可用字符，则为 -1。默认实现将返回 -1。
        /// </summary>
        /// <returns></returns>
        public int Read()
        {
            pointer++;
            if (_sourceText != null && pointer < _sourceText.Length-1)
            {
                char ch = _sourceText[pointer];
                return ch;
            }
            //else if (_reader != null)
            //{
            //    return _reader.Read();
            //}
            return -1;
        }

        public char ReadChar()
        {
            return (char)(Read());
        }

        public char GetNextChar()
        {
            if (_sourceText != null && pointer < _sourceText.Length-1)
            {
                char ch = _sourceText[pointer+1];
                return ch;
            }
            return END;
        }
    }
}
