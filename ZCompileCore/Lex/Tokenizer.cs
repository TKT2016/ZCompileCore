using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ZCompileCore.Reports;
using ZCompileCore.Tools;

namespace ZCompileCore.Lex
{
    public class Tokenizer
    {
        int line;
        int col;
        SourceReader reader;
        private const char END = '\0';
        private const int EOF = -1; 

        public Tokenizer(SourceReader sr)
        {
            reader = sr;
            line = 1;
            col =1;
        }

        private void report(string msg)
        {
            Console.WriteLine("[" + line + "," + col + "]:" + (ch != '\n' ? ch.ToString() : "(换行)") + ":" + msg);
        }

        char ch
        {
            get
            {
                if (reader.Peek() == -1) return END;
                return reader.PeekChar();
            }
        }

        public char Next()
        {
            col++;
            if (ch == '\t') col+=3;
            return reader.ReadChar();
        }

        public char GetNext()
        {
            return reader.GetNextChar();
        }
        
        List<Token> tokenList = new List<Token>();
        public List<Token> Scan()
        {
            //report("开始");
            tokenList.Clear();
            while (ch != END)
            {
                //report("ch="+ch+" "+(int)ch);
                char nextChar = GetNext();
                if (ch==' ' || ch=='\t')
                {
                    //report("SkipSpace");
                    SkipWhiteSpace();
                }
                else if (ch == '/' && nextChar == '/')
                {
                    SkipSingleLineComment();
                }
                else if (ch == '/' && nextChar == '*')
                {
                    SkipMutilLineComment();
                }
                else if (ch == '/')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.DIV };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == '"'||ch=='“'||ch=='”')
                {
                    string str = scanString();
                    Token tok = new Token() { Col = col - 1, Line = line, Text = str, Kind = TokenKind.LiteralString };
                    tokenList.Add(tok);
                }
                else if (ch == '\r' && nextChar == '\n')
                {
                    //report("扫描换行符");
                    Next(); Next();
                    col = 1;
                    line++;
                }
                else if (ch == '\n')
                {
                    //report("扫描换行符");
                    SkipLine();
                }
                else if (ch == '\r')
                {
                    //report("扫描换行符");
                    SkipLine();
                }
                else if ( "0123456789".IndexOf(ch)!=-1)
                {
                    string str = scanNumber();
                    var temp = col;
                    if (TextUtil.IsInt(str))
                    {
                        Token tok = new Token() { Col = temp, Line = line, Text = str, Kind = TokenKind.LiteralInt };
                        tokenList.Add(tok);
                    }
                    else if (TextUtil.IsFloat(str))
                    {
                        Token tok = new Token() { Col = temp, Line = line, Text = str, Kind = TokenKind.LiteralFloat };
                        tokenList.Add(tok);
                    }
                    else
                    {
                        lexError(str+"不是正确的数字");
                    }
                }
                else if (ch == '+' || ch == '＋')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.ADD };
                    tokenList.Add(tok);
                    Next(); 
                }
                else if (ch == '-' || ch == '－')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.SUB };
                    tokenList.Add(tok);
                    Next();
                }
                else if ((ch == '=' || ch == '＝') && (nextChar == '=' || nextChar == '＝'))
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.EQ };
                    tokenList.Add(tok);
                    Next(); Next();
                }
                else if ((ch == '=' || ch == '＝') && (nextChar == '>'))
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.AssignTo };
                    tokenList.Add(tok);
                    Next(); Next();
                }
                else if ((ch == '=' || ch == '＝'))
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.Assign };
                    tokenList.Add(tok);
                    Next();
                }
                else if ((ch == '*'))
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.MUL };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == ','||ch=='，')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.Comma };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == ';' || ch == '；')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.Semi };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == '(' || ch == '（')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.LBS };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == ')' || ch == '）')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.RBS };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == '>' && GetNext() == '=')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.GE };
                    tokenList.Add(tok);
                    Next(); Next();
                }
                else if (ch == '>')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.GT };
                    tokenList.Add(tok);
                    Next();
                }
                else if (ch == '<' && nextChar == '=')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.LE };
                    tokenList.Add(tok);
                    Next(); Next();
                }
                else if (ch == '<')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.LT };
                    tokenList.Add(tok);
                    Next();
                }
                else if ((nextChar == '!' || nextChar == '！')&& (nextChar == '=' || nextChar == '＝'))
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.NE };
                    tokenList.Add(tok);
                    Next(); Next();
                }
                /*else if (ch == ':' && nextChar == ':')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.Colond };
                    tokenList.Add(tok);
                    Next(); Next();
                }*/
                else if (ch == ':' || ch == '：')
                {
                    Token tok = new Token() { Col = col, Line = line, Kind = TokenKind.Colon };
                    tokenList.Add(tok);
                    Next();
                }
                else if ((ch >= 'A' && ch <= 'Z') /*|| (ch == '_') */|| (ch >= 'a' && ch <= 'z') || ChineseHelper.IsChineseLetter(ch))
                {
                    var tempCol = col;
                    var tempLine = line;
                    Token t1 = scanKeyIdent();
                    //if (t1.GetText().StartsWith("否则如果") || t1.GetText().StartsWith("否则") || t1.GetText().StartsWith("如果"))
                    //{
                    //    Console.WriteLine("否则如果");
                    //}
                    //tokenList.Add(t1);
                    if (t1.GetText() == "说明")
                    {
//                      char nchar = GetNext();
                        if (ch == ':' || ch == '：')
                        {
                            SkipSingleLineComment();
                            continue;;
                        }
                    }
                    addIdentOrKey(t1);
                }
                else if (char.IsControl(ch))
                {
                    while (char.IsControl(ch)&&ch!=END)
                    {
                        Next();
                        if ((int)ch == 13)
                        {
                            line++;
                            col = 1;
                        }
                    }
                }
                else
                {
                    lexError("无法识别"+(int)ch+": '" + ch+"' ");
                    Next();
                }
            }
            return tokenList;
        }

        void addIdentOrKey(Token token)
        {
            if(!isNewLine())
            {
                tokenList.Add(token);
                return;
            }
            if (processKeyIdent(token, "如果",TokenKind.IF)) return;
            if (processKeyIdent(token, "否则如果", TokenKind.ELSEIF)) return;
            if (processKeyIdent(token, "否则", TokenKind.ELSE)) return;
            if (processKeyIdent(token, "循环当", TokenKind.While)) return;
            tokenList.Add(token);
        }

        bool processKeyIdent(Token token,string keyContent,TokenKind keyKind)
        {
            string srcContent = token.GetText();
            if (!srcContent.StartsWith(keyContent)) return false;
            Token keyToken = new Token(keyContent, keyKind, token.Line, token.Col);
            tokenList.Add(keyToken);
            if (srcContent.Length > keyContent.Length)
            {
                int keyLength = keyContent.Length;
                Token identToken = new Token(srcContent.Substring(keyLength), TokenKind.Ident, token.Line, token.Col + keyLength);
                tokenList.Add(identToken);
            }
            return true;
        }

        Token getPreToken()
        {
            if (tokenList.Count == 0) return null;
            int lastIndex = tokenList.Count - 1;
            return tokenList[lastIndex];
        }

        bool isNewLine()
        {
            Token preToken = getPreToken();
            if (preToken == null)
            {
                return true;
            }
            else if (preToken.Line < this.line)
            {
                return true;
            }
            else if (preToken.Kind == TokenKind.Semi)
            {
                return true;
            }
            return false;
        }

        Token scanKeyIdent()
        {
            var tempCol = col;
            string idtext = scanIdent();
            TokenKind kind = Token.GetKindByText(idtext);
            Token token = new Token() { Col = tempCol, Line = line, Kind = kind };
            if (kind == TokenKind.Ident)
            {
                token.Text = idtext;
            }
            return token;
        }

        private string scanIdent()
        {
            StringBuilder buf = new StringBuilder();
            while (is_identifier_part_character(ch) /*&& ch != '的'*/)
            {
                buf.Append(ch);
                Next();
                //string tempstr = buf.ToString();
                //if (Token.Dict.Containsk(tempstr))
                //{
                //    return tempstr;
                //}
            }
            return buf.ToString();
        }

        void lexError(string msg)
        {
            Messager.Error(line, col, msg);
        }

        private void SkipLine()
        {
            Next();
            col = 1;
            line++;
        }

        private void SkipWhiteSpace()
        {
            while (ch == ' ' || ch == '\t')
            {
                Next();
            }
        }

        private string scanNumber()
        {
            StringBuilder buf = new StringBuilder();
            while ("0123456789.".IndexOf(ch) != -1)
            {
                buf.Append(ch);
                Next();
            }
            return buf.ToString();
        }

        private void SkipSingleLineComment()
        {
            while (ch != '\n')
            {
                Next();
            }
        }

        private void SkipMutilLineComment()
        {
            while (ch!=END)
            {
                char nextChar = GetNext();
                if (ch == '*' && (nextChar == '/' ))
                {
                    Next();
                    Next();
                    break;
                }
                else if (ch == '\r' && nextChar == '\n')
                {
                    //report("扫描换行符");
                    //SkipLine();
                    Next(); Next();
                    col = 1;
                    line++;
                }
                else if (ch == '\n')
                {
                    //report("扫描换行符");
                    SkipLine();
                }
                else if (ch == '\r')
                {
                    //report("扫描换行符");
                    SkipLine();
                }
                else
                {
                    Next();
                }
            }
        }      

        static bool is_identifier_start_character(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || Char.IsLetter(c);
        }

        static bool is_identifier_part_character(char c)
        {
            return (c >= 'a' && c <= 'z') 
                || (c >= 'A' && c <= 'Z') 
                //|| c == '_' 
                || (c >= '0' && c <= '9') 
                || ChineseHelper.IsChineseLetter(c)
                ;
        }

        char scanEscapeChar()
        {
            //report("scanChar " + ch);
            Next();
            char temp;

            //if (ch != '\\' && ch !=END )
            //{
            //    temp = ch;
            //}
            //else
            //{
            //    Next();
            switch (ch)
            {
                case 'a':
                    temp = '\a'; break;
                case 'b':
                    temp = '\b'; break;
                case 'n':
                    temp = '\n'; break;
                case 't':
                    temp = '\t'; break;
                case 'v':
                    temp = '\v'; break;
                case 'r':
                    temp = '\r'; break;
                case '\\':
                    temp = '\\'; break;
                case 'f':
                    temp = '\f'; break;
                case '0':
                    temp = '\0'; break;
                case '"':
                    temp = '"'; break;
                case '\'':
                    temp = '\''; break;
                default:
                    lexError("错误的转义符`\\" + ch + "'");
                    return ch;
            }
            Next();
            //}
            //Next();
            /*if (ch != '\'')
            {
                lexError("字符的长度大于1");
                while (ch != '\'' && ch != END && ch != '\n' && (int)ch != 13)
                {
                    Next();
                }
            }
            if (ch == '\'')
            {
                Next();
            }*/
            return temp;
        }   
         
        string scanString()
        {
            Next();
            StringBuilder buf = new StringBuilder();
            while (ch !=END )
            {
                if (ch == '\\')
                {
                    var c = scanEscapeChar();
                    buf.Append(c);
                }
                else if (ch == '"' || ch == '“' || ch == '”')
                {
                    Next();
                    return buf.ToString();
                }
                else if (ch == '\n')
                {
                    line++;
                    buf.Append("\n");
                }
                else
                {
                    buf.Append(ch);
                    Next();
                }
            }
            lexError("文本没有对应的结束双引号");
            return buf.ToString();
        }
    }
}

