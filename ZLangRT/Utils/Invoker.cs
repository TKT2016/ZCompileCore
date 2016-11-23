using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ZLangRT
{
    public static class Invoker
    {
        private static void debug(string message)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Debug:" + message);
            Console.ForegroundColor = temp;
        }
        /*
        public static void SetIndex(object instance,  object index ,object value )
        {
            Type type = instance.GetType();
            var method = type.GetMethod("set_Item");
            if (method == null)
            {
                throw new TKTRTException("类型" + type.FullName + "没有索引器,无法对实例赋值");
            }

            if (index is int || index is double)
            {
                index = Calculater.Obj2Int(index);
            }

            object newValue = Calculater.ConvertObjectType(value);
            method.Invoke(instance, new object[] { index, newValue });
        }

        public static object GetIndex(object instance, object index)
        {
            Type type = instance.GetType();
            var method = type.GetMethod("get_Item");
            if (method == null)
            {
                throw new TKTRTException("类型" + type.FullName + "没有索引器,无法对实例赋值");
            }
            if (index is int || index is double)
            {
                index = Calculater.Obj2Int(index);
            }
            return  method.Invoke(instance, new object[] { index });
        }
        */
        
        public static object Call(object obj, string funcName, params object[] args)
        {
            //debug("--- ------  Call "+ funcName);
            //List<object> argList = new List<object>();
            //foreach (object arg in args)
            //{
            //    argList.Add(Calculater.ConvertObjectType(arg));
            //}
            object[] newArgs = args;// args.Select(p => Calculater.ConvertObjectType(p)).ToArray();
            //debug("newArgs " + string.Join(",",newArgs.Select(p=>p.ToString())));
            //debug(" ----- obj Type " + (obj.GetType()));
            //debug(" **** obj= " + obj.ToString());
            //debug(" **** obj= " + obj.ToString());

            object result = null;

            if (obj is Type)
            {
                //debug("is Type " + obj.ToString());
                Type type = (Type)obj;
                MethodInfo func = GetMethodInfo(type, funcName, newArgs);
                result =  func.Invoke(null, newArgs);
            }
            else
            {
                //debug("not is Type " + obj.ToString());
                Type type = obj.GetType();
                MethodInfo func = GetMethodInfo(type, funcName, newArgs);
                result = func.Invoke(obj, newArgs);
            }
            //debug(" **** call result= " + result.ToString());
            return result;
        }

        public static MethodInfo GetMethodInfo(Type type, string funcName, params object[] args)
        {
            //List<object> argList = new List<object>();
            //foreach (object arg in args)
            //{
            //    argList.Add(Calculater.ConvertObjectType(arg));
            //}
            //object[] newArgs = argList.ToArray();

            List<Type> types = new List<Type>();
            foreach (object arg in args)
            {
                types.Add(arg.GetType());
                //debug("arg = "+arg.ToString()+", type = " +arg.GetType().FullName);
            }

            MethodInfo func = type.GetMethod(funcName, types.ToArray());
            if (func == null)
            {
                throw new RTException(string.Format("没有找到类型{0}的方法{1}({2})", type.FullName, funcName, string.Join(",", types.Select(p => p.FullName))));
            }

            return func;
        }

        /*
        public static bool SetValue(object obj, string memberName, object value)
        {
            object newValue = Calculater.ConvertObjectType(value);
            Type type = null;
            if (obj is Type)
            {
                type = (Type)obj;
            }
            else
            {
                type = obj.GetType();
            }
           
            var property = type.GetProperty(memberName);
            if (property != null)
            {
                property.SetValue(obj is Type ? null : obj, newValue, null);
                return true;
            }

            var field = type.GetField(memberName);
            if (field != null)
            {
                field.SetValue(obj is Type ? null : obj, newValue);
                return true;
            }

            throw new TKTRTException("找不到类型" + type.FullName + "的" + memberName + "成员,无法赋值");
        }
        */
        public static object GetValue(object obj, string memberName)
        {
            Type type = null;
            if (obj is Type)
            {
                type = (Type)obj;
            }
            else
            {
                type = obj.GetType();
            }

            var property = type.GetProperty(memberName);
            if (property != null)
            {
                return property.GetValue(obj is Type ? null : obj,null);
            }

            var field = type.GetField(memberName);
            if (field != null)
            {
                return field.GetValue(obj is Type ? null : obj);
            }

            throw new RTException("找不到类型" + type.FullName + "的" + memberName + "成员,无法取值");
        }
        /*
        public static object NewInstance(Type type, params object[] args)
        {
            List<object> argList = new List<object>();
            //foreach (object arg in args)
            //{
            //    argList.Add(Calculater.ConvertObjectType(arg));
            //}
            //object[] newArgs = argList.ToArray();

            object[] newArgs = args.Select(p => Calculater.ConvertObjectType(p)).ToArray();

            object obj = Activator.CreateInstance(type, newArgs);//创建一个无参实例
            if (obj == null)
            {
                throw new TKTRTException("无法创建实例"+type.FullName);
            }
            return obj;
        }*/
    }
}
