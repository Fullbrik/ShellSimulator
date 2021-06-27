using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class MKDir : Application
    {
        public MKDir(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string path = "";
            bool ignoreExists = false;

            if (args.Length == 1)
            {
                path = FileSystem.RelativePathToAbsolutePath(args[0]);
            }
            else if (args.Length >= 2)
            {
                if (args[0].StartsWith('-'))
                {
                    ignoreExists = args[0].Contains("p");
                    path = FileSystem.RelativePathToAbsolutePath(args[1]);
                }
                else if (args[1].StartsWith('-'))
                {
                    ignoreExists = args[1].Contains("p");
                    path = FileSystem.RelativePathToAbsolutePath(args[0]);
                }
                else
                {
                    Printlnf("Unable to find any options, despite having enough args for them.");
                    return -1;
                }
            }
            else
            {
                Printlnf("Please provide the directory you want to create, along with (optionally) any options (IE: -p).");
                return -1;
            }

            if (!ignoreExists && FileSystem.DoesDirectoryExist(path))
            {
                Printlnf("Path {0} already exists", path);
                return -1;
            }

            try
            {
                FileSystem.CreateDirectory(path);
                return 0;
            }
            catch (FSException e)
            {
                Printlnf(e.Message);
                return -1;
            }
        }
    }
}