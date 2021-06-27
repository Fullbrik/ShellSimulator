namespace ShellSimulator.Applications
{
    public class Clear : Application
    {
        public Clear(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            return Shell.ClearScreen() ? 0 : -1;
        }
    }
}