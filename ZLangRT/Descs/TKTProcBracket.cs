using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Descs
{
    public class TKTProcBracket
    {
        public List<TKTProcArg> ListArgs { get; protected set; }
        public Dictionary<string, TKTProcArg> DictArgs { get; protected set; }

        bool HasArgName;
        public bool CanAjust { get;private set; }

        public bool IsNameValue
        {
            get
            {
                return DictArgs.Count > 0 && DictArgs.Count == ListArgs.Count;
            }
        }

        public TKTProcBracket(IEnumerable<TKTProcArg> listArgs)
        {
            ListArgs = new List<TKTProcArg>(listArgs);
            DictArgs = new Dictionary<string, TKTProcArg>();
           
            foreach(var arg in listArgs)
            {
                if (string.IsNullOrEmpty(arg.ArgName)) break;
                HasArgName = true;
                DictArgs.Add(arg.ArgName, arg);
            }
            CanAjust = checkCanAjust(HasArgName, ListArgs);
        }

        static bool checkCanAjust(bool hasArgName,List<TKTProcArg> listArgs)
        {
            if (!hasArgName) return false;
            List<Type> types = listArgs.Select(p => p.ArgType).ToList();
            Dictionary<Type, int> dict = new Dictionary<Type, int>();
            foreach(var type in types)
            {
                if(dict.ContainsKey(type))
                {
                    return false;
                }
                else
                {
                    dict.Add(type, 0);
                }
            }
            return true;
        }

        public TKTProcBracket Adjust(TKTProcBracket that)
        {
            if (!this.CanAjust || !that.CanAjust) return this;
            List<TKTProcArg> list = new List<TKTProcArg>();
            foreach (var arg in this.ListArgs)
            {
                string name = arg.ArgName;
                TKTProcArg thatArg = that.DictArgs[name];
                list.Add(thatArg);
            }
            return new TKTProcBracket(list);
        }

        public void SetArg(int i,TKTProcArg newArg)
        {
            string name = ListArgs[i].ArgName;
            ListArgs[i] = newArg;
            DictArgs.Remove(name);
            DictArgs.Add(newArg.ArgName, newArg);
        }
        
        public int Count
        {
            get
            {
                return ListArgs.Count;
                /*if (BracketType == 0) return 0;
                if (BracketType == 1 || BracketType == 4) return ListArgs.Count;
                if (BracketType == 2) return DictArgs.Count;*/
                throw new RTException("TKTProcBracket有问题");
            }
        }

        bool eq_list(TKTProcBracket anthor)
        {
            if (this.ListArgs.Count != anthor.ListArgs.Count) return false;
            for (int i = 0; i < ListArgs.Count; i++)
            {
                if (!this.ListArgs[i].Eq(anthor.ListArgs[i]))
                {
                    return false;
                }
            }
            return true;
        }

        bool eq_dict(TKTProcBracket anthor)
        {
            if (this.DictArgs.Count != anthor.DictArgs.Count) return false;
            foreach(var key in this.DictArgs.Keys)
            {
                if(!this.DictArgs[key].Eq(anthor.DictArgs[key]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Eq(TKTProcBracket anthor)
        {
            if(this.HasArgName)
            {
                if(anthor.HasArgName)
                {
                    return eq_dict(anthor)/* && eq_list(anthor)*/;
                }
                else
                    return eq_list(anthor);
            }
            else
            {
                return eq_list(anthor);
            }
            //return eq_dict(anthor);
            /*
            if (BracketType == 0)
            {
                return (anthor.BracketType == 0);
            }
            else if (BracketType == 1)
            {
                return eq_single(anthor);
            }
            else if (BracketType == 2 || anthor.BracketType==2)
            {
                return eq_list(anthor);
            }
            else if (BracketType == 3 || BracketType == 4)
            {
                return eq_dict(anthor);
            }*/
            throw new RTException("TKTProcBracket有问题");
        }

        public override string ToString()
        {/*
            if (BracketType == 0)
                return "";
            else if (BracketType == 1) 
                return "(" + SingleArg.ToString() + ")";
            else if (BracketType == 2)
            {
                List<string> buff = new List<string>();
                foreach (var arg in ListArgs)
                {
                    buff.Add(arg.ToString());
                }
                return "(" + string.Join(",", buff) + ")";
            }
            else if (BracketType == 3 || BracketType == 4)
            {*/
                List<string> buff = new List<string>();
                foreach (var arg in ListArgs)
                {
                    buff.Add(arg.ToString());
                }
                return "TKTProcBracket(" + string.Join(",", buff) + ")";
            //}
            //throw new RTException("TKTProcBracket有问题");
        }

        public List<TKTProcArg> ToList()
        {/*
            if (BracketType == 0)
            {
                return new List<TKTProcArg> ();
            }
            else if (BracketType == 1)
            {
                var list = new List<TKTProcArg> ();
                list.Add(SingleArg);
                return list;
            }
            else if (BracketType == 2)
            {
                return ListArgs;
            }
            else if (BracketType == 3 || BracketType == 4)
            {
                return DictArgs.Values.ToList();
            }
            else if (BracketType == 4)
            {*/
                return ListArgs;
            /*}
            throw new RTException("TKTProcBracket有问题");*/
        }
    }
}
