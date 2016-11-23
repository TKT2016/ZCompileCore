using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Symbols;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps.Eds;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Exps
{
    public class InvokeSimplestExp : AnalyedExp
    {
        Token ProcToken;
        public TKTProcDesc ProcDesc { get; private set; }

        public InvokeSimplestExp(Exp srcExp,Token token, TKTProcDesc procdesc)
            : base(srcExp)
        {
            ProcToken = token;
            ProcDesc = procdesc;
        }

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            RetType = ProcDesc.ExMethod.Method.ReturnType;
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            //var symbols = this.AnalyExpContext.Symbols;
            ClassContext classContext = this.AnalyExpContext.ClassContext;
            if (classContext.OutClassField == null)
            {
                if (!ProcDesc.ExMethod.Method.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
            }
            else
            {
                if (!ProcDesc.ExMethod.Method.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    EmitHelper.LoadField(il,classContext.OutClassField.GetField());  //il.Emit(OpCodes.Ldfld);
                }
            }
            
            EmitHelper.CallDynamic(il, ProcDesc.ExMethod);
            base.GenerateConv(context);
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            
        }
    }
}
