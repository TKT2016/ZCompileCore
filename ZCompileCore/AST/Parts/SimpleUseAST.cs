using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZCompileCore.Analys;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Parts
{
    public class SimpleUseAST : PartAST
    {
        public Token KeyToken;
        public List<Token> NameTokens { get; set; }

        public SimpleUseAST()
        {
            NameTokens = new List<Token>();
        }

        public void Analy(ImportContext context, ClassContext classContext)
        {
            foreach(var nameToken in NameTokens)
            {
                string typeName = nameToken.GetText();
                List<IGcl> gcls = searchType(typeName, classContext);
                int count = gcls.Count;
                if(count==1)
                {
                    var gcl = gcls[0];
                    var importTypes = classContext.ImportContext.DirectClasses;
                    importTypes.Add(gcl);
                }
                else if(count==0)
                {
                    errorf("没有找到类型'{0}'", typeName);
                }
                else
                {
                    errorf("有多个'{0}'类型", typeName);
                }
            }
        }

        public List<IGcl> searchType(string typeName, ClassContext classContext)
        {
            List<IGcl> list = new List<IGcl>();
            var refTypes = classContext.ProjectContext.TKTTypes;
            var libWords = classContext.ProjectContext.LibWords;
            foreach (var type in refTypes)
            {
                if (type.IsPublic && type.Name == typeName)
                {
                    var gcl = GclUtil.Load(type, getWordDict(libWords, type));
                    if (gcl != null)
                    {
                        list.Add(gcl);
                    }
                }
            }
            return list;
        }
        
        CnEnDict getWordDict(Dictionary<Assembly, CnEnDict> libWords,Type type)
        {
            if(libWords.ContainsKey(type.Assembly))
            {
                return libWords[type.Assembly];
            }
            else
            {
                return null;
            }
        }
        
        #region 位置格式化
        public override string ToCode()
        {
            Deep = 1;
            StringBuilder buf = new StringBuilder();
            buf.Append(KeyToken.GetText());
            buf.Append(":");
            List<string> tempList = new List<string>();
            foreach(var name in NameTokens)
            {
                buf.Append(name.GetText());
            }
            buf.Append(string.Join(",",tempList));
            buf.AppendLine(";");
            return buf.ToString();
        }

        public override CodePostion Postion
        {
            get { return KeyToken.Postion; }
        }
        #endregion
    }
}
