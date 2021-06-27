namespace ShellSimulator.FS
{
    public class CommandNotFoundException : FSException
    {
        public CommandNotFoundException(string path) : base($"Command \"{path}\" not found.") { }
    }
}