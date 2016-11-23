using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZCompileCore.Tools
{
    public static class ListHelper
    {
        public static List<List<T>> Split<T>(List<T> list,List<int> indexList)
        {
            List<List<T>> splitList = new List<List<T>>();
            if (indexList.Count == 0)
            {
                splitList.Add(list);
            }
            else
            {
                if (First(indexList) != 0)
                {
                    var subs = SubList<T>(list, 0, indexList[0]);
                    splitList.Add(subs);
                }
                for (int i = 0; i < indexList.Count - 1; i++)
                {
                    var subs = SubList<T>(list, indexList[i], indexList[i + 1]);
                    splitList.Add(subs);
                }
                if (Last(indexList) != list.Count - 1)
                {
                    var subs = SubList<T>(list, 0, indexList[0]);
                    splitList.Add(subs);
                }
            }
            return splitList;
        }

        public static List<T> SubList<T>( List<T> list, int from, int to)
        {
            List<T> subList = new List<T>();
            if (from == to) return subList;
            for (int i = from; i <= to && i < list.Count; i++)
            {
                subList.Add(list[i]);
            }
            return subList;
        }

        public static T First<T>(List<T> list)
        {
            return list[0];
        }

        public static T Last<T>(List<T> list)
        {
            return list[list.Count-1];
        }
    }
}
