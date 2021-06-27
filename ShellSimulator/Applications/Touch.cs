using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Touch : Application
    {
        public Touch(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            try
            {
                foreach (var file in args)
                {
                    string path = FileSystem.RelativePathToAbsolutePath(file);
                    FileSystem.CreateUserFile(path);
                }

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