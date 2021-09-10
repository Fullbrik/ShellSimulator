using System.Linq;
using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Echo : Application
    {
        public Echo(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string text = "";

            foreach (var arg in args)
            {
                text += arg + " ";
            }

            Printlnf(text);

            return 0;
        }
    }
}