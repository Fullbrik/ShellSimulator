using System.Collections.Generic;
using System.Linq;

namespace ShellSimulator
{
    public class Directory
    {
        public string Name { get; }
        public Directory Parent { get; }
        public virtual FileSystem OwningFileSystem { get => Parent.OwningFileSystem; }

        public virtual string[] SubDirectories { get => subDirectories.Keys.ToArray(); }
        private readonly Dictionary<string, Directory> subDirectories = new Dictionary<string, Directory>();

        public Directory(string name, Directory parent)
        {
            Name = name;
            Parent = parent;
        }
    }

    public abstract class FileSystem : Directory
    {
        protected FileSystem(string name, Directory parent) : base(name, parent)
        {
        }

        public override FileSystem OwningFileSystem => this;

        public abstract bool IsReadOnlyFileSystem { get; }
    }
}