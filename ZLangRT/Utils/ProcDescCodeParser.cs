using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZLangRT;
using ZLangRT.Descs;

namespace ZLangRT.Utils
{
    public class ProcDescCodeParser
    {
        Dictionary<string, Type> ArgTypeDict;
        CnEnDict WordDict;

		public ProcDescCodeParser()
        {
            ArgTypeDict = new Dictionary<string, Type>();
            ArgTypeDict.Add("object", typeof(object));
            ArgTypeDict.Add("Object", typeof(object));
            ArgTypeDict.Add("int", typeof(int));
            ArgTypeDict.Add("float", typeof(float));
            ArgTypeDict.Add("bool", typeof(bool));
            ArgTypeDict.Add("string", typeof(string));
            ArgTypeDict.Add("String", typeof(string));
            ArgTypeDict.Add("void", typeof(void));
            ArgTypeDict.Add("可运行条件", typeof(Func<bool>));
            ArgTypeDict.Add("可运行语句", typeof(Action));
        }
        
        void AddType(Type type)
        {
            string name = type.Name;
            if (ArgTypeDict.ContainsKey(name) == false)
                ArgTypeDict.Add(name, type);
        }
        
        void AddType(string name,Type type)
        {
            if (ArgTypeDict.ContainsKey(name) == false)
                ArgTypeDict.Add(name, type);
        }

        public void InitType(Type type, MethodInfo method)
        {
            if (type.IsGenericType)
            {
                Type parentType = type.GetGenericTypeDefinition();
                Type[] subTypes = GenericUtil.GetInstanceGenriceType(type, parentType);
                Type[] gengeParams = parentType.GetGenericArguments();
                for (int i = 0; i < gengeParams.Length; i++)
                {
                    AddType(gengeParams[i].Name, subTypes[i]);
                }
            }
            ParameterInfo[] paramArray = method.GetParameters();
            foreach (var param in paramArray)
            {
                if (!param.ParameterType.IsGenericType)
                    AddType(param.ParameterType);
            }

            foreach (var param in paramArray)
            {
                if (param.ParameterType.IsGenericType)
                {
                    var ptype = param.ParameterType;
                    Type parentType = ptype.GetGenericTypeDefinition();
                    Type[] subTypes = GenericUtil.GetInstanceGenriceType(ptype, parentType);
                    Type[] gengeParams = parentType.GetGenericArguments();
                    //Type[] arr2 = new Type[gengeParams.Length];
                    string[] subNames = new string[gengeParams.Length];
                    for (int i = 0; i < gengeParams.Length; i++)
                    {
                        //subNames[i] = gengeParams[i].Name;
                        //AddType(gengeParams[i].Name, subTypes[i]);
                        string gengeParamName = gengeParams[i].Name;
                        subNames[i] = gengeParamName;
                        //arr2[i] = ArgTypeDict[gengeParamName];
                    }
                    Type newType = ptype;// ptype.MakeGenericType(arr2);
                    string newTypeName = GenericUtil.GetGenericTypeShortName(ptype) + "<" + string.Join(",", subNames) + ">";
                    AddType(newTypeName, newType);
                }
            }
        }

        int i = 0;
        TKTProcDesc desc = null;
        string Code = null;
        char ch
        {
            get
            {
                if (i > Code.Length - 1) return '\0';
                return Code[i];
            }
        }

        public TKTProcDesc Parser(CnEnDict wordDict, string code)
        {
            this.WordDict = wordDict;
            i = 0;
            desc = new TKTProcDesc();
            Code = code;
            while (i < Code.Length)
            {
                if(ch=='(')
                {
                    parseBracket();
                }
                else
                {
                    parseText();
                }
            }
            return desc;
        }

        void parseText()
        {
            StringBuilder buff = new StringBuilder();
            for (;  i < Code.Length&&ch != '(' ; i++)
            {
                buff.Append(ch);
            }
            desc.Add(buff.ToString());
        }

        void parseBracket()
        {
            i++;
            List<TKTProcArg> bracketargs = new List<TKTProcArg>();
            //Dictionary<string, TKTProcArg> dict = new Dictionary<string, TKTProcArg>();
            for (; i < Code.Length; i++ )
            {
                TKTProcArg arg = parseArg();
                if (arg != null)
                {
                    bracketargs.Add(arg);
                }
                //dict.Add(arg.ArgName, arg);
                if(ch=='\0')
                {
                    break;
                }
                if (ch == ')')
                {
                    i++;
                    break;
                } 
            }
            //TKTProcBracket bracket = new TKTProcBracket(bracketargs,dict);
            desc.Add(bracketargs);
        }

        TKTProcArg parseArg()
        {
            string argtypename = parseIdent();
            movenext();
            string argname = parseIdent();
            string realArgName = argname;
            if (string.IsNullOrEmpty(argname)) return null;
            if (this.WordDict != null)
            {
                realArgName = this.WordDict.Get(argname);
            }
            TKTProcArg arg = null;
            if (argtypename=="类型")
            {
                Type type = typeof(object);
                arg = new TKTProcArg(realArgName, type, true);
            }
            else if (ArgTypeDict.ContainsKey(argtypename))
            {
                Type type = ArgTypeDict[argtypename];
                arg = new TKTProcArg(realArgName, type, false);
            }
            else if (argtypename=="Func<bool>")//(argtypename.IndexOf("<") > 0 && argtypename.IndexOf(">") > 0)
            {
                //if(argtypename=="Func<bool>")
                //{
                    arg = new TKTProcArg(realArgName, typeof(Func<bool>), false);
                //}
            }
            else if(argtypename=="Action")
            {
                arg = new TKTProcArg(realArgName, typeof(Action), false);
            }
			else
            {
                throw new RTException("没有导入'" + argtypename+"'类型");
            }	
            return arg;
        }

        string parseIdent()
        {
            StringBuilder buff = new StringBuilder();
            for (; i < Code.Length; i++)
            {
                if (ch == ':'||ch == ')'||ch == ',')
                {
                    break;
                }
                else
                {
                    buff.Append(ch);
                }
            }
            return buff.ToString();
        }

        void movenext()
        {
            i++;
        }
    }
}
