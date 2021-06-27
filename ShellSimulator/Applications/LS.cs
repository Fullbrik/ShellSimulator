using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class LS : Application
    {
        public LS(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string directory = Shell.CWD;
            if (args.Length > 0) directory = args[0];

            try
            {
                var dirs = FileSystem.GetSubDirectoryNames(directory);
                foreach (var dir in dirs)
                    Printlnf(dir);

                var files = FileSystem.GetFileNamesInDirectory(directory);
                foreach (var file in files)
                    Printlnf(file);
            }
            catch (FSException e)
            {
                Printlnf(e.Message);
                return -1;
            }

            return 0;
        }
    }
}