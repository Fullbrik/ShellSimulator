namespace ShellSimulator.OS.Simnix
{
    public class SimnixRootFS : FileSystem
    {
        public SimnixRootFS(string name, Directory parent) : base(name, parent)
        {
        }

        public override bool IsReadOnlyFileSystem => true;


    }
}