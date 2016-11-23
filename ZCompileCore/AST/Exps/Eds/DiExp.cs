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
using ZCompileCore.Lex;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Exps
{
    public class DiExp : AnalyedCallExp , IGenerateSet
    {
        public DiExp(Exp srcExp)
            : base(srcExp)
        {

        }
        public Exp SubjectExp { get; set; }
        public Exp ArgExp { get; set; }
        public ExPropertyInfo ExProperty;

        public override Exp Analy(AnalyExpContext context)
        {
            base.Analy(context);
            var symbols = this.AnalyExpContext.Symbols;
            SubjectExp = SubjectExp.Analy(context);// AnalyExp(SubjectExp);
            ArgExp = ArgExp.Analy(context);//AnalyExp(ArgExp);
            var propertyName = "Item";
            var subjType = SubjectExp.RetType;
            ExProperty = GclUtil.SearchExProperty(propertyName, subjType); //subjType.GetExProperty(propertyName);
            
            if (ExProperty == null)
            {
                error(SubjectExp.Postion, "不存在索引");
                return null;
            }
            else
            {
                RetType = ExProperty.Property.PropertyType;
            }
            return this;
        }

        public override void Generate(EmitExpContext context)
        {
            GenerateGet(context);
            base.GenerateConv(context);
        }

        public void GenerateGet(EmitExpContext context)
        {
            ILGenerator il = context.ILout;
            MethodInfo getMethod = ExProperty.Property.GetGetMethod();
            SubjectExp.Generate(context);
            ArgExp.RequireType = getMethod.ReturnType;
            ArgExp.Generate(context);
            EmitHelper.CallDynamic(il, getMethod, ExProperty.IsSelf);
        }

        public void GenerateSet(EmitExpContext context,Exp valueExpr)
        {
            ILGenerator il = context.ILout;
            MethodInfo setMethod = ExProperty.Property.GetSetMethod();
            SubjectExp.Generate(context);
            ArgExp.RequireType = setMethod.GetParameters()[0].ParameterType;
            ArgExp.Generate(context);
            //EmitHelper.Box(il, ArgExp.RetType, setMethod.GetParameters()[0].ParameterType);
            valueExpr.RequireType = setMethod.GetParameters()[1].ParameterType;
            valueExpr.Generate(context);
            EmitHelper.CallDynamic(il, setMethod, ExProperty.IsSelf);
        }

        public override void GetNestedFields(Dictionary<string, VarExp> nestedField)
        {
            SubjectExp.GetNestedFields(nestedField);
        }

        public bool CanWrite
        {
            get
            {
                return ExProperty.Property.CanWrite;
            }
        }
    }
}
