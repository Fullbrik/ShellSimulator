using System;
namespace ShellSimulator.FS
{
    public class DirectoryNotFoundException : FSException
    {
        public DirectoryNotFoundException(string path) : base($"Failed to access \"{path}\". Directory not found.") { }
    }
}