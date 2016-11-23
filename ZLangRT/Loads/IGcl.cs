
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TKT.RT.Descs;

namespace TKT.RT.Loads
{
    public interface IGcl
    {
        Type ForType { get; }
        string ShowName { get; }
        string RTName { get; }
        FieldInfo SearchField(string name);
        PropertyInfo SearchProperty(string name);
        TKTProcDesc SearchProc(TKTProcDesc procDesc);
        TKTConstructorDesc SearchConstructor(TKTConstructorDesc bracket);
        IGcl CreateNewFor(Type forType);
        CnEnDict WordDict { get; }
    }
}
