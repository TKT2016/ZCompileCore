using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.AST;
using ZCompileCore.AST.Exps;
using ZCompileCore.AST.Parts;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;

namespace ZCompileCore.Parse
{
    public partial class Parser
    {
        protected List<Token> tokens;
        protected int index;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.tokens.Add(Token.EOF);
            this.tokens.Add(Token.EOF);
            this.tokens.Add(Token.EOF);
            this.index = 0;
        }
        FileAST prog;
        public FileAST ParseFile()
        {
            prog = new FileAST();
            while (CurrentToken.Kind != TokenKind.EOF)
            {
                if (CurrentToken.IsKeyIdent("使用包"/*,"使用类型","简略使用"*/) && NextToken.Kind == TokenKind.Colon)
                {
                    var ast = ParseImport();
                    prog.Add(ast);
                }

                if (CurrentToken.IsKeyIdent("简略使用") && NextToken.Kind == TokenKind.Colon)
                {
                    var ast = ParseSimpleUse();
                    prog.Add(ast);
                }

                if (CurrentKind == TokenKind.Ident && CurrentText == "过程" && NextToken.Kind == TokenKind.Colon)
                {
                    MethodAST ast = ParseMethod();
                    prog.Add(ast);
                }
                else if (CurrentKind == TokenKind.Ident && CurrentText == "属性" && NextToken.Kind == TokenKind.Colon)
                {
                    parseStaticPropertyPart();
                }
                else if (CurrentToken.IsKeyIdent("属于") && NextToken.Kind == TokenKind.Colon)
                {
                    parseExtends();
                }
                else if (CurrentToken.IsKeyIdent("名称") && NextToken.Kind == TokenKind.Colon)
                {
                    parseName();
                }
                else if (CurrentToken.IsKeyIdent("约定") && NextToken.Kind == TokenKind.Colon)
                {
                    parseAgreement();
                }
                else
                {
                    error("无法识别'"+CurrentToken.GetText()+"'");
                    MoveNext();
                }
            }
            return prog;
        }

        void parseAgreement()
        {
            var tempToken = CurrentToken;
            MoveNext();//跳过"约定"
            MoveNext();//跳过":"
            AgreementAST agreeast = new AgreementAST();
            agreeast.KeyToken = tempToken;
            List<Token> items = parseAgreementBlock(tempToken.Postion);
            agreeast.ValueList.AddRange(items);
            prog.Add(agreeast);
        }

        List<Token> parseAgreementBlock(CodePostion parentPostion)
        {
            List<Token> list = new List<Token>();
            while (CurrentToken.Kind != TokenKind.EOF && CurrentToken.Col > parentPostion.Col)
            {
                Token temp = parseEnumItem();
                if (temp != null)
                {
                    list.Add(temp);
                }
            }
            return list;
        }

        Token parseEnumItem()
        {
            if (CurrentKind == TokenKind.Ident)
            {
                Token temp = CurrentToken;
                MoveNext();
                matchSemiOrNewLine();//match(TokenKind.Semi);
                return temp;
            }
            else
            {
                error("规定的成员不正确");
                MoveNext();
            }
            return null;
        }

        void parseExtends()
        {
            MoveNext();
            MoveNext();
            prog.ExtendsToken = CurrentToken;
            MoveNext();
            matchSemiOrNewLine();//match(TokenKind.Semi);
        }

        void parseName()
        {
            MoveNext();
            MoveNext();
            prog.NameToken = CurrentToken;
            MoveNext();
            matchSemiOrNewLine();////match(TokenKind.Semi);
        }

        void parseStaticPropertyPart()
        {
            Token tempToken = CurrentToken;
            MoveNext();//跳过"属性"
            MoveNext();//跳过":"
            while (CurrentToken.Kind != TokenKind.EOF && CurrentToken.Col > tempToken.Col)
            {
                PropertyAST ast = parseStaticProperty(tempToken.Postion);
                prog.Add(ast);
            }
        }

        PropertyAST parseStaticProperty(CodePostion parentPostion)
        {
            PropertyAST ast = new PropertyAST();
            if(CurrentKind!= TokenKind.Ident)
            {
                error("不是正确的名称");
            }
            else
            {
                ast.TypeToken = CurrentToken;
            }
            MoveNext();
            match(TokenKind.Colon);
            if (CurrentKind != TokenKind.Ident)
            {
                error("不是正确的变量名称");
            }
            else
            {
                ast.NameToken = CurrentToken;
            }
            MoveNext();
            if(CurrentKind== TokenKind.Assign)
            {
                MoveNext();
                Exp exp = parseExp();
                ast.ValueExp = exp;
            }
            matchSemiOrNewLine();//match(TokenKind.Semi);
            return ast;
        }
       
        public ImportPackageAST ParseImport()
        {
            ImportPackageAST ast = new ImportPackageAST();
            ast.KeyToken = CurrentToken;
            MoveNext();//跳过"使用"
            MoveNext();//跳过":"
            ast.Packages = parsePackageList();
            matchSemiOrNewLine();//match(TokenKind.Semi);
            return ast;
        }

        List<PackageAST> parsePackageList()
        {
            List<PackageAST> list = new List<PackageAST>();
            if (CurrentKind == TokenKind.Ident)
            {
                PackageAST ast = parsePackage();
                if(ast!=null)
                {
                    list.Add(ast);
                }
                while (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        PackageAST ast2 = parsePackage();
                        if (ast != null)
                        {
                            list.Add(ast2);
                        }
                    }
                    else
                    {
                        //error("错误的类型名称");
                        break;
                    }
                }
            }
            else
            {
                error("错误的开发包名称");
                MoveNext();
            }
            return list;
        }

        PackageAST parsePackage()
        {
            List<Token> tokens = new List<Token>();
            if(CurrentKind== TokenKind.Ident /*|| CurrentToken.IsAtomData()*/)
            {
                tokens.Add(CurrentToken);
                MoveNext();
                while(CurrentKind== TokenKind.DIV)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident /*|| CurrentToken.IsAtomData()*/)
                    {
                        tokens.Add(CurrentToken);
                        MoveNext();
                    }
                    else
                    {
                        error("错误的开发包名称");
                    }
                }
            }
            else
            {
                error("错误的开发包名称");
                MoveNext();
            }
            PackageAST package = new PackageAST();
            package.PackageTokens = tokens;
            return package;
        }

        public SimpleUseAST ParseSimpleUse()
        {
            SimpleUseAST ast = new SimpleUseAST();
            ast.KeyToken = CurrentToken;
            MoveNext();//跳过"使用"
            MoveNext();//跳过":"
            ast.NameTokens = parseNames();
            matchSemiOrNewLine();// match(TokenKind.Semi);
            return ast;
        }

        List<Token> parseNames()
        {
            List<Token> tokens = new List<Token>();
            if (CurrentKind == TokenKind.Ident)
            {
                tokens.Add(CurrentToken);
                MoveNext();
                while (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident )
                    {
                        tokens.Add(CurrentToken);
                        MoveNext();
                    }
                    else
                    {
                        error("错误的类型名称");
                    }
                }
            }
            else
            {
                error("错误的类型名称");
                MoveNext();
            }
            return tokens;
        }
       
        public MethodAST ParseMethod()
        {
            var tempToken = CurrentToken;
            MoveNext();//跳过"过程"
            MoveNext();//跳过":"
            FnName fnname = parseFnName();
            BlockStmt body = parseBlock(tempToken.Postion, 2);
            if(fnname!=null /*&&body!=null*/)
            {
                MethodAST fnp = new MethodAST();
                fnp.KeyToken = tempToken;
                fnp.FnName = fnname;
                fnp.FnBody = body;
                return fnp;
            }
            return null;
        }

        FnName parseFnName()
        {
            FnName fname = new FnName();
            var curline = CurrentToken.Line;
            while (CurrentToken.Kind != TokenKind.EOF && curline==CurrentToken.Line)
            {
                if(CurrentKind== TokenKind.LBS)
                {
                    FnBracket arg = parseFnMuArg();
                    if(arg!=null)
                    {
                        fname.NameTerms.Add(arg);
                    }
                }
                else if(CurrentKind== TokenKind.Ident)
                {
                    FnText textt = parseFnText();
                    if (textt != null)
                    {
                        fname.NameTerms.Add(textt);
                    }
                }
                else if(CurrentKind== TokenKind.AssignTo)
                {
                    if(fname.NameTerms==null ||fname.NameTerms.Count==0 )
                    {
                        error("过程没有名称");
                        MoveNext();
                    }
                    else
                    {
                        MoveNext();
                        if(/*CurrentToken.IsAtomData()||*/CurrentKind== TokenKind.Ident)
                        {
                            fname.RetToken = CurrentToken;
                            MoveNext();
                        }
                        else
                        {
                            error("错误的过程结果");
                            MoveNext();
                        }
                    }
                }
                else
                {
                    error("错误的过程名称");
                    MoveNext();
                }
            }
            return fname;
        }

        FnText parseFnText()
        {
            FnText textterm = new FnText ();
            textterm.TextToken = CurrentToken;
            MoveNext();
            return textterm;
        }

        FnBracket parseFnMuArg()
        {
            FnBracket marg = new FnBracket();
            marg.LeftBracketToken = CurrentToken;
            MoveNext();
            while (!isBracketEnd(CurrentKind))
            {
                FnArg sarg = parseFnArg();
                if(sarg!=null)
                {
                    marg.Args.Add(sarg);
                }
                if (CurrentKind == TokenKind.Comma)
                {
                    MoveNext();
                }
            }
            if (CurrentKind == TokenKind.RBS)
            {
                marg.RightBracketToken = CurrentToken;
                MoveNext();
            }
            else
            {
                error("括号不匹配");
            }
            return marg;
        }

        FnArg parseFnArg()
        {
            FnArg argt = new FnArg();
            if(CurrentKind== TokenKind.Ident)
            {
                argt.ArgTypeToken = CurrentToken;
                MoveNext();
                if(CurrentKind== TokenKind.Colon)
                {
                    MoveNext();
                    if (CurrentKind == TokenKind.Ident)
                    {
                        argt.ArgNameToken = CurrentToken;
                        MoveNext();
                        return argt;
                    }
                    else
                    {
                        error("参数名称不正确");
                    }
                }
                else
                {
                    error("应该是':'");
                }
            }
            else
            {
                error("参数类型不正确");
                MoveNext();
            }
            return null;
        }

    }
}
