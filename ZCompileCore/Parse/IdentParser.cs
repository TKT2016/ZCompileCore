using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.AST.Exps;
using ZCompileCore.AST.Exps.Eds;
using ZCompileCore.Lex;
using ZCompileCore.Tools;

namespace ZCompileCore.Parse
{
    public class IdentParser
    {
        Token sourceToken;
        public IdentParser()
        {       

        }
        int i = 0;
        string code;
        public Exp Parse(Token identToken,Exp srcExp)
        {
            sourceToken = identToken;
            i = 0;
            code = this.sourceToken.GetText();
            if (code.IndexOf('的') == -1 && code.IndexOf('第') == -1) return null;
            Exp exp = parseIdentExp();

            while (i < code.Length /*&& code[i] == '的'*/)
            {
                char ch = code[i];
                if(ch=='的')
                {
                    i++;
                    DeExp expDe = new DeExp();
                    expDe.SubjectExp = exp;
                    Token token = parseToken();
                    expDe.MemberToken = token;
                    exp = expDe;
                }
                else if (ch == '第')
                {
                    i++;
                    DiExp expDi = new DiExp(srcExp);
                    expDi.SubjectExp = exp;
                    Token token = parseToken();
                    if(token!=null)
                    {
                        if(TextUtil.IsInt(token.GetText()))
                        {
                            token.Kind = TokenKind.LiteralInt;
                            LiteralExp expLiteral = new LiteralExp();
                            expLiteral.LiteralToken = token;
                            expDi.ArgExp = expLiteral;
                        }
                        else
                        {
                            token.Kind = TokenKind.Ident;
                            //IdentExp expIdent = new IdentExp();
                            //expIdent.IsFinally = true;
                            VarExp varExp = new VarExp(srcExp, token);
                            //varExp.VarToken = token;
                            expDi.ArgExp = varExp;
                        }
                    }
                    exp = expDi;
                }
                i++;
            }
            return exp;
        }
        int temp = 0;

        FTextExp parseIdentExp()
        {
             Token token=parseToken();
             return createIdentExp(token);
        }

        Token parseToken()
        {
            temp = 0;
            StringBuilder buff = new StringBuilder();
            //string code = this.sourceToken.GetText();
            while (i < code.Length)
            {
                char ch = code[i];
                if (ch == '的' || ch == '第')
                {
                    //i++;
                    break;
                }
                else
                {
                    buff.Append(code[i]);
                }
                i++;
            }
            string text = buff.ToString();
            Token token = createToken(text);
            return token;
        }

        Token createToken(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            Token token = new Token();
            token.Kind = TokenKind.Ident;
            token.Line = sourceToken.Line;
            token.Col = temp + sourceToken.Col;
            token.Text = text;
            return token;
        }

        FTextExp createIdentExp(Token token)
        {
            if (token == null) return null;
            token.Kind = TokenKind.Ident;
            FTextExp exp = new FTextExp();
            exp.IdentToken = token;
            //exp.IsFinally = true;
            return exp;
        }

    }
}
