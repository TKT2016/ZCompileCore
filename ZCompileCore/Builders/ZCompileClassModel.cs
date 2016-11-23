using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZCompileCore.Builders
{
    public class ZCompileClassModel
    {
        /// <summary>
        /// 项目实体
        /// </summary>
        public ZCompileProjectModel Project { get; set; }

        /// <summary>
        /// Z语言源文件
        /// </summary>
        public FileInfo SourceFileInfo { get; set; }

        /// <summary>
        /// 编译前在Z程序前的补充代码
        /// </summary>
        public string PreSourceCode { get; set; }
    }
}
