using System;
namespace ShellSimulator.FS
{
    public class PermissionDeniedException : FSException
    {
        public PermissionDeniedException(string path) : base($"Failed to access \"{path}\". Permission denied") { }
    }
}