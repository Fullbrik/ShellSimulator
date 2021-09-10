namespace ShellSimulator.Applications
{
    public class Exit : Application
    {
        public Exit(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            try
            {
                Parent.Exit();
            }
            catch (System.Exception e)
            {
                Printlnf("Error: {0}", e.Message);
                return 1;
            }

            return 0;
        }
    }
}