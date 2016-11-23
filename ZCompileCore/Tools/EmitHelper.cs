using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using ZCompileCore.Reports;
using ZCompileCore.Symbols;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Symbols.Imports;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps;
using ZCompileCore.Analys.EContexts;
using ZLangRT.Descs;

namespace ZCompileCore.Tools
{
    public static class EmitHelper
    {
        public static void TypeOf(ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, typeof(System.Type).GetMethod("GetTypeFromHandle"));
        }

        public static void EmitThis(ILGenerator il, bool isStatic)
        {
            if (!isStatic)
            {
                il.Emit(OpCodes.Ldarg_0); // this
            }
        }

        public static void Inc(ILGenerator il, LocalBuilder local)
        {
            EmitHelper.LoadVar(il, local);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            EmitHelper.StormVar(il, local);
        }

        public static void LoadInt(ILGenerator il, int value)
        {
            if (value == 0)
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
            else if (value == 1)
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }
            else if (value == 2)
            {
                il.Emit(OpCodes.Ldc_I4_2);
            }
            else if (value == 3)
            {
                il.Emit(OpCodes.Ldc_I4_3);
            }
            else if (value == 4)
            {
                il.Emit(OpCodes.Ldc_I4_4);
            }
            else if (value == 5)
            {
                il.Emit(OpCodes.Ldc_I4_5);
            }
            else if (value == 6)
            {
                il.Emit(OpCodes.Ldc_I4_6);
            }
            else if (value == 7)
            {
                il.Emit(OpCodes.Ldc_I4_7);
            }
            else if (value == 8)
            {
                il.Emit(OpCodes.Ldc_I4_8);
            }
            else if (value == -1)
            {
                il.Emit(OpCodes.Ldc_I4_M1);
            }
            else if ( value>=-127 && value <= 128 )
            {
                il.Emit(OpCodes.Ldc_I4_S,value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4,value);
            }
        }

        public static void SetLocalArrayElementValue(ILGenerator il,LocalBuilder arrayLocal,int index,Action emitValue )
        {
            EmitHelper.LoadVar(il, arrayLocal);
            EmitHelper.LoadInt(il, index);
            emitValue();
            il.Emit(OpCodes.Stelem_Ref);
        }

        public static void NewArray(ILGenerator il, int length,Type type/*,LocalBuilder arrayLocal*/)
        {
            LoadInt(il,length);
            il.Emit(OpCodes.Newarr, type);
            //EmitHelper.SaveVar(il, arrayLocal);
        }

        public static void StormArg(ILGenerator il, int argIndex)
        {
            il.Emit(OpCodes.Starg, argIndex);
        }

        public static void StormVar(ILGenerator il, LocalBuilder local)
        {
            il.Emit(OpCodes.Stloc, local);
        }

        public static void LoadVar(ILGenerator il, LocalBuilder local)
        {
            //il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Ldloc, local);
        }

        public static void EmitBool(ILGenerator il, bool b)
        {
            if(b)
                il.Emit(OpCodes.Ldc_I4_1);
            else
                il.Emit(OpCodes.Ldc_I4_0);
        }

        public static void StormField(ILGenerator il, FieldInfo field)
        {
            if (field.IsStatic)
            {
                il.Emit(OpCodes.Stsfld, field);
            }
            else
            {
                il.Emit(OpCodes.Stfld, field);
            }
        }

        public static void LoadField(ILGenerator il, FieldInfo field)
        {
            if(field.IsLiteral)//针对const字段
            {
                object value = field.GetValue(null);
                if(value is int)
                {
                    EmitHelper.LoadInt(il, (int)value);
                }
                else if (value is float)
                {
                    il.Emit(OpCodes.Ldc_R4, (float)value);
                }
                else if (value is string)
                {
                    il.Emit(OpCodes.Ldstr, (string)value);
                }
                else if (value is bool)
                {
                    bool bv = (bool)value;
                    if(bv)
                    {
                        il.Emit(OpCodes.Ldc_I4_1);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4_0);
                    }
                }
                else
                {
                    throw new CompileException("编译器不支持" + field.FieldType.Name+"类型");
                }
            }
            else if (field.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, field);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, field);
            }
        }

        public static void NewObj(ILGenerator il, ConstructorInfo newMethod)
        {
            il.Emit(OpCodes.Newobj, newMethod);
        }

        public static void CallPropertyMethod(ILGenerator il, MethodInfo method)
        {
            il.Emit(OpCodes.Call, method);
        }

        public static void Call(ILGenerator il, MethodInfo method)
        {
            il.Emit(OpCodes.Call, method);
        }

        public static void CallDynamic(ILGenerator il, MethodInfo method,bool isSelf)
        {
            if (method.IsStatic)
            {
                il.Emit(OpCodes.Call, method);
            }
            else if (isSelf)
            {
                il.Emit(OpCodes.Callvirt, method); //il.Emit(OpCodes.Call, method);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, method);
            }
        }

        public static void CallDynamic(ILGenerator il, ExMethodInfo exMethod)
        {
            CallDynamic(il, exMethod.Method, exMethod.IsSelf);
        }

        //public static void CallDynamic(ILGenerator il, MethodInfo method)
        //{
            //il.Emit(OpCodes.Callvirt, method);
            /*if (method.IsAbstract || method.IsVirtual)
            {
                il.Emit(OpCodes.Callvirt, method);
            }
            else
            {
                il.Emit(OpCodes.Call, method);
            }*/
           /* Console.WriteLine(string.Format("{0} IsStatic:{1}", method.Name, method.IsStatic));
            if (method.IsStatic)
            {
                il.Emit(OpCodes.Call, method);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, method);
            }
        }*/
        /*
        public static void CallDynamic(ILGenerator il, MethodInfo method,Type objType)
        {
            if (objType == method.DeclaringType)
                il.Emit(OpCodes.Call, method);
            else
                il.Emit(OpCodes.Callvirt, method);
        }*/
      
        public static bool EmitSymbolGet(ILGenerator il ,SymbolInfo symbol)
        {
            if (symbol is SymbolVar)
            {
                var symbolVar=symbol as SymbolVar;
                EmitHelper.LoadVar(il , symbolVar.VarBuilder);
            }
            else if (symbol is SymbolArg)
            {
                SymbolArg argsymbol = (symbol as SymbolArg);
                il.Emit(OpCodes.Ldarg, argsymbol.ArgIndex);
            }
            else if (symbol is SymbolEnumItem)
            {
                SymbolEnumItem eisymbol = (symbol as SymbolEnumItem);
                EmitHelper.LoadInt(il, (int)eisymbol.EnumValue);
            }
            else if (symbol is SymbolPropertyDirect)
            {
                SymbolPropertyDirect prsymbol = symbol as SymbolPropertyDirect;
                MethodInfo getMethod = prsymbol.ExProperty.Property.GetGetMethod();
                EmitHelper.CallDynamic(il, getMethod, prsymbol.ExProperty.IsSelf);
            }
            else if (symbol is SymbolFieldDirect)
            {
                SymbolFieldDirect fymbol = symbol as SymbolFieldDirect;
                EmitHelper.LoadField(il, fymbol.ExField.Field);
            }
            else if (symbol is SymbolDefProperty)
            {
                SymbolDefProperty prsymbol = (symbol as SymbolDefProperty);
                if (!prsymbol.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
                MethodInfo getMethod = prsymbol.GetProperty().GetGetMethod();
                EmitHelper.CallDynamic(il, getMethod,true);
            }
            else if (symbol is SymbolDefField)
            {
                SymbolDefField prsymbol = (symbol as SymbolDefField);
                if (!prsymbol.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
                //il.Emit(OpCodes.Ldfld, prsymbol.Builder);
                EmitHelper.LoadField(il, prsymbol.GetField());
            }
            else if (symbol is SymbolDefClass)
            {
                return true;
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool EmitSet_Load(ILGenerator il, SymbolInfo symbol)
        {
            if (symbol is SymbolDefProperty)
            {
                SymbolDefProperty prsymbol = (symbol as SymbolDefProperty);
                if (!prsymbol.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
            }
            else if (symbol is SymbolDefField)
            {
                SymbolDefField prsymbol = (symbol as SymbolDefField);
                if (!prsymbol.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool EmitSet_Storm(ILGenerator il, SymbolInfo symbol)
        {
            if (symbol is SymbolVar)
            {
                EmitHelper.StormVar(il, (symbol as SymbolVar).VarBuilder);
            }
            else if (symbol is SymbolArg)
            {
                EmitHelper.StormArg(il, (symbol as SymbolArg).ArgIndex);
            }
            else if (symbol is SymbolPropertyDirect)
            {
                SymbolPropertyDirect prsymbol = symbol as SymbolPropertyDirect;
                MethodInfo setMethod = prsymbol.ExProperty.Property.GetSetMethod();
                EmitHelper.CallDynamic(il, setMethod, prsymbol.ExProperty.IsSelf);
            }
            else if (symbol is SymbolDefProperty)
            {
                SymbolDefProperty prsymbol = (symbol as SymbolDefProperty);
                MethodInfo setMethod = prsymbol.GetProperty().GetSetMethod();
                EmitHelper.CallDynamic(il, setMethod,true);
            }
            else if (symbol is SymbolDefField)
            {
                SymbolDefField prsymbol = (symbol as SymbolDefField);
                EmitHelper.StormField(il,prsymbol.GetField());
            }
            else if (symbol is SymbolFieldDirect)
            {
                SymbolFieldDirect fymbol = symbol as SymbolFieldDirect;
                EmitHelper.StormField(il, fymbol.ExField.Field);
            }
            else
            {
                return false;
            }
            return true;
        }
        /*
        public static void Box(ILGenerator il, Type type, Type callArgType)
        {
        //    if (type == typeof(bool) || type == typeof(byte) || type == typeof(char)
        //               || type == typeof(int) || type == typeof(long)
        //               || type == typeof(float) || type == typeof(double)
        //               || type == typeof(decimal)
        //               || type.IsValueType
        //        || type.IsEnum
        //               )
            if (callArgType == typeof(object))
            {
                if (type.IsValueType)
                {
                    il.Emit(OpCodes.Box, type);
                }
            }
        }*/

        public static void EmitConv(ILGenerator il ,Type targetType,Type curType)
        {
            if (targetType == null) return;
            //ILGenerator il = context.ILout;
            if (targetType == typeof(object) && curType.IsValueType)
            {
                il.Emit(OpCodes.Box, curType);
            }
            if (targetType == typeof(float) && curType == typeof(int))
            {
                il.Emit(OpCodes.Conv_R4);
            }
        }
        /*
        public static void EmitParam(ILGenerator il,EmitExpContext context, Exp exp, Type paramType)
        {
            exp.Generate(context);
            EmitHelper.Box(il, exp.RetType, paramType);
        }*/
        
    }
}
