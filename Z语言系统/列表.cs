using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLangRT;
using ZLangRT.Attributes;

namespace Z语言系统
{
    [ZClass]
    public class 列表<T> //: IList<T>
    {
        List<T> list;

        public 列表()
        {
            list = new List<T>();
        }

        public 列表(IEnumerable<T> k)
        {
            list = new List<T>(k);
        }

        int getIndex(int i)
        {
            if (i > 0)
            {
                return i - 1;
            }
            else if (i < 0)
            {
                return i + list.Count;
            }
            else
            {
                throw new Exception("列表的索引不能为0");
            }
        }

        public T this[int index]
        {
            get
            {
                //Console.WriteLine("index="+index);
                int i = getIndex(index);
                return list[i];
            }
            set
            {
                int i = getIndex(index);
                list[i] = value;
            }
        }

        [ZCode("个数")]
        public int Count
        {
            get
            {
                return this.list.Count;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("列表个数不能为负数");
                }
                else if (value == 0)
                {
                    list.Clear();
                }
                else if (value < list.Count)
                {
                    List<T> list2 = new List<T>();
                    for (int i = 0; i < value - 1; i++)
                    {
                        list2.Add(list[i]);
                    }
                    list.Clear();
                    list = list2;
                }
                else if (value > list.Count)
                {
                    while (value < list.Count)
                    {
                        list.Add(default(T));
                    }
                }
            }
        }

        [ZCode("添加(T:item)")]
        public void Add(T item)
        {
            //Console.WriteLine("加入 "+item);
            list.Add(item);
        }

        [ZCode("包含(T:item)")]
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        [ZCode("删除(T:item)")]
        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        [ZCode("删除第(int:index)")]
        public void RemoveAt(int index)
        {
            int i = getIndex(index);
            list.RemoveAt(i);
        }

        [ZCode("删除(列表<T>:LB)")]
        public int RemoveList(列表<T> LB)
        {
            int count = 0;
            List<T> list2 = new List<T>();
            foreach (var item in this.list)
            {
                if(LB.list.IndexOf(item)==-1)
                {
                    list2.Add(item);
                }
                else
                {
                    count++;
                }
            }
            this.list.Clear();
            this.list = list2;
            return count;
        }

        [ZCode("清空")]
        public void Clear()
        {
            list.Clear();
        }

        [ZCode("查找(T:item)序号")]
        public int IndexOf(T item)
        {
            return list.IndexOf(item) + 1;
        }
       
        [ZCode("插入(T:t)到第(int:index)")]
        public void Insert( T item,int index)
        {
            //插入_到(item, index);
            int i = getIndex(index);
            //Console.WriteLine(i);
            list.Insert(index, item);
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /*
        public IEnumerable<T> GetEnumerator()
        {
            return list.GetEnumerator() as IEnumerable<T>;
        }

        public void CopyTo(T[] array,int arrayIndex)
        {
            int index= getIndex(arrayIndex);
            list.CopyTo(array, index);
        }
        
        public List<T>.Enumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }
        
        public IEnumerator IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }*/
    }
}
