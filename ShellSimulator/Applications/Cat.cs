using System.Linq;
using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Cat : Application
    {
        public Cat(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string text = "";

            try
            {
                foreach (var arg in args)
                {
                    string fileName = FileSystem.RelativePathToAbsolutePath(arg);
                    string fileText = FileSystem.ReadAllFileText(fileName);
                    text += fileText + "\n";
                }

                Printlnf(text);
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