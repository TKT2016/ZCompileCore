using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;

namespace ZCompileCore.Parse
{
    public partial class Parser
    {
        Exp parseExp()
        {
            Token opToken;
            Exp resultExpr = parseCompareExpr();
            while (CurrentToken.Kind == TokenKind.AND || CurrentToken.Kind == TokenKind.OR)
            {
                opToken = CurrentToken;
                MoveNext();
                Exp rightExpr = parseCompareExpr();
                resultExpr = new BinExp() { LeftExp = resultExpr, OpToken = opToken, RightExp = rightExpr };
            }
            return resultExpr;
        }

        protected Exp parseCompareExpr()
        {
            Token opToken;
            Exp resultExpr = parseAddSub();
            while (CurrentToken.Kind == TokenKind.GT || CurrentToken.Kind == TokenKind.LT || CurrentToken.Kind == TokenKind.GE
                || CurrentToken.Kind == TokenKind.LE || CurrentToken.Kind == TokenKind.NE || CurrentToken.Kind == TokenKind.EQ)
            {
                opToken = CurrentToken;
                MoveNext();
                Exp rightExpr = parseAddSub();
                resultExpr = new BinExp() { LeftExp = resultExpr, OpToken = opToken, RightExp = rightExpr };
            }
            return resultExpr;
        }

        public Exp parseAddSub()
        {
            Token opToken;
            Exp resultExpr = parseMulDiv();
            while (CurrentToken.Kind == TokenKind.ADD || CurrentToken.Kind == TokenKind.SUB)
            {
                opToken = CurrentToken;
                MoveNext();
                Exp rightExpr = parseAddSub();
                resultExpr = new BinExp() { LeftExp = resultExpr, OpToken = opToken, RightExp = rightExpr };
            }
            return resultExpr;
        }

        Exp parseMulDiv()
        {
            Token opToken;
            Exp resultExpr = parseCall();
            while (CurrentToken.Kind == TokenKind.MUL || CurrentToken.Kind == TokenKind.DIV)
            {
                opToken = CurrentToken;
                MoveNext();
                Exp rightExpr = parseMulDiv();
                resultExpr = new BinExp() { LeftExp = resultExpr, OpToken = opToken, RightExp = rightExpr };
            }
            return resultExpr;
        }

        Exp parseCall()
        {
            Exp subjExpr = parseTerm(); //parseDe();
            if (PreToken != null && CurrentToken.Line == PreToken.Line)
            {
                if (expIsInExp()) //if (CurrentKind == TokenKind.Ident || CurrentKind == TokenKind.LBS || CurrentToken.IsLiteral())
                {
                    FCallExp callexpr = new FCallExp();
                    callexpr.Elements.Add(subjExpr);
                    while(expIsInExp())//while (CurrentKind == TokenKind.Ident || CurrentKind == TokenKind.LBS || CurrentToken.IsLiteral())
                    {
                        subjExpr = parseTerm();//parseDe();
                        callexpr.Elements.Add(subjExpr);
                    }
                    return callexpr;
                }
            }
            return subjExpr;
        }

        bool expIsInExp()
        {
            if (isNewLine()) return false;
            return (CurrentKind == TokenKind.Ident || CurrentKind == TokenKind.LBS || CurrentToken.IsLiteral());
        }

        BracketExp parseBracket()
         {
             BracketExp bracket = new BracketExp();
             bracket.LeftBracketToken = CurrentToken; MoveNext();
             if (!isBracketEnd(CurrentKind))
             {
                 while (true)
                 {
                     Exp expr = parseExp();
                     if (expr != null)
                     {
                         bracket.InneExps.Add(expr);
                     }

                     if (isBracketEnd(CurrentKind))
                     {
                         break;
                     }
                     if (CurrentKind != TokenKind.Comma)
                     {
                         error("多余的表达式元素");
                         MoveNext();
                         while (!(isBracketEnd(CurrentKind)) && CurrentKind != TokenKind.Comma)
                         {
                             MoveNext();
                         }
                     }
                     if (CurrentKind == TokenKind.Comma)
                     {
                         MoveNext();
                     }
                 }
             }
            if(CurrentKind== TokenKind.RBS)
            {
                bracket.RightBracketToken = CurrentToken;
                MoveNext();
            }
            else
            {
                error("括号不匹配");
            }
             return bracket;
         }

        bool isBracketEnd(TokenKind kind)
        {
            return (kind == TokenKind.EOF || kind == TokenKind.Semi || kind == TokenKind.RBS || isNewLine());
        }

        Exp parseTerm()
        {
            if (CurrentToken.Kind == TokenKind.LBS)
            {
                return parseBracket();
            }
            else if (CurrentToken.Kind == TokenKind.RBS)
            {
                error("多余的右括号");
                MoveNext();
                return null;
            }
            else if (CurrentToken.IsLiteral())
            {
                return parseLiteral();
            }
            else if (CurrentToken.Kind == TokenKind.Ident && NextToken.Kind == TokenKind.Colon)
            {
                return pareNV();
            }
            else if (CurrentToken.Kind == TokenKind.Ident)
            {
                return parseIdent();
            }
            else
            {
                return null;
            }
        }

        Exp parseIdent()
        {
            FTextExp idexp = new FTextExp();
            idexp.IdentToken = CurrentToken;
            MoveNext();
            return idexp;
        }

        Exp parseLiteral()
        {
            LiteralExp literalex = new LiteralExp();
            literalex.LiteralToken = CurrentToken;
            MoveNext();
            return literalex;
        }

        Exp pareNV()
        {
            NameValueExp nv = new NameValueExp();
            nv.NameToken = CurrentToken;
            MoveNext();
            match(TokenKind.Colon);
            Exp exp = parseExp();
            nv.ValueExp = exp;
            return nv;
        }
    }
}

