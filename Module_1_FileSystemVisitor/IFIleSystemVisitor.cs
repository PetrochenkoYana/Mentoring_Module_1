using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_1_FileSystemVisitor
{
    interface IFileSystemVisitor
    {
        List<FileSystemInfo> TraverseDirectoryTree();
        IEnumerable<FileSystemInfo> GetAllFilesAndDirectories(DirectoryInfo directory, int counter = 0);
    }
}
