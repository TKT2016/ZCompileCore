using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Reports;
using ZCompileCore.Tools;

namespace ZCompileCore.Builders
{
    public class ZCompileBuilder
    {
        public ProjectCompileResult CompileProject(ZCompileProjectModel zCompileProjectModel)
        {
            ProjectContext projectContext = new ProjectContext();
            ProjectCompileResult result = CompileProject(zCompileProjectModel, projectContext);
            return result;
        }

        private ProjectCompileResult CompileProject(ZCompileProjectModel zCompileProjectModel, ProjectContext projectContext)
        {
            ProjectCompileResult result = new ProjectCompileResult();
            Messager.Clear();

            projectContext.RootNameSpace = zCompileProjectModel.ProjectPackageName; //设置项目包名称
            projectContext.BinaryFileKind = zCompileProjectModel.BinaryFileKind; //设置项目目标文件类型
            projectContext.BinaryFileNameNoEx = zCompileProjectModel.BinaryFileNameNoEx;
            projectContext.BinarySaveDirectoryInfo = zCompileProjectModel.BinarySaveDirectoryInfo;
            projectContext.EntryClassName = zCompileProjectModel.EntryClassName;

            /*--------------------------------------------- 添加DLL -------------------------------------*/
            if (zCompileProjectModel.RefDllList != null && zCompileProjectModel.RefDllList.Count > 0)
            {
                foreach (var dll in zCompileProjectModel.RefDllList)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFile(dll.FullName);
                        projectContext.AddAssembly(asm);
                    }
                    catch (Exception ex)
                    {
                        Messager.Error("加载DLL文件"+dll.Name+"错误:"+ex.Message);
                    }
                }
            }

            /*--------------------------------------------- 添加Package -------------------------------------*/
            if (zCompileProjectModel.RefPackageList != null && zCompileProjectModel.RefPackageList.Count > 0)
            {
                foreach (var packageName in zCompileProjectModel.RefPackageList)
                {
                    projectContext.AddPackage(packageName);
                }
            }
            /*--------------------------------------------- 加载引用Class -------------------------------------*/
            projectContext.LoadRefTypes();

            CompileUtil.GenerateBinary(projectContext);
            /*--------------------------------------------- 编译类 -------------------------------------*/
            if (zCompileProjectModel.SouceFileList != null && zCompileProjectModel.SouceFileList.Count > 0)
            {
                foreach (ZCompileClassModel zCompileClassModle in zCompileProjectModel.SouceFileList)
                {
                    Type type = CompileClass(zCompileClassModle, projectContext, result);
                    if (type != null)
                    {
                        projectContext.LoadProjectType(type);
                    }
                    else
                    {
                        result.Errors.AddRange(Messager.Results.Errors);
                        result.Errors.AddRange(Messager.Results.Warnings);
                        Messager.Clear();
                    }
                }
            }
            /*--------------------------------------------- 设置入口点 -------------------------------------*/
            if (projectContext.BinaryFileKind!= PEFileKinds.Dll&& !string.IsNullOrEmpty(projectContext.EntryClassName))
            {
                var name = projectContext.EntryClassName;
                Type type = result.GetCompiledType(projectContext.EntryClassName);
                var file = projectContext.ProjectFileName;
                var i = -1;
                if (type == null)
                {
                    errorf(result, file, i + 1, 6, "入口类型'{0}'不存在", name);
                    //continue;
                }
                else
                {
                    MethodInfo main = type.GetMethod("启动");
                    if (main == null)
                    {
                        errorf(result, file, i + 1, 6, "入口类型'{0}'不存在'启动'过程", name);
                        //continue;
                    }
                    else if (!main.IsStatic)
                    {
                        errorf(result, file, i + 1, 6, "入口类型'{0}'不是唯一类型，不能作为启动入口", name);
                        //continue;
                    }
                    projectContext.EmitContext.AssemblyBuilder.SetEntryPoint(main, projectContext.BinaryFileKind);
                }
            }

            if (result.HasError() == false)
            {
                if (zCompileProjectModel.NeedSave)
                {
                    result.BinaryFilePath = saveBinaryFile(projectContext);
                }
            }
            return result;
        }

        private void errorf(ProjectCompileResult result, string file, int line, int col, string formatstring, params string[] args)
        {
            CompileMessage msg = new CompileMessage() { FileName = file, Line = line, Col = col, Text = string.Format(formatstring, args) };
            result.Errors.Add(msg);
        }

        private string saveBinaryFile(ProjectContext projectContext)
        {
            string binFileName = projectContext.GetBinaryNameEx();
            projectContext.EmitContext.AssemblyBuilder.Save(binFileName);
            CompileUtil.MoveBinary(projectContext);
            CompileUtil.DeletePDB(projectContext);
            string exBinFileName = projectContext.GetBinaryNameEx();
            string toFileFullPath = Path.Combine(projectContext.BinarySaveDirectoryInfo.FullName, exBinFileName);
            return toFileFullPath;
        }

        private Type CompileClass(ZCompileClassModel zCompileClassModel, ProjectContext projectContext, ProjectCompileResult result)
        {
            FileCompiler fc = new FileCompiler();
            Type type = fc.Compile(projectContext,zCompileClassModel);
            if (type != null)
            {
                //projectContext.LoadProjectType(type);
                result.CompiledTypes.Add(type);
            }
            else
            {
                result.Errors.AddRange(Messager.Results.Errors);
                result.Errors.AddRange(Messager.Results.Warnings);
                Messager.Clear();
            }
            return type;
        }
    }
}
