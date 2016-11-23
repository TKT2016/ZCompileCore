using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Lex;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;

namespace ZCompileCore.AST.Parts
{
    public class PackageAST:FileElementAST
    {
        public List<Token> PackageTokens { get; set; }
        public PackageModel PackageModel { get; set; }
        public PackageAST()
        {
            PackageTokens = new List<Token>();
        }

        public PackageModel Analy()
        {
            if(PackageTokens.Count==0)
            {
                error("没有包名称");
                return null;
            }
            List<string> list = new List<string>();
            foreach (Token item in this.PackageTokens)
            {
                list.Add(item.ToCode());
            }
            var FullName = string.Join(".", list);
            List<string> list2 = new List<string>();
            int size = PackageTokens.Count;
            for (int i = 0; i < size - 1; i++)
            {
                var item = PackageTokens[i];
                list2.Add(item.ToCode());
            }
            var PackageName = string.Join(".", list2);
            var TypeName = PackageTokens[size - 1].ToCode();
            PackageModel = new PackageModel();
            PackageModel.FullName = FullName;
            PackageModel.PackageName = PackageName;
            PackageModel.TypeName = TypeName;
            return PackageModel;
        }

        #region 位置格式化
        public override string ToCode()
        {
            List<string> list = new List<string>();
            foreach (Token item in this.PackageTokens)
            {
                list.Add(item.ToCode());
            }
            string str =string.Join("/",list);
            return str;
        }
        #endregion

        public override CodePostion Postion
        {
            get { return PackageTokens[0].Postion; }
        }
    }

    public class PackageModel
    {
        public string FullName { get; set; }
        public string PackageName { get;  set; }
        public string TypeName { get;  set; }
    }
}
