using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Loads;
using ZCompileCore.Symbols;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;
using Z语言系统;

namespace ZCompileCore.AST.Exps
{
    public class NewExp : AnalyedCallExp
    {
        public NewExp(Exp srcExp)
            : base(srcExp)
        {

        }
        public IGcl SubjectGCL { get; set; }
        public BracketExp BrackestArgs { get; set; }

        /// <summary>
        /// 构造类型:(1:列表,2:构造函数,3:函数)
        /// </summary>
        int newCode = 0;

        TKTProcDesc ProcDesc;
        TKTConstructorDesc ConstructorDesc;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var classContext = context.StmtContext.MethodContext.ClassContext;
            var symbols = this.AnalyExpContext.Symbols;
            BrackestArgs = BrackestArgs.Analy(context) as BracketExp;
            if (!BrackestArgs.TrueAnalyed)
            {
                return null;
            }
            Type subjectType = SubjectGCL.ForType;
            if (subjectType.FullName.StartsWith(CompileConstant.LangPackageName+".列表`1["))
            {
                newCode = 1;
                ConstructorDesc = new TKTConstructorDesc();
                var Constructor = subjectType.GetConstructor(new Type[]{});
                if (Constructor == null)
                {
                    error(BrackestArgs.Postion, "没有正确的创建过程");
                    return null;
                }
                else
                {
                    RetType = subjectType;
                    ConstructorDesc.Constructor = Constructor;
                    Type[] genericTypes = GenericUtil.GetInstanceGenriceType(subjectType, typeof(列表<>));
                    Type ElementType = genericTypes[0];

                    var args = BrackestArgs.GetDimArgs();
                    for (int i = 0; i < args.Count; i++)
                    {
                        Exp arg = args[i].Value as Exp;
                       if(arg.RetType!=ElementType&&!ReflectionUtil.IsExtends(arg.RetType,ElementType))
                       {
                           errorf(arg.Postion,"类型不是列表的子类型，不能添加");
                       }
                    }
                }
            }
            else
            {
                var args = BrackestArgs.GetDimArgs();
                ProcDesc = new TKTProcDesc();
                ProcDesc.Add(SubjectGCL.ShowName);
                ProcDesc.Add(args);
                TKTProcDesc newProcDesc = searchNewProc(classContext, ProcDesc);
                if (newProcDesc != null)
                {
                    newProcDesc.AdjustBracket(ProcDesc);// ProcDesc.AdjustBracket(newProcDesc);
                    ProcDesc.ExMethod = newProcDesc.ExMethod;
                    newCode = 3;
                    RetType = ProcDesc.ExMethod.Method.ReturnType;
                }
                else
                {
                    ConstructorDesc = new TKTConstructorDesc(args);
                    TKTConstructorDesc realDesc = SubjectGCL.SearchConstructor(ConstructorDesc);
                    if (realDesc == null)
                    {
                        error(BrackestArgs.Postion, "没有正确的创建过程");
                        return null;
                    }
                    else
                    {
                        RetType = SubjectGCL.ForType;
                        realDesc.AdjustBracket(ConstructorDesc);
                        ConstructorDesc.Constructor = realDesc.Constructor;
                        newCode = 2; 
                    }
                }
            }
            return this;
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

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            BrackestArgs.GetNestedFields(nestedField);
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            if (newCode == 1)
            {
                var Constructor = ConstructorDesc.Constructor;
                LocalBuilder varLocal = il.DeclareLocal(SubjectGCL.ForType);
                EmitHelper.NewObj(il, Constructor);
                il.Emit(OpCodes.Stloc, varLocal);

                MethodInfo addMethod = SubjectGCL.ForType.GetMethod("Add");
                ExMethodInfo exAddMethodInfo = GclUtil.CreatExMethodInfo(addMethod, SubjectGCL.ForType);
                if (BrackestArgs != null)
                {
                    foreach (var exp in BrackestArgs.InneExps)
                    {
                        EmitHelper.LoadVar(il, varLocal);//il.Emit(OpCodes.Ldloc, varLocal);
                        (exp).Generate(context);
                        EmitHelper.CallDynamic(il, exAddMethodInfo);
                    }
                }
                EmitHelper.LoadVar(il, varLocal);//il.Emit(OpCodes.Ldloc, varLocal);
            }
            else if (newCode == 2)
            {
                var Constructor = ConstructorDesc.Constructor;
                if (!IsAssignedValue)
                {
                    LocalBuilder varLocal = il.DeclareLocal(SubjectGCL.ForType);
                    emitConstructor(context);//emitArgs(context);
                    EmitHelper.NewObj(il, Constructor);
                    il.Emit(OpCodes.Stloc, varLocal);
                    EmitHelper.LoadVar(il, varLocal);//il.Emit(OpCodes.Ldloc, varLocal);
                }
                else
                {
                    emitConstructor(context);//emitArgs(context);
                    EmitHelper.NewObj(il, Constructor);
                }
            }
            else
            {
                var Method = ProcDesc.ExMethod;
                emitProc(context);
                EmitHelper.CallDynamic(il, Method);
            }
            base.GenerateConv(context);
        }

        void emitConstructor(EmitExpContext context)
        {
            emitArgs(context, ConstructorDesc.Bracket.ListArgs, ConstructorDesc.Constructor.GetParameters());
        }

        void emitProc(EmitExpContext context)
        {
            emitArgs(context, ProcDesc.ArgsList, ProcDesc.ExMethod.Method.GetParameters());
        }

        void emitArgs(EmitExpContext context,List<TKTProcArg> args,ParameterInfo[] parameterInfos)
        {
            if (args == null) return;
            List<Exp> argExps = new List<Exp>();
            foreach (var procArg in args)
            {
                Exp arg = procArg.Value as Exp;
                argExps.Add(arg);
            }
            base.GenerateArgsExp(context, parameterInfos, argExps.ToArray());
            /*ILGenerator il = context.ILout;
            for (int i = 0; i < args.Count; i++)
            {
                Exp arg = args[i].Value as Exp;
                EmitHelper.EmitParam(il, context, arg, parameterInfos[i].ParameterType);
            }*/
        }

        public override string ToCode()
        {
            return SubjectGCL.ShowName + BrackestArgs.ToCode();
        }
    }
}
