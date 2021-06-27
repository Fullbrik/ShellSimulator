using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class CD : Application
    {
        public CD(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            if (args.Length > 0)
            {
                string newDir = FileSystem.RelativePathToAbsolutePath(args[0]);
                try
                {
                    FileSystem.AssetDirectoryAvailable(newDir);
                }
                catch (FSException e)
                {
                    Printlnf(e.Message);
                    return -1;
                }
                Shell.CWD = newDir;
                return 0;
            }
            else
            {
                Printlnf("Please specify a path to change to.");
                return -1;
            }
        }
    }
}