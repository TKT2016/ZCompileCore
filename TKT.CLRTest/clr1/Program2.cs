using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKT.CLRTest.Lei;
using TKT.RT;
using TKT系统;

namespace TKT.CLRTest.clr1
{
    class Program2
    {
        static void Main3(string[] args)
        {
            TesClassA t1 = new TesClassA();
            string str = "aaaa" + t1;
            //int a = 100 + 8;
            Console.ReadKey();
        }


        object TEST_ASSING_ARG(object a,string b,int c)
        {
            c = 100;
            return c;
        }

        object TEST_ASSING_ARG_DOT(TesClassA t1,string c)
        {
            t1.Name = "TEST_" + c;
            return t1;
        }

        int testADD(int a,int b)
        {
            return a + b;
        }

        int testSUB(int a, int b)
        {
            return a - b;
        }

        int testMUL(int a, int b)
        {
            return a * b;
        }

        int testDIV(int a, int b)
        {
            return a / b;
        }

        bool testGT(int a, int b)
        {
            return a > b;
        }

        bool testGE(int a, int b)
        {
            return a >= b;
        }

        bool testLT(int a, int b)
        {
            return a < b;
        }

        bool testLE(int a, int b)
        {
            return a <= b;
        }

        bool testEQ(int a, int b)
        {
            return a == b;
        }

        bool testNE(int a, int b)
        {
            return a != b;
        }
        //--------------------------------------------------
        float testADD(float a, int b)
        {
            return a + b;
        }

        float testSUB(float a, int b)
        {
            return a - b;
        }

        float testMUL(float a, int b)
        {
            return a * b;
        }

        float testDIV(float a, int b)
        {
            return a / b;
        }

        bool testGT(float a, int b)
        {
            return a > b;
        }

        bool testGE(float a, int b)
        {
            return a >= b;
        }

        bool testLT(float a, int b)
        {
            return a < b;
        }

        bool testLE(float a, int b)
        {
            return a <= b;
        }

        bool testEQ(float a, int b)
        {
            return a == b;
        }

        bool testNE(float a, int b)
        {
            return a != b;
        }
        //---------------------------------------------------------
        float testADD(float a, float b)
        {
            return a + b;
        }

        float testSUB(float a, float b)
        {
            return a - b;
        }

        float testMUL(float a, float b)
        {
            return a * b;
        }

        float testDIV(float a, float b)
        {
            return a / b;
        }

        bool testGT(float a, float b)
        {
            return a > b;
        }

        bool testGE(float a, float b)
        {
            return a >= b;
        }

        bool testLT(float a, float b)
        {
            return a < b;
        }

        bool testLE(float a, float b)
        {
            return a <= b;
        }

        bool testEQ(float a, float b)
        {
            return a == b;
        }

        bool testNE(float a, float b)
        {
            return a != b;
        }
      //-------------------------------
        bool testAnd(bool a,bool b)
        {
            bool c= a && b;
            string str = "aaaaaaaaaaaaaa";
            return (str == "aaaaa")&&c;
        }

        bool testOr(bool a, bool b)
        {
            return a || b;
        }

        bool testNot(bool a)
        {
            return !a;
        }
        //-------------------------------
        bool testEQ(object a,object b)
        {
            return a == b;
        }

        bool testNE(object a, object b)
        {
            return a != b;
        }

    }
}
