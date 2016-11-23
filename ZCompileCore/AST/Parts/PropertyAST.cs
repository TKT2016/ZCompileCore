using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Exps;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analyers;
using ZCompileCore.Analys;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.AST.Parts
{
    public class PropertyAST : PartAST
    {
        public Token TypeToken;
        public Token NameToken;
        public Exp ValueExp;
        public bool IsStatic { get; set; }
        public SymbolDefProperty PropertySymbol { get; set; }

        string TypeName;
        string PropertyName;
        Type PropertyType;
        IGcl PropertyGcl;

        public PropertyAST()
        {
            //IsStatic = true;
        }
        /// <summary>
        /// 构造类型:(2:构造函数,3:函数,4:值类型)
        /// </summary>
        int newCode = 0;

        public void CompileName(ClassContext classContext,bool isStatic)
        {
            if (TypeToken==null ||NameToken==null)
            {
                TrueAnalyed = false;
                return;
            }
            IsStatic = isStatic;
            this.ClassContext = classContext;
            var symbols = ClassContext.Symbols;

            TypeName = TypeToken.GetText();
            PropertyName = NameToken.GetText();
            PropertyGcl = classContext.SearchType(TypeName);

            if (PropertyGcl != null)
            {
                PropertyType = PropertyGcl.ForType;
            }
            else
            {
                errorf("没有找到类型'{0}'",TypeName);
            }

            if (ClassContext.ClassSymbol.PropertyDict.ContainsKey(PropertyName))
            {
                error("属性'" + PropertyName + "'已经存在");
                return;
            }

            if(PropertyType==null)
            {
                error("属性类型'" + TypeName + "'不存在");
                return;
            }
            PropertySymbol = new SymbolDefProperty(PropertyName, PropertyType, isStatic);
            symbols.AddSafe(PropertySymbol);
            ClassContext.ClassSymbol.PropertyDict.Add(PropertyName, PropertySymbol);

            var classBuilder = ClassContext.EmitContext.CurrentTypeBuilder;
            GenerateName(classBuilder, IsStatic);

            AnalyBody(classContext);
        }

        TKTProcDesc ProcDesc;
        TKTConstructorDesc ConstructorDesc;

        public void AnalyBody(ClassContext classContext)
        {
            if (ValueExp != null)
            {
                MethodContext methodContext = new MethodContext(ClassContext, PropertyName);
                var symbols = ClassContext.Symbols;

                AnalyStmtContext stmtContext = new AnalyStmtContext(methodContext, PropertyName);
                AnalyExpContext expContext = new AnalyExpContext(stmtContext);
                ValueExp = ValueExp.Analy(expContext);

                if (ValueExp == null)
                {
                    return;
                }
                if (!ReflectionUtil.IsExtends(ValueExp.RetType, PropertyType))
                {
                    error("属性值的类型不正确");
                }
            }
            else
            {
                if (PropertyType.IsValueType)
                {
                    newCode = 4;
                }
                else
                {
                    List<TKTProcArg> args = new List<TKTProcArg>();
                    ProcDesc = new TKTProcDesc();
                    ProcDesc.Add(PropertyGcl.ShowName);
                    ProcDesc.Add(args);
                    TKTProcDesc newProcDesc = searchNewProc(classContext, ProcDesc);
                    if (newProcDesc != null)
                    {
                        newCode = 3;
                    }
                    else
                    {
                        ConstructorDesc = new TKTConstructorDesc(args);
                        TKTConstructorDesc realDesc = PropertyGcl.SearchConstructor(ConstructorDesc);
                        if (realDesc != null)
                        {
                            ConstructorDesc.Constructor = realDesc.Constructor;
                            newCode = 2;
                        }
                    }
                }
            }
        }

        TKTProcDesc searchNewProc(ClassContext classContext, TKTProcDesc expProcDesc)
        {
            var procArray = ClassContextHelper.SearchProc(classContext, expProcDesc);
            if (procArray.Length == 1)
            {
                return procArray[0];
            }
            else if (procArray.Length > 1)
            {
                error("找到多个过程，不能确定是属于哪一个简略使用的类型的过程");
            }
            else if (procArray.Length == 0)
            {
                //error("没有找到对应的过程'" + this.ToCode() + "'");
            }
            return null;
        }

        public void GenerateBody(EmitExpContext expContext)
        {
            var il = expContext.ILout;
            MethodInfo method = PropertySymbol.GetProperty().GetSetMethod();
            if (ValueExp != null)
            {
                EmitHelper.EmitThis(il, IsStatic);
                ValueExp.Generate(expContext);     
                EmitHelper.CallDynamic(il, method,true);
            }
            else
            {
                if (newCode == 2)
                {
                    var Constructor = ConstructorDesc.Constructor;
                    EmitHelper.EmitThis(il, IsStatic);
                    EmitHelper.NewObj(il, Constructor);
                    EmitHelper.CallDynamic(il, method, true);
                }
                else if (newCode == 3)
                {
                    var createMethod = ProcDesc.ExMethod;
                    EmitHelper.EmitThis(il, IsStatic);
                    EmitHelper.CallDynamic(il, createMethod);
                    EmitHelper.CallDynamic(il, createMethod);
                }
                else if (newCode == 4)
                {
                    EmitHelper.EmitThis(il, IsStatic);
                    EmitHelper.LoadInt(il, 0);
                    EmitHelper.CallDynamic(il, method,true);
                }
            }
        }

        MethodAttributes getMethodAttr(bool isStatic)
        {
            if (isStatic)
                return MethodAttributes.Public | MethodAttributes.Static;
            else
                return MethodAttributes.Public;
        }

        FieldAttributes getFieldAttr(bool isStatic)
        {
            if (isStatic)
                return FieldAttributes.Private | FieldAttributes.Static;
            else
                return FieldAttributes.Private;
        }
             
        public void GenerateName(TypeBuilder classBuilder, bool isStatic)
        {
            string propertyName = PropertySymbol.SymbolName;
            Type propertyType = PropertySymbol.PropertyType;
            MethodAttributes methodAttr=getMethodAttr( isStatic);
            FieldAttributes fieldAttr = getFieldAttr(isStatic);

            FieldBuilder field = classBuilder.DefineField("_" + propertyName, propertyType, fieldAttr);
            PropertyBuilder property = classBuilder.DefineProperty(propertyName, PropertyAttributes.None , propertyType, null);

            emitGet(classBuilder, propertyName, propertyType, isStatic, field, property, methodAttr);
            emitSet(classBuilder, propertyName, propertyType, isStatic, field, property, methodAttr);

            PropertySymbol.SetProperty(property);
        }

        void emitGet(TypeBuilder classBuilder, string propertyName, Type propertyType, bool isStatic, FieldBuilder field, PropertyBuilder property, MethodAttributes methodAttr)
        {
            MethodBuilder methodGet = classBuilder.DefineMethod("get_" + propertyName, methodAttr, propertyType, null);
            var ilget = methodGet.GetILGenerator();
            EmitHelper.EmitThis(ilget, isStatic);
            LocalBuilder retBuilder = ilget.DeclareLocal(propertyType);
            EmitHelper.LoadField(ilget, field);
            EmitHelper.StormVar(ilget, retBuilder);
            EmitHelper.LoadVar(ilget, retBuilder);
            ilget.Emit(OpCodes.Ret);
            property.SetGetMethod(methodGet);
        }

        void emitSet(TypeBuilder classBuilder, string propertyName, Type propertyType, bool isStatic, FieldBuilder field, PropertyBuilder property, MethodAttributes methodAttr)
        {
            MethodBuilder methodSet = classBuilder.DefineMethod("set_" + propertyName, methodAttr, typeof(void), new Type[] { propertyType });
            var ilSet = methodSet.GetILGenerator();
            if (isStatic)
            {
                ilSet.Emit(OpCodes.Ldarg_0); // this
                EmitHelper.StormField(ilSet, field);
            }
            else
            {
                ilSet.Emit(OpCodes.Ldarg_0); // this
                ilSet.Emit(OpCodes.Ldarg_1); 
                EmitHelper.StormField(ilSet, field);
            }
            
            ilSet.Emit(OpCodes.Ret);
            property.SetSetMethod(methodSet);
        }

        #region 位置格式化
        public override string ToCode()
        {
            if (IsStatic)
                Deep = 1;
            else
                Deep = 2;
            StringBuilder buf = new StringBuilder();
            buf.Append(getStmtPrefix());
            buf.Append(TypeToken.GetText());
            buf.Append(":");
            buf.Append(NameToken.GetText());
            if(ValueExp!=null)
            {
                buf.Append("=");
                buf.Append(ValueExp.ToCode());
            }
            buf.Append(";");
            buf.AppendLine();
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get { return NameToken.Postion; }
        }

        protected string getStmtPrefix()
        {
            StringBuilder buff = new StringBuilder();
            int temp = Deep;
            while (temp > 0)
            {
                buff.Append("  ");
                temp--;
            }
            return buff.ToString();
        }
        #endregion
    }


}
