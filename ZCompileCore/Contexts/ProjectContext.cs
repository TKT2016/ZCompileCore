using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ZCompileCore.Analys.EContexts;
using ZLangRT;
using ZLangRT.Descs;
using ZLangRT.Utils;

namespace ZCompileCore.Analys.AContexts
{
    public class ProjectContext
    {
        public string RootNameSpace { get; set; }
        public EmitProjectContext EmitContext { get; set; }
        public PEFileKinds BinaryFileKind { get; set; }
        public string BinaryFileNameNoEx { get; set; }
        public List<Assembly> Refs { get; private set; }
        public List<Type> ProjectTypes { get; private set; }
        public DirectoryInfo BinarySaveDirectoryInfo { get; set; }

        public void LoadProjectType(Type type)
        {
            ProjectTypes.Add(type);
        }

        public Type GetProjectType(string name)
        {
            foreach (var type in ProjectTypes)
            {
                if (type.Name == name)
                    return type;
            }
            return null;
        }

        public ProjectContext()
        {
            Refs = new List<Assembly>();
            EmitContext = new EmitProjectContext();
            ProjectTypes = new List<Type>();
            BinaryFileKind = PEFileKinds.Dll;
        }

        public void AddAssembly(Assembly asm)
        {
            if (ExistsRef(asm)) return;
            Refs.Add(asm);
        }

        public bool ExistsRef(Assembly asm)
        {
            foreach (var item in Refs)
            {
                if (item.GetName().FullName == asm.GetName().FullName)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddFile(string filePath)
        {
            Assembly asm = Assembly.LoadFile(filePath);
            AddAssembly(asm);
        }

        public void AddPackage(string packageName)
        {
            Assembly asm = Assembly.Load(packageName);
            AddAssembly(asm);
        }

        public List<Type> TKTTypes { get; private set; }
        public Dictionary<Assembly, CnEnDict> LibWords { get; private set; }
        public void LoadRefTypes()
        {
            TKTTypes = new List<Type>();
            LibWords = new Dictionary<Assembly, CnEnDict>();
            foreach (Assembly asm in this.Refs)
            {
                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsPublic)
                        {
                            TKTTypes.Add(type);
                            if (ReflectionUtil.IsExtends(type, typeof(CnEnDict)))
                            {
                                CnEnDict dict = ReflectionUtil.NewInstance(type) as CnEnDict;
                                LibWords.Add(asm, dict);
                            }
                        }
                    }
            }
        }

        public string GetBinaryNameEx()
        {
            string binFileName = this.BinaryFileNameNoEx;
            if (this.BinaryFileKind == PEFileKinds.Dll)
            {
                binFileName += ".dll";
            }
            else
            {
                binFileName += ".exe";
            }
            return binFileName;
        }

        //public string BinFileName { get; set; }
    }
}
