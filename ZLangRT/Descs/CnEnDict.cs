using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLangRT.Descs
{
    public abstract class CnEnDict
    {
        Dictionary<string, string> ArgNameDict;

        public CnEnDict()
        {
            ArgNameDict = new Dictionary<string, string>();
            AddWords();
        }

        public abstract void AddWords();

        public string Get(string cnword)
        {
            if (ArgNameDict.ContainsKey(cnword)) return ArgNameDict[cnword];
            else return cnword;
        }

        public bool AddSafe(string cn,string en)
        {
            if (ArgNameDict.ContainsKey(cn)) return false;
            ArgNameDict.Add(cn, en);
            return true;
        }
    }
}
