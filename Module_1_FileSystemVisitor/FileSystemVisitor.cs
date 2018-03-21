using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module_1_FileSystemVisitor.EventArgs;

namespace Module_1_FileSystemVisitor
{
    public class FileSystemVisitor : IFileSystemVisitor
    {
        public DirectoryInfo Root { get; set; }
        public Func<FileSystemInfo, bool> FilterMethod { get; set; }
        public Func<FileSystemInfo, bool> ExcludeFile { get; set; }
        public bool StopSearch { get; set; }

        public event EventHandler<StartArgs> Start;
        public event EventHandler<FinishArgs> Finish;
        public event EventHandler<FileFindedArgs> FileFinded;
        public event EventHandler<DirectoryFindedArgs> DirectoryFinded;
        public event EventHandler<FilteredFileFindedArgs> FilteredFileFinded;
        public event EventHandler<FilteredDirectoryFindedArgs> FilteredDirectoryFinded;

        public FileSystemVisitor(string rootPath)
        {
            try
            {
                Root = new DirectoryInfo(rootPath);
            }
            catch (UnauthorizedAccessException e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public FileSystemVisitor(string rootPath, Func<FileSystemInfo, bool> filterMethod) : this(rootPath)
        {
            FilterMethod = filterMethod;
        }

        public List<FileSystemInfo> TraverseDirectoryTree()
        {
            int counter = 0;
            OnEvent(Start, new StartArgs());

            var allFilesAndDirectories = GetAllFilesAndDirectories(Root, counter).ToList();
            var filteredFilesAndDirectories = new List<FileSystemInfo>();

            if (FilterMethod != null)
            {
                foreach (var fileSystem in allFilesAndDirectories)
                {
                    if (FilterMethod(fileSystem))
                    {
                        filteredFilesAndDirectories.Add(fileSystem);
                    }
                    if ((fileSystem.Attributes & FileAttributes.Directory) != 0)
                    {
                        OnEvent(FilteredDirectoryFinded, new FilteredDirectoryFindedArgs() { FileSystemInfo = fileSystem });
                    }
                    else
                    {
                        OnEvent(FilteredFileFinded, new FilteredFileFindedArgs() { FileSystemInfo = fileSystem });
                    }
                }
            }
            else
            {
                filteredFilesAndDirectories = allFilesAndDirectories;
            }

            if (ExcludeFile != null)
            {
                filteredFilesAndDirectories = filteredFilesAndDirectories.Where(file => ExcludeFile(file) != true).ToList();
            }
            OnEvent(Finish, new FinishArgs());
            StopSearch = false;
            return filteredFilesAndDirectories;

        }

        public IEnumerable<FileSystemInfo> GetAllFilesAndDirectories(DirectoryInfo directory, int counter)
        {
            foreach (var fileSystem in directory.GetFiles())
            {
                if (StopSearch)
                {
                    yield break;
                }
                counter++;
                OnEvent(FileFinded, new FileFindedArgs() { FileSystemInfo = fileSystem, NumberOfFiles = counter });
                yield return fileSystem;
            }

            foreach (var directorySystem in directory.GetDirectories())
            {
                if (StopSearch)
                {
                    yield break;
                }
                counter++;
                OnEvent(DirectoryFinded, new DirectoryFindedArgs() { FileSystemInfo = directorySystem });
                yield return directorySystem;

                foreach (var files in GetAllFilesAndDirectories(directorySystem, counter))
                {
                    if (StopSearch)
                    {
                        yield break;
                    }
                    counter++;
                    yield return files;
                }
            }
        }

        protected virtual void OnEvent<TArgs>(EventHandler<TArgs> eventhandler, TArgs args)
        {
            eventhandler?.Invoke(this, args);
        }

    }
}

