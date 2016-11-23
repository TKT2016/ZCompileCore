using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Lex;
using ZCompileCore.Tools;
using ZCompileCore.Analys;
using ZCompileCore.AST.Exps;

namespace ZCompileCore.AST.Stmts
{
    public class BlockStmt:Stmt
    {
        List<Stmt> Body = new List<Stmt>();

        public void Analy(MethodContext methodContext)
        {
            var stmtContext = new AnalyStmtContext(methodContext,"BlockStmt");
            Analy(stmtContext);
        }

        public override void Analy(AnalyStmtContext context)
        {
            base.Analy(context);

            foreach (var stmt in Body)
            {
                stmt.Method = this.Method;
                stmt.Analy(context);
            }
            //处理catch
            List<int> catchIndex = new List<int>();
            for (int i = 0; i < Body.Count; i++)
            {
                if(Body[i] is CatchStmt)
                {
                    catchIndex.Add(i);
                }
            }

            if(catchIndex.Count>0)
            {
                List<Stmt> Body2 = new List<Stmt>();            
                catchIndex.Insert(0, -1);
                for (int i = 0; i < catchIndex.Count-1; i++)
                {
                     Body2.Add(new TryStmt());
                     List<Stmt> subList = ListHelper.SubList<Stmt>(Body,catchIndex[i]+1, catchIndex[i+1]);
                     Body2.AddRange(subList);
                }
                List<Stmt> subList2 = ListHelper.SubList<Stmt>(Body, ListHelper.Last(catchIndex)+1, Body.Count);
                Body2.AddRange(subList2);
                Body = Body2;            
            }
        }

        public override void Generate(EmitStmtContext context)
        {
            foreach (var stmt in Body)
            {
                stmt.Generate(context);
            }
        }

        public void Add(Stmt stmt)
        {
            Body.Add(stmt);
        }

        #region 覆盖
        public override string ToCode()
        {
            StringBuilder buf = new StringBuilder();

            buf.Append(getStmtPrefix());
            buf.AppendLine();
            foreach (var fe in Body)
            {
                buf.Append(fe.ToCode());
                buf.AppendLine();
            }
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get
            {
                return Body[0].Postion;
            }
        }
        #endregion

    }
}
