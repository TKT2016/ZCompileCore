using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Collections;

namespace ZCompileCore.Lex
{
    public class Token
    {
        public static PairDict<string, TokenKind> Dict{get;private set;}
        public static Token EOF = new Token() { Kind= TokenKind.EOF };

        public Token()
        {

        }

        public Token(string text,TokenKind kind,int line ,int col)
        {
            Text = text;
            Kind = kind;
            Line = line;
            Col = col;
        }

        public bool IsSplited { get; set; }

        static Token()
        {
            Dict = new PairDict<string, TokenKind>();

            Dict.Add("+", TokenKind.ADD);
            Dict.Add("-", TokenKind.SUB);
            Dict.Add("/", TokenKind.DIV);
            Dict.Add("*", TokenKind.MUL);

            Dict.Add("并且", TokenKind.AND);
            Dict.Add("或者", TokenKind.OR);
            Dict.Add("是", TokenKind.True);
            Dict.Add("否", TokenKind.False);

            Dict.Add("=", TokenKind.Assign);
            Dict.Add("=>", TokenKind.AssignTo);

            //Dict.Add("的", TokenKind.DE);
           
            Dict.Add("==", TokenKind.EQ);
            Dict.Add("!=", TokenKind.NE);
            Dict.Add(">=", TokenKind.GE);
            Dict.Add(">", TokenKind.GT);
            Dict.Add("<", TokenKind.LT);
            Dict.Add("<=", TokenKind.LE);

            //Dict.Add("整数", TokenKind.INT);
            //Dict.Add("浮点数", TokenKind.FLOAT);
            //Dict.Add("文本", TokenKind.String);
            //Dict.Add("事物", TokenKind.Object);

            Dict.Add("(", TokenKind.LBS);
            Dict.Add(")", TokenKind.RBS);
            Dict.Add(",", TokenKind.Comma);
            Dict.Add(";", TokenKind.Semi);
            //Dict.Add("::", TokenKind.Colond);
            Dict.Add(":", TokenKind.Colon);

            //Dict.Add("启动", TokenKind.Main);
            
            Dict.Add("如果", TokenKind.IF);
            Dict.Add("否则", TokenKind.ELSE);
            Dict.Add("否则如果", TokenKind.ELSEIF);
            Dict.Add("循环当", TokenKind.While);
            Dict.Add("循环每一个", TokenKind.Foreach);
            Dict.Add("捕捉", TokenKind.Catch);
            Dict.Add("说明:", TokenKind.Caption);
            Dict.Add("空", TokenKind.NULL);

        }

        public int Line { get; set; }
        public int Col { get; set; }
        public string Text { private get; set; }
        public TokenKind Kind { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1}){2}:{3}", Line, Col, Kind,Text);
        }

        public static string GetTextByKind(TokenKind kind)
        {
            if (kind == TokenKind.Ident)
            {
                return null;
            }
            if(Dict.Containsv(kind))
            {
                return Dict.Getk(kind);
            }
            return null;
        }

        public static TokenKind GetKindByText(string text)
        {
            if (Dict.Containsk(text))
            {
                return Dict.Getv(text);
            }
            return TokenKind.Ident;
        }

        public string GetText()
        {
            if (Dict.Containsv(this.Kind))
            {
                return Dict.Getk(this.Kind);
            }
            else
            {
                return this.Text;
            }         
        }

        public string ToCode()
        {
            if(this.Kind == TokenKind.LiteralString)
            {
                return "\"" + this.Text +"\"";
            }
            else if (this.Kind == TokenKind.LiteralInt || this.Kind == TokenKind.LiteralFloat)
            {
                return this.Text ;
            }
            else if (this.Kind == TokenKind.Ident)
            {
                return this.Text;
            }
            else if (this.Kind == TokenKind.Unknow)
            {
                return this.Text;
            }
            else
            {
                if(Dict.Containsv(this.Kind))
                {
                    return Dict.Getk(this.Kind);
                }
                else
                {
                    return /*this.Kind.ToString()+":"+*/this.Text;
                }            
            }
        }
       
        public bool IsLiteral()
        {
            return Kind == TokenKind.LiteralInt
                || Kind == TokenKind.LiteralFloat
                || Kind == TokenKind.NULL
                || Kind == TokenKind.LiteralString
                || Kind == TokenKind.True
                || Kind == TokenKind.False
                   ;
        }

        public bool IsKeyIdent(params string[] texts)
        {
            if (this.Kind != TokenKind.Ident) return false;
            foreach(var str in texts  )
            {
                if(str == this.Text)
                {
                    return true;
                }
            }
            return false;
        }
        /*
        public bool IsAtomData()
        {
            return Kind == TokenKind.String
                   || Kind == TokenKind.INT
                   || Kind == TokenKind.FLOAT
                   ;
        }*/

        CodePostion _Postion;
        public CodePostion Postion
        {
            get
            {
                if(_Postion==null)
                    _Postion = new CodePostion(Line, Col);
                return _Postion;
            }
        }
    }
}
