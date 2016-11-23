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
    [ZClass]
    public class TestTurlet1 : TurtleForm
    {
       
        [Code("运行")]
        public override void RunZLogo()
        {
            //this.Turtle.SetDelay(200);
            //this.Window.Text = "我的第一个ZLogo程序";
            //第一个爬行.第一个爬行_1 @object = new 第一个爬行.第一个爬行_1();
            int x = 0;
            补语控制.执行_次(() => { 旋转绘制三角(x); }, 6);
        }

        [Code("旋转绘制三角")]
        public virtual void 旋转绘制三角(int x)
        {
            this.Turtle.RotateLeft((float) 60);
            this.绘制三角();
            Console.WriteLine(x);
        }

        [Code("绘制三角")]
        public virtual void 绘制三角()
        {
            this.Turtle.RotateLeft((float) 30);
            this.Turtle.Forward((float) 200);
            this.Turtle.RotateLeft((float) 120);
            this.Turtle.Forward((float) 200);
            this.Turtle.RotateLeft((float) 120);
            this.Turtle.Forward((float) 200);
            this.Turtle.RotateRight((float) 30);
            this.Turtle.PenUp();
            this.Turtle.Backward((float) 50);
            this.Turtle.PenDown();
            this.Turtle.Backward((float) 100);
            this.Turtle.PenUp();
            this.Turtle.Forward((float) 150);
            this.Turtle.PenDown();
            this.Turtle.RotateLeft((float) 30);
        }
    }
}
