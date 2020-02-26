using System;
using System.IO;
using System.Linq;
using FileAndDirectoryInfoExtensionsNamespace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileAndDirectoryInfoExtensionsTestsNamespace
{
    [TestClass]
    public class ExtensionsTests
    {
        public TestContext TestContext { get; set; }
        public DirectoryInfo TestDirectory => new DirectoryInfo(TestContext.TestRunDirectory);
        public DirectoryInfo DirWithOriginalData;

        [TestInitialize]
        public void Setup()
        {
            DirWithOriginalData = TestDirectory
                .CreateSubdirectory("DirectoryWithOriginals");

            DirWithOriginalData.CreateFile("TestFile1.txt");
            DirWithOriginalData.CreateFile("TestFile2.txt");
            DirWithOriginalData.CreateSubdirectory("SomeSubdir")
                .CreateFile("TestFile1InSubdir.txt");
        }

        [TestMethod]
        public void TestCopyContent()
        {
            var dirWithCopiedData = TestDirectory
                .CreateSubdirectory("DirectoryWithCopies");

            DirWithOriginalData.CopyContentTo(dirWithCopiedData);

            var copiedFiles = Directory
                .GetFiles(dirWithCopiedData.FullName, "*.*", SearchOption.AllDirectories)
                .Select(s => new FileInfo(s).Name)
                .ToList();
            var copiedDirectories = Directory
                .GetDirectories(dirWithCopiedData.FullName)
                .Select(s => new DirectoryInfo(s).Name)
                .ToList();

            Assert.IsTrue(copiedFiles.Contains("TestFile1.txt"));
            Assert.IsTrue(copiedFiles.Contains("TestFile2.txt"));
            Assert.IsTrue(copiedFiles.Contains("TestFile1InSubdir.txt"));
            Assert.AreEqual(3, copiedFiles.Count);

            Assert.IsTrue(copiedDirectories.Contains("SomeSubdir"));
            Assert.AreEqual(1, copiedDirectories.Count);
        }
    }
}
