namespace ShellSimulator.FS
{
    public class FileAlreadyExistsException : FSException
    {
        public FileAlreadyExistsException(string path) : base($"Failed to create \"{path}\". File already exists.") { }
    }
}