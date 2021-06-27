namespace ShellSimulator.Applications
{
    public class WhoAmI : Application
    {
        public WhoAmI(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            Printlnf(Shell.Username);
            return 0;
        }
    }
}