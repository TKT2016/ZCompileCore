
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZLangRT.Descs;

namespace ZCompileCore.Loads
{
    public interface IGcl
    {
        Type ForType { get; }
        string ShowName { get; }
        string RTName { get; }
        ExFieldInfo SearchExField(string name);
        ExPropertyInfo SearchExProperty(string name);
        TKTProcDesc SearchProc(TKTProcDesc procDesc);
        TKTConstructorDesc SearchConstructor(TKTConstructorDesc bracket);
        IGcl CreateNewFor(Type forType);
        CnEnDict WordDict { get; }
    }
}
