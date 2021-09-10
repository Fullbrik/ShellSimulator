using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ShellSimulator.FS
{
    public class FileSystem
    {
        public Shell Shell { get; }

        private Directory Root { get; } = new Directory();

        public FileSystem(Shell shell)
        {
            Shell = shell;
        }

        #region Path
        public string RelativePathToAbsolutePath(string path)
        {
            if (IsPathAbsolute(path))
                return path;
            else
                return AppendPaths(Shell.CWD, path);
        }

        public bool IsPathAbsolute(string path)
        {
            return path.StartsWith("/");
        }

        public string AppendPaths(params string[] paths)
        {
            string final = "";
            if (paths[0].StartsWith("/")) final += "/"; //We add the / here because if it is there, it will get removed

            foreach (string p in paths)
            {
                string path = p; //So we can do some mods on the iterator var.

                if (path.StartsWith("/")) path = path.Remove(0, 1); // We should already have a /, so a second one would cause errors.

                final += path;

                if (!final.EndsWith("/")) final += "/"; // Complete what we said above
            }

            return final;
        }

        public string FlattenPath(string path)
        {
            var split = SplitPath(path);

            Stack<string> finalStack = new Stack<string>();

            foreach (var part in split)
            {
                switch (part)
                {
                    case ".": //Ignore any "."s b/c they just mean current directory
                        break;
                    case "..": //The ".." means to go back one directory, so we go back one on the stack
                        finalStack.Pop();
                        break;
                    default:
                        finalStack.Push(part); //If it's just a normal part of a path, just push it onto the stack
                        break;
                }
            }

            return string.Join('/', finalStack.Reverse());
        }

        private string[] SplitPath(string path)
        {
            if (path == "/") return new string[] { "" };
            return path.Split('/');
        }
        #endregion

        #region Directory
        public int CreateDirectory(string directory, DirectoryType directoryType = DirectoryType.Normal)
        {
            return CreateDirectory(directory, true, directoryType);
        }

        public int CreateNullDirectory(string directory)
        {
            return CreateDirectory(directory, false, DirectoryType.Normal);
        }

        private int CreateDirectory(string directory, bool hasPermission, DirectoryType directoryType)
        {
            int directoriesCreatedCount = 0;

            directory = RelativePathToAbsolutePath(directory);
            var path = SplitPath(directory);

            Directory current = null;

            int i = 0;
            foreach (var dir in path)
            {
                if (current == null)
                {
                    current = Root;
                }
                else if (!string.IsNullOrEmpty(dir)) //Skip if we get an empty part
                {
                    Directory nextDir = current.GetSub(dir, directory);
                    if (nextDir != null)
                    {
                        current = nextDir;
                    }
                    else
                    {
                        directoriesCreatedCount++;

                        current = current.AddSub(dir, !hasPermission && i == path.Length - 1, directoryType);
                    }
                }

                i++;
            }

            return directoriesCreatedCount;
        }

        public string[] GetSubDirectoryNames(string directory)
        {
            Directory dir = GetDirectory(directory);
            if (dir == null) throw new Exception("What??");
            return dir.GetSubNames();
        }

        public string[] GetFileNamesInDirectory(string directory)
        {
            Directory dir = GetDirectory(directory);
            if (dir == null) throw new Exception("What??");
            return dir.GetFileNames(Shell);
        }

        public void AssetDirectoryAvailable(string directory)
        {
            GetDirectory(directory);
        }

        public bool DoesDirectoryExist(string directory)
        {
            string endedOnPart = "";
            try
            {
                GetDirectory(directory, ref endedOnPart);
                return true;
            }
            catch (PermissionDeniedException)
            {
                if (directory.EndsWith(endedOnPart) || directory.EndsWith(endedOnPart + "/")) return true;
                else return false;
            }
            catch (FSException)
            {
                return false;
            }
        }

        private Directory GetDirectory(string path)
        {
            string s = ""; //Needed for the ref
            return GetDirectory(path, ref s);
        }

        private Directory GetDirectory(string[] pathSplit, string path = null)
        {
            string s = ""; //Needed for the ref
            return GetDirectory(pathSplit, ref s, path);
        }

        private Directory GetDirectory(string path, ref string endedOnPart)
        {
            path = RelativePathToAbsolutePath(path);
            var pathSplit = SplitPath(path);

            return GetDirectory(pathSplit, ref endedOnPart, path);
        }

        private Directory GetDirectory(string[] pathSplit, ref string endedOnPart, string path = null)
        {
            if (path == null) path = string.Join('/', pathSplit);

            Directory current = null;

            foreach (var dir in pathSplit)
            {
                endedOnPart = dir;
                if (current == null)
                {
                    current = Root;
                }
                else if (!string.IsNullOrEmpty(dir)) //Skip if we get an empty part
                {
                    current = current.GetSub(dir, path);
                    if (current == null) throw new DirectoryNotFoundException(path);
                }
            }

            return current;
        }
        #endregion

        #region File
        public void CreateUserFile(string path)
        {
            CreateFile(new UserCreatedFile(), path);
        }

        public void InstallApplication(string path, Func<Shell, Application> builder)
        {
            var file = new ApplicationFile(builder);
            CreateFile(file, path);
        }

        public int ExecuteFileOnPath(string path, Application sender, System.IO.TextWriter stdout, params string[] args)
        {
            var PATH = Shell.Path;

            foreach (var part in PATH)
            {
                File file = null;

                try { file = GetFile(AppendPaths(part, path)); }
                catch (FSException) { } //Ignore any fs exceptions because we just want to move on if the file is inaccessible

                if (file != null)
                    return file.Execute(Shell, sender, stdout, args);
            }

            throw new CommandNotFoundException(path);
        }

        public int ExecuteFile(string path, Application sender, params string[] args)
        {
            return ExecuteFile(path, sender, null, args);
        }

        public int ExecuteFile(string path, Application sender, System.IO.TextWriter stdout, params string[] args)
        {
            File file = null;

            try
            {
                file = GetFile(path);
            }
            catch (System.Exception)
            {
                throw new CommandNotFoundException(path);
            }

            if (file != null)
                return file.Execute(Shell, sender, stdout, args);
            else
                throw new CommandNotFoundException(path);
        }

        private void CreateFile(File file, string path)
        {
            var dir = GetDirectoryAndFileName(path, out string fileName);

            dir.AddFile(fileName, file, path);
        }

        public bool DoesFileExist(string path)
        {
            try
            {
                GetFile(path);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public string[] ReadAllFileLines(string path)
        {
            return ReadAllFileText(path).Split('\n');
        }

        public string ReadAllFileText(string path)
        {
            return Encoding.UTF8.GetString(ReadAllFileBytes(path));
        }

        public byte[] ReadAllFileBytes(string path)
        {
            var file = GetFile(path);
            return file.Read();
        }

        public void WriteAllFileLines(string path, string[] lines)
        {
            WriteAllFileText(path, string.Join('\n', lines));
        }

        public void WriteAllFileText(string path, string text)
        {
            WriteAllFileBytes(path, Encoding.UTF8.GetBytes(text));
        }

        public void WriteAllFileBytes(string path, byte[] bytes)
        {
            File file = null;

            //Try to find the already existing file, otherwise create one.
            try
            {
                file = GetFile(path);
            }
            catch (FSException)
            {
                file = new UserCreatedFile();
                CreateFile(file, path);
            }

            file.Write(bytes);
        }

        private File GetFile(string path)
        {
            var dir = GetDirectoryAndFileName(path, out string fileName);

            return dir.GetFile(fileName, Shell, path);
        }

        private Directory GetDirectoryAndFileName(string path, out string fileName)
        {
            path = RelativePathToAbsolutePath(path);
            var split = SplitPath(path);
            if (string.IsNullOrEmpty(split[split.Length - 1])) split = split.ToList().DoChained((l) => l.RemoveAt(l.Count - 1)).ToArray(); //If the last part is empty, remove it
            fileName = split[split.Length - 1];
            var directoryPath = split.ToList().DoChained((l) => l.RemoveAt(l.Count - 1)).ToArray();

            return GetDirectory(directoryPath);
        }
        #endregion
    }
}