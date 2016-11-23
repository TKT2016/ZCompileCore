using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Descs
{
    public class TKTProcDesc
    {
        public List<object> Parts { get; private set; }
        public ExMethodInfo ExMethod { get; set; }
        List<TKTProcArg> procArgs;

        public TKTProcDesc()
        {
            Parts = new List<object>();
            procArgs = new List<TKTProcArg>();
        }

        public void Add(string obj)
        {
            Parts.Add(obj);
        }

        public void Add(TKTProcArg obj)
        { 
            Parts.Add(obj);
            procArgs.Add(obj);
        }

        public void Add(List<TKTProcArg> args)
        {
            if (args.Count() > 1)
            {
                TKTProcBracket bracket = new TKTProcBracket (args);
                 Parts.Add(bracket);
                 procArgs.AddRange(args);
            }
            else if (args.Count() == 1)
            {
                Add(args[0]);
            }
        }

        public void Add(TKTProcBracket bracket)
        {
            var args = bracket.ListArgs;
            if (args.Count() > 1)
            {
                Parts.Add(bracket);
                procArgs.AddRange(args);
            }
            else if (args.Count() == 1)
            {
                Add(args[0]);
            }
        }

        public TKTProcArg GetArg(int i)
        {
            return procArgs[i];
        }
        
        public void SetArg(int k,TKTProcArg newArg)
        {
            procArgs[k] = newArg;
            int j = 0;
            int i = 0;
            for (; i < this.Parts.Count;)
            {
                object thisPart = this.Parts[i];
                if (thisPart is TKTProcArg)
                {
                    if(j==k)
                    {
                        Parts[i] = newArg;
                        break;
                    }
                    j++;
                }
                else if (thisPart is TKTProcBracket)
                {
                    TKTProcBracket bracket = thisPart as TKTProcBracket;
                    for(int m=0;i<bracket.ListArgs.Count;m++)
                    {
                        if (j == k)
                        {
                            Parts[i] = newArg;
                            bracket.SetArg(m, newArg);
                            break;
                        }
                        j++;
                    }
                }
                i++;
            }      
        }

        public int ArgCount
        {
            get
            {
                return procArgs.Count;
            }
        }

        private void Add(object obj)
        {
            if(obj is string)
            {          
                Add(obj as string);
            }
            else if (obj is TKTProcArg)
            {
                Add(obj as TKTProcArg);
            }
            else if (obj is IEnumerable<TKTProcArg>)
            {
                Add(obj as IEnumerable<TKTProcArg>);
            }
            else if (obj is TKTProcBracket)
            {
                Add(obj as TKTProcBracket);
            }
            else
            {
                throw new Exception("类型不正确");
            }
        }

        public void AdjustBracket(TKTProcDesc that)
        {
            //if (this.Bracket == null || that.Bracket == null) return;
            //that.Bracket = this.Bracket.Adjust(that.Bracket);
            for (int i = 0; i < this.Parts.Count; i++)
            {
                if ((this.Parts[i] is TKTProcBracket)&&(this.Parts[i] is TKTProcBracket ) )
                {
                    TKTProcBracket bracket = this.Parts[i] as TKTProcBracket;
                    var bracket2 = bracket.Adjust(that.Parts[i] as TKTProcBracket);
                    this.Parts[i] = bracket2;
                }
            }
            procArgs.Clear();
            for (int i = 0; i < this.Parts.Count; i++)
            {
               if (this.Parts[i] is TKTProcArg)
                {
                    procArgs.Add(this.Parts[i] as TKTProcArg);
                }
                else if (this.Parts[i] is TKTProcBracket)
                {
                    procArgs.AddRange((this.Parts[i] as TKTProcBracket).ListArgs);
                }
            }
        }

        public bool Eq(TKTProcDesc another)
        {
            if (this == another) return true;
            return EqParts(another);//&& EqBrackets(another);
        }

        bool EqParts(TKTProcDesc another)
        {
            if (this.Parts.Count != another.Parts.Count) return false;
            for (int i = 0; i < this.Parts.Count;i++ )
            {
                object thisPart = this.Parts[i];
                object anotherPart = another.Parts[i];

                if (thisPart is string)
                {
                    if (!(anotherPart is string)) return false;

                    if (((thisPart as string) != anotherPart as string))
                    {
                        return false;
                    }
                }
                else if (thisPart is TKTProcArg)
                {
                    if (!(anotherPart is TKTProcArg)) return false;

                    if (!((thisPart as TKTProcArg).Eq(anotherPart as TKTProcArg)))
                    {
                        return false;
                    }
                }
                else if (thisPart is TKTProcBracket)
                {
                    if (!(anotherPart is TKTProcBracket)) return false;

                    if (!((thisPart as TKTProcBracket).Eq(anotherPart as TKTProcBracket)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /*
        bool EqBrackets(TKTProcDesc another)
        {
            if (this.Bracket == null && another.Bracket == null) return true;
            if (this.Bracket != null && another.Bracket != null) return this.Bracket.Eq(another.Bracket);
            return false;
        }*/

        public bool HasSubject()
        {
            return (Parts.Count > 0 && Parts[0] is TKTProcArg);
        }

        public TKTProcArg GetSubjectArg()
        {
            if (!HasSubject()) return null;
            return (Parts[0] as TKTProcArg);
        }
        
        public TKTProcDesc CreateTail()
        {
            TKTProcDesc tailDesc = new TKTProcDesc();
            List<object> list = this.Parts;
            for (int i = 1; i < list.Count;i++ )
            {
                object item = list[i];
                tailDesc.Add(item);
            }
            //if(this.Bracket!=null)
            //{
            //    tailDesc.Bracket = this.Bracket;
            //}
            return tailDesc;
        }

        //List<TKTProcArg> _argsList;
        public List<TKTProcArg> ArgsList
        {
            get
            {
                return procArgs;
               /* _argsList = null;
                if (_argsList == null)
                {
                    _argsList = new List<TKTProcArg>();
                    _argsList.AddRange(GetSpecialArgs(0, Parts));
                    if (this.Bracket != null)
                    {
                        _argsList.AddRange(GetSpecialArgs(0,this.Bracket.ListArgs));
                    }
                }
                return _argsList;*/
            }
        }

        /// <summary>
        /// 根据查询类型获取过程参数
        /// </summary>
        /// <param name="getType">过程类型(0:全部,1:普通参数,2:泛型参数)</param>
        public List<TKTProcArg> GetSpecialArgs(int getType)
        {
            if(getType==0)
            {
                return ArgsList;
            }
            else if (getType == 1)
            {
                return ArgsList.Where(p => p.IsGenericArg == false).ToList();
            }
            else if (getType == 2)
            {
                return ArgsList.Where(p => p.IsGenericArg).ToList();
            }
            else
            {
                return ArgsList;
            }
        }

        private List<TKTProcArg> GetSpecialArgs(int getType, List<TKTProcArg> args)
        {
            List<TKTProcArg> list = new List<TKTProcArg>();
            foreach (var arg in args)
            {
                if (getType == 0)
                {
                    list.Add(arg);
                }
                else if (getType == 1 && !arg.IsGenericArg)
                {
                    list.Add(arg);
                }
                else if (getType == 2 && arg.IsGenericArg)
                {
                    list.Add(arg);
                }
            }
            return list;
        }

        private List<TKTProcArg> GetSpecialArgs(int getType, List<object> objs)
        {
            List<TKTProcArg> list = new List<TKTProcArg>();
            foreach (var part in objs)
            {
                if (part is TKTProcArg)
                {
                    TKTProcArg arg = part as TKTProcArg;
                    if (getType == 0)
                    {
                        list.Add(arg);
                    }
                    else if (getType == 1 && !arg.IsGenericArg)
                    {
                        list.Add(arg);
                    }
                    else if (getType == 2 && arg.IsGenericArg)
                    {
                        list.Add(arg);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 根据查询类型获取过程参数类型
        /// </summary>
        /// <param name="getType">过程类型(0:全部,1:普通参数,2:泛型参数)</param>
        /// <returns></returns>
        public List<Type> GetArgTypes(int getType)
        {
            List<TKTProcArg> args = this.GetSpecialArgs(getType);
            List<Type> types = args.Select(p => p.ArgType).ToList();
            return types;
        }

        public int Count
        {
            get
            {
                return Parts.Count;
            }
        }

        public override string ToString()
        {
             List<string> list = new List<string>();
            for (int i = 0; i < this.Parts.Count; i++)
            {
                if (this.Parts[i] is string)
                {
                    list.Add(this.Parts[i] as string);
                }
                else if (this.Parts[i] is TKTProcArg)
                {
                    list.Add("(" + (this.Parts[i] as TKTProcArg).ToString()+")");
                }
                else if (this.Parts[i] is TKTProcBracket)
                {
                    list.Add((this.Parts[i] as TKTProcBracket).ToString() );
                }
            }
            return string.Join("", list);
        }
    }
}
