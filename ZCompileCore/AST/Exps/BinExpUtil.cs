using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZCompileCore.Lex;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Exps
{
    static class BinExpUtil
    {
        public enum CalculaterMethodTypeEnum
        {
           None,ContactString, MathOp,MathDiv, MathCompare, RefCompare, Logic
        }

        public static CalculaterMethodTypeEnum GetCalculaterMethodType(TokenKind opKind, Type ltype, Type rtype)
        {
            //Type calcType = typeof(Calculater);
            if (ltype == typeof(string) || rtype == typeof(string))
            {
                if (opKind == TokenKind.ADD)
                {
                    return CalculaterMethodTypeEnum.ContactString;
                }
            }
            if (opKind == TokenKind.ADD || opKind == TokenKind.SUB || opKind == TokenKind.MUL)
            {
                return CalculaterMethodTypeEnum.MathOp;
            }
            if (opKind == TokenKind.DIV)
            {
                return CalculaterMethodTypeEnum.MathDiv;
            }
            if (opKind == TokenKind.GT || opKind == TokenKind.GE || opKind == TokenKind.LT || opKind == TokenKind.LE)
            {
                return CalculaterMethodTypeEnum.MathCompare;
            }
            if (opKind == TokenKind.AND || opKind == TokenKind.OR)
            {
                return CalculaterMethodTypeEnum.Logic;
            }
            if (opKind == TokenKind.EQ || opKind == TokenKind.NE)
            {
                if (Calculater.IsNumber(ltype) && Calculater.IsNumber(rtype))
                {
                    return CalculaterMethodTypeEnum.MathCompare;
                }
                else
                {
                    return CalculaterMethodTypeEnum.RefCompare;
                }
            }
            return CalculaterMethodTypeEnum.None;
        }

        public static Type GetRetType(CalculaterMethodTypeEnum calculaterMethodType, Type ltype, Type rtype)
        {
            if (calculaterMethodType == CalculaterMethodTypeEnum.None) return null;
            if (calculaterMethodType == CalculaterMethodTypeEnum.ContactString) return typeof(string);
            if (calculaterMethodType == CalculaterMethodTypeEnum.MathOp)
            {
                if (ltype == typeof(int) || rtype == typeof(int)) return typeof(int);
                else return typeof(float);
            }
            if (calculaterMethodType == CalculaterMethodTypeEnum.MathDiv)
            {
                return typeof(float);
            }
            if (calculaterMethodType == CalculaterMethodTypeEnum.MathCompare) return typeof(bool);
            if (calculaterMethodType == CalculaterMethodTypeEnum.RefCompare) return typeof(bool);
            if (calculaterMethodType == CalculaterMethodTypeEnum.Logic) return typeof(bool);
            return null;
        }

        public static MethodInfo GetCalcMethod(TokenKind opKind,  Type ltype, Type rtype)
        {
            CalculaterMethodTypeEnum calculaterMethodType = GetCalculaterMethodType(opKind, ltype, rtype);
            if (calculaterMethodType == CalculaterMethodTypeEnum.None) return null;
            return GetCalcMethod(calculaterMethodType, opKind, ltype, rtype);
        }

        private static MethodInfo GetCalcMethod(CalculaterMethodTypeEnum calculaterMethodType, TokenKind opKind, Type ltype, Type rtype)
        {
            Type calcType = typeof(Calculater);
            
            if (calculaterMethodType == CalculaterMethodTypeEnum.ContactString) return calcType.GetMethod("AddString");
            string opName = GetOpName(opKind);
            if (opName == null) return null;
            string typeName = GetTypeName(ltype, rtype);
            if (typeName == null) return null;
            string methodName = opName + typeName;

            if (calculaterMethodType == CalculaterMethodTypeEnum.MathOp) return calcType.GetMethod(methodName);
            if (calculaterMethodType == CalculaterMethodTypeEnum.MathDiv) return calcType.GetMethod(methodName);
            if (calculaterMethodType == CalculaterMethodTypeEnum.MathCompare) return calcType.GetMethod(methodName);
            if (calculaterMethodType == CalculaterMethodTypeEnum.RefCompare) return calcType.GetMethod(methodName);
            if (calculaterMethodType == CalculaterMethodTypeEnum.Logic) return calcType.GetMethod(methodName);
            return null;
        }

        public static string GetOpName(TokenKind opKind)
        {
            if (opKind == TokenKind.ADD) return "Add";
            if (opKind == TokenKind.SUB) return "Sub";
            if (opKind == TokenKind.MUL) return "Mul";
            if (opKind == TokenKind.DIV) return "Div";
            if (opKind == TokenKind.GT) return "GT";
            if (opKind == TokenKind.GE) return "GE";
            if (opKind == TokenKind.EQ) return "EQ";
            if (opKind == TokenKind.NE) return "NE";
            if (opKind == TokenKind.LT) return "LT";
            if (opKind == TokenKind.LE) return "LE";
            if (opKind == TokenKind.AND) return "AND";
            if (opKind == TokenKind.OR) return "OR";
            return null;
        }

        public static string GetTypeName(Type ltype, Type rtype)
        {
            if (ltype == typeof(bool) && rtype == typeof(bool)) return "Bool";
            if (ltype == typeof(int) && rtype == typeof(int)) return "Int";
            if (Calculater.IsNumber(ltype) && Calculater.IsNumber(rtype)) return "Float";
            return "Ref";
        }
    }
}
