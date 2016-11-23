using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.AST.Parts;
using ZCompileCore.Analys;
using ZCompileCore.Loads;
using ZCompileCore.Tools;
using ZLangRT;
using ZLangRT.Descs;

namespace ZCompileCore.Analyers
{
    public class ImportAnalyer
    {
        ClassContext classContext;
        ImportPackageAST importPackage;
        Dictionary<string, PackageModel> importPackages;
        SimpleUseAST simpleUse;

        public ImportAnalyer(ClassContext classContext, ImportPackageAST importList, SimpleUseAST simpleUse)
        {
            this.classContext = classContext;
            importPackage = importList;
            this.simpleUse = simpleUse;
        }

        public void Analy()
        {
            importPackages = new Dictionary<string, PackageModel>();
            var context = classContext.ImportContext;
            importPackage.Analy(context, classContext, importPackages);
            if (simpleUse != null)
            {
                simpleUse.Analy(context, classContext);
            }
        }
       
    }
}

