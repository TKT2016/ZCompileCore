using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.Lex;
using ZCompileCore.Reports;

namespace ZCompileCore.Parse
{
    public partial class Parser
    {
        protected Token PreToken
        {
            get { 
                if (index == 0) return null;
                return this.tokens[index - 1];
            }
        }

        protected Token CurrentToken
        {
            get { return this.tokens[index]; }
        }

        protected TokenKind CurrentKind
        {
            get { return CurrentToken.Kind; }
        }

        protected string CurrentText
        {
            get { return CurrentToken.GetText(); }
        }

        protected Token NextToken
        {
            get { return this.tokens[index + 1]; }
        }

        protected void MoveNext()
        {
            index++;
        }

        void skipToSemi()
        {
            while (CurrentToken.Kind != TokenKind.Semi && CurrentToken.Kind != TokenKind.EOF)
            {
                MoveNext();
            }
            if (CurrentToken.Kind == TokenKind.Semi)
            {
                MoveNext();
            }
        }

        protected bool match(TokenKind tokKind)
        {
            if (CurrentToken.Kind!=tokKind)
            {
                error(CurrentToken, CurrentToken.ToCode() + "不正确,应该是" + Token.GetTextByKind(tokKind) );
                return false;
            }
            else
            {
                MoveNext();
                return true;
            }
        }

        bool matchSemiOrNewLine()
        {
            if (CurrentKind == TokenKind.EOF) return true;
            if (isNewLine()) return true;
            TokenKind semi = TokenKind.Semi;
            if (CurrentToken.Kind != semi)
            {
                error(CurrentToken, CurrentToken.ToCode() + "不正确,应该是" + Token.GetTextByKind(semi));
                return false;
            }
            else
            {
                MoveNext();
                return true;
            }
        }

        protected bool isNewLine()
        {
            Token preToken = PreToken;
            Token curToken = CurrentToken;
            if (preToken == null)
            {
                return true;
            }
            else if (preToken.Line < curToken.Line)
            {
                return true;
            }
            else if (preToken.Kind == TokenKind.Semi)
            {
                return true;
            }
            return false;
        }

        protected bool checkToken(TokenKind tokKind)
        {
            if (CurrentToken.Kind != tokKind)
            {
                error(CurrentToken, CurrentToken.ToCode() + "不正确,应该是" + Token.GetTextByKind(tokKind));
                return false;
            }
            else
            {
                return true;
            }
        }

        List<Token> inputLine()
        {
            List<Token> tokens = new List<Token>();
            int curline = CurrentToken.Postion.Line;
            while (CurrentToken.Kind != TokenKind.EOF&&curline == CurrentToken.Postion.Line)
            {
                tokens.Add(CurrentToken);
                MoveNext();
            }
            return tokens;
        }

        protected void error(Token tok, string str)
        {
            Messager.Error(tok.Line, tok.Col, /*" " + CurrentToken.ToString() + " - " +*/ str);
        }

        protected void error(string str)
        {
            error(CurrentToken, str);
        }

        protected void report(string str, int color = 1)
        {
            ConsoleColor c = Console.ForegroundColor;
            if (color == 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (color == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (color == 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (color == 4)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            Console.WriteLine("DEBUG:" + index + " [" + CurrentToken.Line + "," + CurrentToken.Col + "]:" + CurrentToken.ToCode() + " --- " + str);
            Console.ForegroundColor = c;
        }

    }
}

