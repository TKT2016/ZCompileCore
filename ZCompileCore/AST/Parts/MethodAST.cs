using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.AST.Parts.Fns;
using ZCompileCore.AST.Stmts;
using ZCompileCore.AST.Types;
using ZCompileCore.Lex;
using ZCompileCore.Symbols.Defs;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Attributes;

namespace ZCompileCore.AST.Parts
{
    public class MethodAST : PartAST
    {
        public Token KeyToken;
        public FnName FnName {get;set;}
        public BlockStmt FnBody { get; set; }

        public MethodContext MethodContext { get; private set; }
        public ClassAST ClassAST { get; private set; }

        SymbolVar retResult;
        string ClassName;

        public bool CompileName(ClassAST classAst,int index,bool isStatic)
        {
            this.ClassAST = classAst;
            this.ClassContext = classAst.ClassContext;
            this.ClassName = classAst.ClassName;
            MethodContext = new MethodContext(this.ClassContext, FnName.ToCode());
            MethodContext.MethodIndex = index;
            bool b= FnName.Analy(this, isStatic);
            if (b)
            {
                GenerateName(isStatic);
            }
            else
            {
                return false;
            }
            return true;
        }

        MethodAttributes getMethodAttributes(bool isStatic)
        {
            if (isStatic)
            {
                return MethodAttributes.Public | MethodAttributes.Static;
            }
            else
            {
                if (FnName.IsConstructor(this.ClassName)!=-1)
                {
                    return MethodAttributes.Public;
                }
                else
                {
                    return MethodAttributes.Public | MethodAttributes.Virtual;
                }
            }
        }

        string getMethodName(bool isStatic)
        {
            if (!isStatic)
            {
                if (this.ClassContext.ClassSymbol.BaseGcl != null)
                {
                    TKTProcDesc desc = this.ClassContext.ClassSymbol.BaseGcl.SearchProc(this.FnName.ProcDesc);
                    if (desc != null)
                    {
                        return desc.ExMethod.Method.Name;
                    }
                }
            }
            return FnName.CreateMethodName();
        }
        string MethodName;
        public void GenerateName(bool isStatic)
        {
            MethodAttributes attr = getMethodAttributes(isStatic);
            if (FnName.IsConstructor(this.ClassName)!=-1)
            {
                List<Type> argTypes = FnName.ProcDesc.GetArgTypes(1);
                var ClassSymbol = MethodContext.ClassContext.ClassSymbol;
                ConstructorBuilder constructorBuilder = ClassSymbol.ClassBuilder.DefineConstructor(attr, CallingConventions.Standard, argTypes.ToArray());
                MethodContext.EmitContext.SetBuilder(constructorBuilder);
                MethodContext.EmitContext.ClassContext = this.ClassContext.EmitContext;
                if(FnName.IsMinConstructor())
                {
                    MethodContext.ClassContext.ZeroConstructor = constructorBuilder;
                }
                else
                {
                    List<TKTProcArg> normalArgs = this.MethodContext.ProcDesc.GetSpecialArgs(1);
                    int start_i = isStatic ? 0 : 1;
                    for (var i = 0; i < normalArgs.Count; i++)
                    {
                        constructorBuilder.DefineParameter(i + start_i, ParameterAttributes.None, normalArgs[i].ArgName);
                    }
                }
                MethodContext.ClassContext.ConstructorBuilderList.Add(constructorBuilder);
            }
            else
            {
                List<Type> argTypes = FnName.ProcDesc.GetArgTypes(1);
                var ClassSymbol = MethodContext.ClassContext.ClassSymbol;
                MethodName = getMethodName(isStatic);
                MethodBuilder methodBuilder = ClassSymbol.ClassBuilder.DefineMethod(MethodName, attr, MethodContext.RetType, argTypes.ToArray());
                if(MethodName=="启动")
                {
                    Type myType = typeof(STAThreadAttribute);
                    ConstructorInfo infoConstructor = myType.GetConstructor(new Type[] { });
                    CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(infoConstructor, new object[] { });
                    methodBuilder.SetCustomAttribute(attributeBuilder);
                }
                else
                {
                    setAttrMappingCode(methodBuilder);
                }
                MethodContext.EmitContext.SetBuilder(methodBuilder);
                MethodContext.EmitContext.ClassContext = this.ClassContext.EmitContext;
                List<TKTProcArg> genericArgs = this.MethodContext.ProcDesc.GetSpecialArgs(2);
                if (genericArgs.Count > 0)
                {
                    string[] names = genericArgs.Select(p => p.ArgName).ToArray();
                    methodBuilder.DefineGenericParameters(names);
                }
                          
                List<TKTProcArg> normalArgs = this.MethodContext.ProcDesc.GetSpecialArgs(1);
                int start_i = isStatic ? 0 : 1;
                for (var i = 0; i < normalArgs.Count; i++)
                {
                    methodBuilder.DefineParameter(i + start_i, ParameterAttributes.None, normalArgs[i].ArgName);
                }
            }
        }

        void setAttrMappingCode(MethodBuilder methodBuilder)
        {
            Type myType = typeof(CodeAttribute);
            ConstructorInfo infoConstructor = myType.GetConstructor(new Type[] { typeof(string) });
            string code = this.FnName.ToProcDescCode();
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(infoConstructor, new object[] { code });
            methodBuilder.SetCustomAttribute(attributeBuilder);
        }
        
        public void AnalyBody()
        {
            var symbols = MethodContext.Symbols;
            if (MethodContext.RetType != typeof(void))
            {
                retResult = new SymbolVar("结果", MethodContext.RetType);
                retResult.IsAssigned = true;
                retResult.LoacalVarIndex = this.MethodContext.CreateLocalVarIndex("结果");
                symbols.AddSafe(retResult);
            }
            FnBody.Method = this;
            FnBody.Analy(MethodContext);
        }

        void callBaseDefault()
        {
            var il = MethodContext.EmitContext.ILout;
            ConstructorInfo zeroCons = this.MethodContext.ClassContext.ClassSymbol.BaseGcl.ForType.GetConstructor(new Type[]{});
            if(zeroCons!=null)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Call, zeroCons);
            }
        }

        public void Generate(TypeBuilder classBuilder, bool isStatic)
        {
            var symbols = MethodContext.Symbols;
            EmitStmtContext stmtContext = new EmitStmtContext(MethodContext.EmitContext);
            var il = MethodContext.EmitContext.ILout;
            for (int i = 0; i < MethodContext.LoacalVarList.Count; i++)
            {
                string ident = MethodContext.LoacalVarList[i];
                SymbolVar varSymbol = symbols.Get(ident) as SymbolVar;
                varSymbol.VarBuilder = il.DeclareLocal(varSymbol.DimType);
                varSymbol.VarBuilder.SetLocalSymInfo(varSymbol.SymbolName);
            }
            if (MethodContext.RetType != typeof(void))
            {
                if (MethodContext.RetType.IsValueType)
                {
                    EmitHelper.LoadInt(il, 0);
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);         
                }
                EmitHelper.StormVar(il, (symbols.Get(MethodContext.LoacalVarList[0]) as SymbolVar).VarBuilder);
            }

            if (FnName.IsConstructor(this.ClassName)!=-1)
            {
                if (!isStatic)
                {
                    callBaseDefault();
                    if (ClassContext.InitMemberValueMethod != null)
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod, true);//EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod);
                    }
                }
                else
                {
                    if (ClassContext.InitMemberValueMethod != null)
                    {
                        EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod, true);//EmitHelper.CallDynamic(il, ClassContext.InitMemberValueMethod);
                    }
                }
            }
            //var MethodName = getMethodName(isStatic);
            //if(MethodName=="启动")
            //{
            //    //EmitHelper.CallDynamic(il, typeof(DebugHelper).GetMethod("PrintBaseDirectory"));
            //}
            FnBody.Generate(stmtContext);
           
            if (MethodContext.RetType != typeof(void))
            {
                il.Emit(OpCodes.Ldloc, retResult.VarBuilder);
                EmitHelper.EmitConv(il, MethodContext.RetType, retResult.DimType);
            }
            il.Emit(OpCodes.Ret);
        }

        #region 位置格式化
        public override string ToCode()
        {
            FnBody.Deep = 3;
            StringBuilder buf = new StringBuilder();
            buf.AppendLine();
            buf.Append("过程:");
            buf.Append(FnName.ToCode());
            buf.AppendLine(FnBody.ToCode());
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get { return KeyToken.Postion; }
        }
        #endregion
    }


}
