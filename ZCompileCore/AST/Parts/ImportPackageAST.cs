using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ZCompileCore.Analys;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Lex;
using ZCompileCore.Loads;
using ZLangRT.Descs;

namespace ZCompileCore.AST.Parts
{
    public class ImportPackageAST : PartAST
    {
        public Token KeyToken;
        public List< PackageAST> Packages { get; set; }
        SymbolTable Symbols;

        public ImportPackageAST()
        {
            Packages = new List<PackageAST>();
        }

        public void Analy(ImportContext context, ClassContext classContext, Dictionary<string, PackageModel> importNames)
        {
            Symbols = classContext.Symbols;
            checkAddTKTSystemPackage();
            foreach (var item in this.Packages)
            {
                var PackageModel = item.Analy();
                if (PackageModel != null)
                {
                    if (importNames.ContainsKey(PackageModel.FullName))
                    {
                        errorf("'{0}'已经声明使用过", PackageModel.FullName);
                    }
                    else
                    {
                        loadPackage(PackageModel.FullName, classContext);
                    }
                }     
            }
        }

        void checkAddTKTSystemPackage()
        {
            if (!existsTKTSystemPackage())
            {
                Token tktsysToken = new Token() { Text = CompileConstant.LangPackageName, Kind = TokenKind.Ident, Col = 0, Line = 0 };
                PackageAST packageAST = new PackageAST();
                packageAST.PackageTokens.Add(tktsysToken);
                Packages.Insert(0, packageAST);
            }
        }

        bool existsTKTSystemPackage()
        {
            foreach (var item in this.Packages)
            {
                if(item.PackageTokens!=null && item.PackageTokens.Count==1)
                {
                    Token packageToken = item.PackageTokens[0];
                    if (packageToken.GetText() == CompileConstant.LangPackageName)
                    {
                        return true;
                    }
                }
            }
            return false;
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

        void loadPackage(string packageName, ClassContext classContext)
        {
            var refTypes = classContext.ProjectContext.TKTTypes;
            var libWords = classContext.ProjectContext.LibWords;
            var genericTypes = classContext.ImportContext.GenericTypes;
            var importPackages = classContext.ImportContext.importPackages;

            List<IGcl> list = new List<IGcl>();
            foreach (var type in refTypes)
            {
                if (type.IsPublic && type.Namespace == packageName)
                {
                    IGcl gcl = GclUtil.Load(type, getWordDict(libWords, type));
                    if (gcl != null)
                    {
                        list.Add(gcl);
                        addGeneric(genericTypes, gcl);
                    }
                }
            }
            if(list.Count==0)
            {
                errorf("开发包'{0}'内没有类型", packageName);
            }
            else
            {
                importPackages.Add(packageName, list);
            }
        }

        static void addGeneric(List<IGcl> genericTypes,IGcl gcl)
        {
            if(gcl.ForType.IsGenericType)
            {
                genericTypes.Add(gcl);
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
            foreach (var item in this.Packages)
            {
                buf.Append(item.ToCode());
            }
            buf.Append(string.Join(",", tempList));
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
