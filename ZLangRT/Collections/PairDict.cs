using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Collections
{
    public class PairDict<K,V>
    {
        Dictionary<K, V> kvdict;
        Dictionary<V, K> vkdict;

        public PairDict()
        {
            kvdict = new Dictionary<K, V>();
            vkdict = new Dictionary<V, K>();
        }

        public bool Add(K k,V v)
        {
            if (kvdict.ContainsKey(k) || vkdict.ContainsKey(v))
                return false;
            kvdict.Add(k, v);
            vkdict.Add(v, k);
            return true;
        }

        public bool Containsk(K k)
        {
            return kvdict.ContainsKey(k);
        }

        public bool Containsv(V v)
        {
            return vkdict.ContainsKey(v);
        }

        public V Getv(K k)
        {
            return kvdict[k];
        }

        public K Getk(V v)
        {
            return vkdict[v];
        }

        public int Count()
        {
            return kvdict.Count;
        }

        public List<K> Keys
        {
            get
            {
                return kvdict.Keys.ToList();
            }
        }
    }
}
