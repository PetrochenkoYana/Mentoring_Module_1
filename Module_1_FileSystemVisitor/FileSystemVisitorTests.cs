using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace Module_1_FileSystemVisitor
{
    [TestFixture]
    class FileSystemVisitorTests
    {
        private FileSystemVisitor FileSystemVisitor { get; set; }
        private List<string> EventsList { get; set; }
        [SetUp]
        public void SetUp()
        {
            FileSystemVisitor = new FileSystemVisitor("//epbyminsa0000/Training Materials/EPAM Trainings/.NET Mentroring/2017/Q4_RU/D1-D2", (file) => file.Extension == ".mp4");
            EventsList = new List<string>();
            FileSystemVisitor.Start += (s, e) => EventsList.Add("Start");
            FileSystemVisitor.Finish += (s, e) => EventsList.Add("Finish");
            FileSystemVisitor.FileFinded += (s, e) => EventsList.Add("FileFinded " + e.FileSystemInfo);
            FileSystemVisitor.DirectoryFinded += (s, e) => EventsList.Add("DirectoryFinded " + e.FileSystemInfo);
            FileSystemVisitor.FilteredFileFinded += (s, e) => EventsList.Add("filterfile " + e.FileSystemInfo);
            FileSystemVisitor.FilteredDirectoryFinded += (s, e) => EventsList.Add("filterdirectory " + e.FileSystemInfo);
        }

        [Test]
        public void FilesIsNotNull()
        {
            int counter = 0;
            var files = FileSystemVisitor.GetAllFilesAndDirectories(FileSystemVisitor.Root, counter).ToList();
            Assert.NotNull(files);
            Assert.IsTrue(files.Count > 0);

        }
        [Test]
        public void FilteringByExtention()
        {
            var filteredFiles = FileSystemVisitor.TraverseDirectoryTree();
            Assert.NotNull(filteredFiles);
            foreach (var file in filteredFiles)
            {
                Assert.AreEqual(file.Extension, ".mp4");
            }

        }

        [Test]
        public void FileEventRaised()
        {
            var filteredFiles = FileSystemVisitor.TraverseDirectoryTree();
            var fileSystem =
                new FileInfo(
                    "//epbyminsa0000/Training Materials/EPAM Trainings/.NET Mentroring/2017/Q4_RU/D1-D2/01. Advanced C#/01. Introduction to types/Introduction to types.mp4");
            Assert.Contains("Start", EventsList);
            Assert.Contains("Finish", EventsList);
            Assert.Contains("FileFinded " + fileSystem.Name, EventsList);
            Assert.Contains("filterfile " + fileSystem.Name, EventsList);

        }
        [Test]
        public void DirectoryEventRaised()
        {
            var filteredFiles = FileSystemVisitor.TraverseDirectoryTree();
            var fileSystem =
                new DirectoryInfo(
                    "//epbyminsa0000/Training Materials/EPAM Trainings/.NET Mentroring/2017/Q4_RU/D1-D2/01. Advanced C#/01. Introduction to types");
            Assert.Contains("Start", EventsList);
            Assert.Contains("Finish", EventsList);
            Assert.Contains("DirectoryFinded " + fileSystem.Name, EventsList);
            Assert.Contains("filterdirectory " + fileSystem.Name, EventsList);

        }

        [Test]
        public void InterruptionSearching()
        {
            //Delegate stops searching when number of finded files more than 10
            FileSystemVisitor.FileFinded += (sender, args) =>
            {
                if (args.NumberOfFiles > 10)
                {
                    ((FileSystemVisitor)sender).StopSearch = true;
                }
                EventsList.Add("FileFinded " + args.FileSystemInfo);
            };

            FileSystemVisitor.FilterMethod = null;

            var filteredFiles = FileSystemVisitor.TraverseDirectoryTree();

            Assert.IsTrue(filteredFiles.Count <= 10);
        }

        [Test]
        public void ExcludeFiles()
        {
            //Exclude files that are archives
            FileSystemVisitor.FileFinded += (sender, args) =>
            {

                ((FileSystemVisitor)sender).ExcludeFile = (file) => (file.Attributes == FileAttributes.Archive);

                EventsList.Add("FileFinded " + args.FileSystemInfo);
            };

            FileSystemVisitor.FilterMethod = null;

            var filteredFiles = FileSystemVisitor.TraverseDirectoryTree();

            Assert.IsTrue((filteredFiles.Where(file => file.Attributes == FileAttributes.Archive)).ToList().Count == 0);
        }

    }
}
