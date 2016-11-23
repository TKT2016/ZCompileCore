using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Descs
{
    public class TKTConstructorDesc
    {
        //public List<object> Parts { get; private set; }
        public TKTProcBracket Bracket { get; private set; }

        public ConstructorInfo Constructor { get; set; }
        //public object Model { get; set; }

        public TKTConstructorDesc()
        {
            //Parts = new List<object>();
        }

        public TKTConstructorDesc(IEnumerable<TKTProcArg> args)
        {
            //Parts = new List<object>();
            //Add(args);
            Bracket = new TKTProcBracket(args);
        }

        public bool Eq(TKTConstructorDesc another)
        {
            if (this == another) return true;
            return EqBrackets(another);
        }

        bool EqBrackets(TKTConstructorDesc another)
        {
            if (this.Bracket == null && another.Bracket == null) return true;
            if (this.Bracket != null && another.Bracket != null) return this.Bracket.Eq(another.Bracket);
            return false;
        }
       
        public void AdjustBracket(TKTConstructorDesc that)
        {
            if (this.Bracket == null || that.Bracket == null) return;
            that.Bracket = this.Bracket.Adjust(that.Bracket);
        }

        public int Count
        {
            get
            {
                return (Bracket==null?0:Bracket.Count);
            }
        }

        public override string ToString()
        {
            List<string> list = new List<string>();
            if(this.Constructor!=null)
            {
                list.Add(this.Constructor.DeclaringType.Name);
            }
            if(this.Bracket!=null)
            {
                list.Add(Bracket.ToString());
            }
            return string.Join("", list);
        }
    }
}
