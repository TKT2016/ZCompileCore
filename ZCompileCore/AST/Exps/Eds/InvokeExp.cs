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
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Exps.Eds
{
    public class InvokeExp : AnalyedCallExp
    {
        public InvokeExp(Exp srcExp)
            : base(srcExp)
        {

        }
        public Exp SubjectExp { get; private set; }
        public ExMethodInfo CallExMethod { get; private set; }
        public TKTProcDesc ExpProcDesc {get;private set; }
        Type subjType;
        ClassContext classContext;
        TKTProcDesc searchedProcDesc;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            classContext = context.StmtContext.MethodContext.ClassContext;
            var symbols = this.AnalyExpContext.Symbols;
            if(! analyCallBody())
            {
                return null;
            }
            searchedProcDesc = searchProc(classContext);
            if (searchedProcDesc == null)
            {
                errorf("没有找到过程'{0}'",this.ToCode());
                return null;
            }
            searchedProcDesc.AdjustBracket(ExpProcDesc);
            CallExMethod = searchedProcDesc.ExMethod;
            if (CallExMethod == null) return null;
            RetType = CallExMethod.Method.ReturnType;

            analyGeneric();
            analyArgLanmbda(context);
            if(isSubjectEveryOneExp())
            {
                InvokeEveryoneSubejctExp ieoexp = new InvokeEveryoneSubejctExp(this);
                Exp ioeResultExp = ieoexp.Analy(context);
                return ioeResultExp;
            }
            else if (isObjectEveryOneExp())
            {
                InvokeEveryoneObejctExp ieoexp = new InvokeEveryoneObejctExp(this);
                Exp ioeResultExp = ieoexp.Analy(context);
                return ioeResultExp;
            }
            return this;
        }

        bool isSubjectEveryOneExp()
        {
            return (SubjectExp != null && SubjectExp is EveryOneExp);
        }

        bool isObjectEveryOneExp()
        {
            int argsCount = searchedProcDesc.ArgCount;
            if(argsCount==1)
            {
                TKTProcArg arg = ExpProcDesc.GetArg(0);
                Exp exp = (arg.Value as Exp);
                if (exp is EveryOneExp)
                {
                    return true;
                }
            }
            return false;
        }

        void analyGeneric()
        {
            if (CallExMethod.Method.IsGenericMethod)
            {
                List<Type> types = findGenericTypes();
                CallExMethod = GclUtil.MakeGenericExMethod(CallExMethod, types.ToArray());
            }
        }
      
        bool analyCallBody()
        {
            ExpProcDesc = new TKTProcDesc();
            int i = 0;
           
            for (; i < this.Elements.Count; i++)
            {
                var exp = this.Elements[i];
                if (exp is FTextExp)
                {
                    ExpProcDesc.Add((exp as FTextExp).IdentToken.GetText());
                }
                else if (exp is BracketExp)
                {
                    var bexp = exp as BracketExp;
                    var exp2 = bexp.Analy(this.AnalyExpContext);
                    if(exp2!=null && exp2.TrueAnalyed)
                    {
                        Elements[i] = exp2;
                        ExpProcDesc.Add(bexp.GetDimArgs());
                    }
                    else
                    {
                        return false;
                    }              
                }
            }
            return true;
        }

        void analyArgLanmbda(AnalyExpContext context)
        {
            for (int i = 0; i < ExpProcDesc.ArgCount; i++)
            {
                TKTProcArg procArg = searchedProcDesc.GetArg(i);
                if (procArg.ArgType == TKTLambda.ActionType || procArg.ArgType == TKTLambda.CondtionType)
                {
                    TKTProcArg expArg = ExpProcDesc.GetArg(i);
                    Exp exp = expArg.Value as Exp;
                    NewLambdaExp newLambdaExp = new NewLambdaExp(this, exp, procArg.ArgType);
                    expArg.Value = newLambdaExp;
                    newLambdaExp.Analy(context);
                } 
            }
        }

        List<Type> findGenericTypes()
        {
            List<Type> types = new List<Type>();
            for (int i = 0; i < ExpProcDesc.Count; i++)
            {
                if (searchedProcDesc.GetArg(i).IsGenericArg)
                {
                    TKTProcArg procArg = ExpProcDesc.GetArg(i);
                    Type type = (procArg.Value as Exp).RetType;
                    types.Add(type);
                }
            }
            return types;
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            foreach (var arg in ExpProcDesc.GetSpecialArgs(0))
            {
                (arg.Value as Exp).GetNestedFields(nestedField);
            }
        }

        TKTProcDesc searchProc(ClassContext classContext)
        {
            var procArray = ClassContextHelper.SearchProc( classContext,ExpProcDesc);
            if(procArray.Length==1)
            {
                //procArray[0].AdjustBracket(procDesc);
                return procArray[0];
            }
            else if (procArray.Length > 1)
            {
                error("找到多个过程，不能确定是属于哪一个简略使用的类型的过程");
            }
            else if (procArray.Length == 0)
            {
                if (Elements[0] is BracketExp)
                {
                    SubjectExp = Elements[0];
                    subjType = SubjectExp.RetType;
                    ExpProcDesc = ExpProcDesc.CreateTail();
                    procArray = ClassContextHelper.SearchProc(classContext, subjType, ExpProcDesc);

                    if (procArray.Length == 1)
                    {
                        return procArray[0];
                    }
                    else if (procArray.Length == 0)
                    {
                        error("没有找到对应的过程'" + this.ToCode() + "'");
                    }
                    else if (procArray.Length > 1)
                    {
                        error("找到多个过程，不能确定是属于哪一个简略使用的类型的过程:'" + this.ToCode() + "'");
                    }
                }
            }
            return null;
        }
        
        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            GenerateSubject(context);
            base.GenerateArgsExp(context, ExpProcDesc, searchedProcDesc); //GenerateArgs(context);
            EmitHelper.CallDynamic(il,CallExMethod);
            base.GenerateConv(context);
        }

        public void GenerateFirstArgs(EmitExpContext context)
        {
            base.GenerateArgsExp(context, ExpProcDesc, searchedProcDesc,0);
        }


        public void GenerateArgs(EmitExpContext context)
        {
            base.GenerateArgsExp(context, ExpProcDesc, searchedProcDesc);
            /*
            ILGenerator il = context.ILout;
            for (int i = 0; i < ExpProcDesc.ArgCount; i++)
            {
                var arg = ExpProcDesc.GetArg(i);
                var exp = (arg.Value as Exp);
                TKTProcArg procArg = searchedProcDesc.GetArg(i);
                //Type callArgType = procArg.ArgType;
                exp.RequireType = procArg.ArgType;
                exp.Generate(context);
                //GenerateArgILBox(context,i);
            }*/
        }

/*
                public void GenerateArgILBox(EmitExpContext context,int i)
                {
                    ILGenerator il = context.ILout;
                    var arg = ExpProcDesc.GetArg(i);
                    var exp = (arg.Value as Exp);
                    var rettype = exp.RetType;
                    TKTProcArg procArg = searchedProcDesc.GetArg(i);
                    Type callArgType = procArg.ArgType;
                    EmitHelper.Box(il, rettype, callArgType);
                }*/

        public void GenerateSubject(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            if (SubjectExp != null)
                SubjectExp.Generate(context);
            else
            {
                if (!CallExMethod.Method.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
            }
        }
    }
}
