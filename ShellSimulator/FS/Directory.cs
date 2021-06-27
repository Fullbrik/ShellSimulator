using System.Linq;
using System.Collections.Generic;
namespace ShellSimulator.FS
{
    public enum DirectoryType
    {
        Normal,
        Processes
    }

    internal class Directory
    {
        public DirectoryType DirectoryType { get; set; }

        private Dictionary<string, Directory> Directories { get; } = new Dictionary<string, Directory>();
        private Dictionary<string, File> Files { get; } = new Dictionary<string, File>();

        public Directory()
        {
            Directories.Add(".", this);
        }

        public Directory(Directory parent)
        {
            Directories.Add(".", this);
            Directories.Add("..", parent);
        }

        public Directory GetSub(string name, string fullDir)
        {
            if (Directories.ContainsKey(name))
            {
                var dir = Directories[name];
                if (dir == null) throw new PermissionDeniedException(fullDir);
                else return dir;
            }
            else
            {
                return null;
            }
        }

        public Directory AddSub(string name, bool isNull, DirectoryType directoryType)
        {
            Directory dir = (isNull) ? null : new Directory(this)
            {
                DirectoryType = directoryType
            };

            Directories.Add(name, dir);

            return dir;
        }

        public string[] GetSubNames() => Directories.Keys.ToArray();

        public string[] GetFileNames(Shell shell)
        {
            switch (DirectoryType)
            {
                case DirectoryType.Processes:
                    return shell.Processes.Select((a, i) => a.GetType().Name + i.ToString()).ToArray();
                case DirectoryType.Normal:
                default:
                    return Files.Keys.ToArray();
            }
        }

        public void AddFile(string name, File file, string path)
        {
            if (Files.ContainsKey(name)) throw new FileAlreadyExistsException(path);
            Files.Add(name, file);
        }

        public File GetFile(string name, Shell shell, string path)
        {
            switch (DirectoryType)
            {
                case DirectoryType.Processes:
                    throw new PermissionDeniedException(path); //You can't actually access these files.
                case DirectoryType.Normal:
                default:
                    if (!Files.ContainsKey(name)) throw new FileNotFoundException(path);
                    return Files[name];
            }

        }
    }
}