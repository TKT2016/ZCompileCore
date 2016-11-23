using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Attributes;
using ZLogoEngine;
using Z语言系统;

namespace TKT.CLRTest.S1
{
    namespace 测试ZLOGO.zlogo
    {
        [ZClass]
        public class 测试ZLOGO : TurtleForm
        {
            private class 测试ZLOGO_1
            {
                public 测试ZLOGO _____Nested_This;

                public void __CALL()
                {
                    this._____Nested_This.旋转绘制三角();
                }
            }

            [Code("运行ZLOGO")]
            public override void RunZLogo()
            {
                补语控制.执行_次(new Action(new 测试ZLOGO.测试ZLOGO_1
                {
                    _____Nested_This = this
                }.__CALL), 6);
            }

            [Code("旋转绘制三角")]
            public virtual void 旋转绘制三角()
            {
                this.Turtle.Forward((float) 200);
            }

            [Code("绘制三角")]
            public virtual void 绘制三角()
            {
            }

        }
    }
}
