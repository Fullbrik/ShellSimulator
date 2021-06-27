namespace ShellSimulator.FS
{
    public class FileNotFoundException : FSException
    {
        public FileNotFoundException(string path) : base($"Failed to access \"{path}\". File not found.") { }
    }
}