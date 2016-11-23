using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Reports;

namespace ZCompileCore.AST
{
    public abstract class FileElementAST : ASTree
    {
        public abstract CodePostion Postion { get; }

        public void error(string msg)
        {
            error(this.Postion, /*this.GetType().Name + ":" + */msg);
            TrueAnalyed = false;
        }

        public void errorf(string msgFormat, params string[] msgParams)
        {
            string msg = string.Format(msgFormat,msgParams);
            error(msg);
        }

        public void errorf(CodePostion postion, string msgFormat, params string[] msgParams)
        {
            string msg = string.Format(msgFormat, msgParams);
            error(postion,msg);
        }

        public void warningFormat(string msgFormat, params string[] msgParams)
        {
            string msg = string.Format(msgFormat, msgParams);
            Messager.Warning(this.Postion.Line,this.Postion.Col, /*this.GetType().Name + ":" + */msg);
        }
    }
}
