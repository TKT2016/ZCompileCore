using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Descs;

namespace ZLangRT.Collections
{
    public class TKTProcDescDictionary<V>
    {
        Dictionary<TKTProcDesc, V> dict;
        public TKTProcDescDictionary()
        {
            dict = new Dictionary<TKTProcDesc, V>();
        }

        public void Add(TKTProcDesc procDesc,V v)
        {
            dict.Add(procDesc, v);
        }

        public bool Contains(TKTProcDesc procDesc)
        {
            foreach (TKTProcDesc key in dict.Keys.ToList())
            {
                if (key.Eq(procDesc))
                {
                    return true;
                }
            }
            return false;
        }

        public V Get(TKTProcDesc procDesc)
        {
            foreach (TKTProcDesc key in dict.Keys.ToList())
            {
                if (key.Eq(procDesc))
                {
                    return dict[key];
                }
            }
            return default(V);
        }

        public TKTProcDesc SearchProc(TKTProcDesc procDesc)
        {
            foreach (TKTProcDesc key in dict.Keys.ToList())
            {
                if (key.Eq(procDesc))
                {
                    return key;
                }
            }
            return null;
        }

        public int Count
        {
            get
            {
                return dict.Count;
            }
        }

    }
}
