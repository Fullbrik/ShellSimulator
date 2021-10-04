namespace ShellSimulator
{
    public class SimpleFileSystem : FileSystem
    {
        public SimpleFileSystem(string name, Directory parent) : base(name, parent)
        {
        }

        public override bool IsReadOnlyFileSystem => false;
    }
}