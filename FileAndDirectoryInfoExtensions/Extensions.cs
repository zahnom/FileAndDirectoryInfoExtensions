using PathBuilderNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileAndDirectoryInfoExtensionsNamespace
{
    public static class Extensions
    {
        public static DirectoryInfo SelectDirectory(this DirectoryInfo dir, string path)
        {
            var currentDir = dir.FullName;

            var resultingPath = PathBuilder.CreateAbsolutePath()
                .StartAt(currentDir)
                .GoTo(path)
                .UseBackslashes()
                .TerminateDirsWithSlash()
                .TrimSlashes()
                .Create();

            var selectedDir = new DirectoryInfo(resultingPath);
            if (selectedDir.Exists == false) throw new DirectoryNotFoundException($"Directory '{resultingPath}' does not exist!");

            return selectedDir;
        }
        public static FileInfo SelectFile(this DirectoryInfo dir, string path)
        {
            var currentDir = dir.FullName;

            var resultingPath = PathBuilder.CreateAbsolutePath()
                .StartAt(currentDir)
                .GoTo(path)
                .UseBackslashes()
                .TerminateDirsWithSlash()
                .TrimSlashes()
                .Create();

            var selectedFile = new FileInfo(resultingPath);
            if (selectedFile.Exists == false) throw new FileNotFoundException($"File '{resultingPath}' does not exist!");

            return selectedFile;
        }
        public static List<FileInfo> SelectFilesInCurrentOnly(this DirectoryInfo dir, string searchPattern = "*.*")
        {
            var currentDir = dir.FullName;

            var resultingPath = PathBuilder.FormatPath(currentDir)
                .UseBackslashes()
                .TerminateDirsWithSlash()
                .TrimSlashes()
                .Format();

            string[] files = Directory.GetFiles(resultingPath, searchPattern);
            return new List<string>(files)
                .Select(p => new FileInfo(p))
                .ToList();
        }
        public static List<FileInfo> SelectFilesInCurrentRecursively(this DirectoryInfo dir, string searchPattern = "*.*")
        {
            var currentDir = dir.FullName;

            var resultingPath = PathBuilder.FormatPath(currentDir)
                .UseBackslashes()
                .TerminateDirsWithSlash()
                .TrimSlashes()
                .Format();

            string[] files = Directory.GetFiles(resultingPath, searchPattern, SearchOption.AllDirectories);
            return new List<string>(files)
                .Select(p => new FileInfo(p))
                .ToList();
        }
        public static DirectoryInfo CreateCopyAt(this DirectoryInfo dirToCopy, DirectoryInfo destinationDir)
        {
            var destination = destinationDir.CreateSubdirectory(dirToCopy.Name);
            CopyAllFiles(dirToCopy, destination);
            CopyAllSubfolders(dirToCopy, destination);

            return destination;
        }
        public static DirectoryInfo CreateCopyAt(this DirectoryInfo dirToCopy, string destinationPath)
        {
            var destinationDir = Directory.CreateDirectory(destinationPath);
            return dirToCopy.CreateCopyAt(destinationDir);
        }
        public static DirectoryInfo CopyContentTo(this DirectoryInfo dirToCopy, DirectoryInfo destinationDir)
        {
            CopyAllFiles(dirToCopy, destinationDir);
            CopyAllSubfolders(dirToCopy, destinationDir);

            return destinationDir;
        }
        public static DirectoryInfo CopyContentTo(this DirectoryInfo dirToCopy, string destinationDir)
        {
            return dirToCopy.CopyContentTo(new DirectoryInfo(destinationDir));
        }
        public static FileInfo CreateFile(this DirectoryInfo dirContainingNewFile, string fileName)
        {
            using (File.Create(dirContainingNewFile.FullName + @"\" + fileName));
            return new FileInfo(dirContainingNewFile.FullName + @"\" + fileName);
        }

        private static void CopyAllFiles(DirectoryInfo resourcesDir, DirectoryInfo testDir)
        {
            var resourceFiles = resourcesDir.GetFiles();
            foreach (var file in resourceFiles)
            {
                var pathToOriginal = file.FullName;
                var pathToNew = testDir.FullName + "\\" + file.Name;
                File.Copy(pathToOriginal, pathToNew, overwrite: true);
            }
        }
        private static void CopyAllSubfolders(DirectoryInfo resourcesDir, DirectoryInfo testDir)
        {
            foreach (DirectoryInfo dir in resourcesDir.GetDirectories())
                CopyAllSubfolders(dir, testDir.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in resourcesDir.GetFiles())
            {
                try
                {
                    file.CopyTo(Path.Combine(testDir.FullName, file.Name));
                }
                catch (IOException) { };
            }

        }
    }
}
