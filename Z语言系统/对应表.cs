using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZClass]
    public class 对应表<K,V>
    {
        Dictionary <K,V> dict;

        public 对应表()
        {
            dict = new Dictionary <K,V>();
        }

        int getIndex(int i)
        {
            if(i>0)
            {
                return i - 1;
            }
            else if(i<0)
            {
                return i + dict.Count;
            }
            else
            {
                throw new Exception("列表的索引不能为0");
            }
        }

        public V this[K k]
        {
            get
            {
                return dict[k];
            }
            set
            {
                dict[k] = value;  
            }
        }

        public int 个数
        {
            get
            {
                return this.dict.Count;
            }
        }

        public void 添加(K k,V v)
        {
            dict.Add(k,v);
        }

        public bool 删除(K k)
        {
            if (!dict.ContainsKey(k)) return false;
            dict.Remove(k);
            return true;
        }

        public void 清空()
        {
            dict.Clear();
        }

        [Code("判断存在(K:k)")]
        public bool 判断存在(K k)
        {
            return dict.ContainsKey(k);
        }
    }
}
