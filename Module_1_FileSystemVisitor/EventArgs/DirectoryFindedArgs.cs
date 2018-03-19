using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_1_FileSystemVisitor.EventArgs
{
    public class DirectoryFindedArgs
    {
        public FileSystemInfo FileSystemInfo { get; internal set; }
    }
}
