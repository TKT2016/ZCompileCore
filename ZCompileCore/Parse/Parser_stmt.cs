using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZCompileCore.AST.Exps;
using ZCompileCore.AST.Stmts;
using ZCompileCore.Lex;
using ZCompileCore.Tools;


namespace ZCompileCore.Parse
{
    public partial class Parser
    {
        List<Stmt> ParseFuncBody()
        {
            int funcBodyDeep = 2;
            List<Stmt> stmtlist = new List<Stmt>();
            while (CurrentToken.Kind != TokenKind.EOF)
            {
                Stmt temp = parseStmt(funcBodyDeep);
                if (temp != null)
                {
                    stmtlist.Add(temp);
                }
            }
            return stmtlist;
        }

        Stmt parseStmt(int deep)
        {
            //report("parseStmt()"+ CurrentToken.ToString());
            Stmt stmt = null;
            if (CurrentToken.Kind == TokenKind.EOF)
            {
                return null;
            }
            else if (CurrentToken.Kind == TokenKind.IF)
            {
                stmt = parseIf(deep);
                return stmt;
            }
            else if (CurrentToken.Kind == TokenKind.While)
            {
                stmt = ParseWhile(deep);
                return stmt;
            }
            else if (CurrentToken.Kind == TokenKind.Foreach)
            {
                stmt = ParseForeach(deep);
                return stmt;
            }
            else if (CurrentToken.Kind == TokenKind.Catch)
            {
                stmt = ParseCatch(deep);
                return stmt;
            }
            else if (CurrentToken.Kind == TokenKind.Semi)
            {
                MoveNext();
                return null;
            }
            else
            {
                stmt = ParseAssignOrExprStmt();
            }
            if(stmt!=null)
            {
                stmt.Deep = deep;
            }
            return stmt;
        }

        BlockStmt parseBlock(CodePostion parentPostion,int deep)
        {
            //report("parseBlock()");
            BlockStmt blockStmt = new BlockStmt();
            List<Stmt> arr = new List<Stmt>();
            while (CurrentToken.Kind != TokenKind.EOF && CurrentToken.Col > parentPostion.Col)
            {
                Stmt temp = parseStmt(deep+1);
                if (temp != null)
                {
                    blockStmt.Add(temp);
                }
            }
            return blockStmt;
        }

        Stmt parseIf(int deep)
        {
            IfStmt ifStmt = new IfStmt();
            IfStmt.IfTrueStmt ifPart = parseTruePart(deep);
            ifStmt.Parts.Add(ifPart);
            while (CurrentToken.Kind == TokenKind.ELSEIF)
            {
                IfStmt.IfTrueStmt elseifPart = parseTruePart(deep);
                ifStmt.Parts.Add(elseifPart);
            }
            if (CurrentToken.Kind== TokenKind.ELSE)
            {
                CodePostion pos = CurrentToken.Postion;
                MoveNext();
                ifStmt.ElsePart = parseBlock(pos, deep);
            }
            return ifStmt;
        }

        IfStmt.IfTrueStmt parseTruePart(int deep)
        {
            IfStmt.IfTrueStmt eistmt = new IfStmt.IfTrueStmt();
            eistmt.KeyToken = CurrentToken;
            MoveNext();//跳过否则如果
            eistmt.Condition = parseCondition();
            eistmt.Deep = deep;
            eistmt.Body = parseBlock(eistmt.Postion,deep+1);
            return eistmt;
        }

        Exp parseCondition()
        {
            //match(TokenKind.LBS);
            Exp exp = parseExp();
            //match(TokenKind.RBS);
            return exp;
        }

        public Stmt ParseWhile(int deep)
        {
            //report("ParseWhile()");
            checkToken(TokenKind.While);
            WhileStmt whileStmt = new WhileStmt();
            whileStmt.WhileToken = CurrentToken;
            MoveNext();
            whileStmt.Condition = parseCondition();
            whileStmt.Body = parseBlock(whileStmt.Postion,deep);
            return whileStmt;
        }

        public Stmt ParseForeach(int deep)
        {
            //report("ParseForeach()");
            checkToken(TokenKind.Foreach);
            ForeachStmt foreachStmt = new ForeachStmt();
            foreachStmt.ForeachToken = CurrentToken;
            MoveNext();
            match(TokenKind.LBS);
            foreachStmt.ListExp = parseExp();
            match(TokenKind.Comma);
            foreachStmt.ElementToken = CurrentToken;
            MoveNext();
            if(CurrentKind==TokenKind.Comma)
            {
                MoveNext();
                foreachStmt.IndexToken = CurrentToken;
                MoveNext();
            }
            match(TokenKind.RBS);
            foreachStmt.Body = parseBlock(foreachStmt.Postion, deep);          
            return foreachStmt;
        }

        public Stmt ParseCatch(int deep)
        {
            //report("ParseCatch()");
            checkToken(TokenKind.Catch);
            CatchStmt catchStmt = new CatchStmt();
            catchStmt.CatchToken = CurrentToken;
            MoveNext();
            match(TokenKind.LBS);
            catchStmt.ExceptionTypeToken = CurrentToken;
            MoveNext();
            match(TokenKind.Colon);
            catchStmt.ExceptionNameToken = CurrentToken;
            MoveNext();
            match(TokenKind.RBS);
            catchStmt.CatchBody = parseBlock(catchStmt.Postion, deep);
            return catchStmt;
        }

        Stmt ParseAssignOrExprStmt( )
        {
            Exp startExpr = parseExp();
            if (CurrentToken.Kind == TokenKind.Assign )
            {
                return parseAssign(startExpr);
            }
            else if (CurrentToken.Kind == TokenKind.AssignTo)
            {
                return parseAssignTo(startExpr);
            }
            else if (CurrentKind == TokenKind.Semi || CurrentKind== TokenKind.EOF|| isNewLine())
            {
                return parseCall(startExpr);
            }       
            else
            {
                error("ParseAssignOrExprStmt 无法识别" + CurrentToken.ToCode());
                return null;
            }
        }

        Stmt parseCall(Exp start)
        {
            CallStmt stmtcall = new CallStmt();
            stmtcall.CallExpr = start;
            matchSemiOrNewLine();//match(TokenKind.Semi);
            return stmtcall;
        }

        Stmt parseAssign(Exp start)
        {
            AssignStmt stmtassign = new AssignStmt();
            stmtassign.LeftToExp = start;
            //if(start is FTextExp)
            //{
            //    FTextExp idexp = start as FTextExp;
            //    //spliter.Add(idexp.IdToken);
            //}
            MoveNext();
            stmtassign.RightValueExp = parseExp();
            matchSemiOrNewLine();//match(TokenKind.Semi);
            return stmtassign;
        }

        Stmt parseAssignTo(Exp start)
        {
            AssignStmt stmtassign = new AssignStmt();
            stmtassign.IsAssignTo = true;
            stmtassign.RightValueExp = start;
            MoveNext();
            stmtassign.LeftToExp = parseExp();
            matchSemiOrNewLine();//match(TokenKind.Semi);
            return stmtassign;
        }
    }
}

