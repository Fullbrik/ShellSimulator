namespace ShellSimulator.Applications
{
    public class Alias : Application
    {
        public Alias(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string errorMsg = "";

            if (args.Length == 1)
            {
                args = args[0].Split("=");

                if (args.Length == 2)
                {
                    DoAlias(args[0], args[1]);
                    return 0;
                }
                else
                {
                    errorMsg = "No = sign to split arg";
                    goto error;
                }
            }
            else if (args.Length == 3)
            {
                if (args[1] == "=")
                {
                    DoAlias(args[0], args[2]);
                    return 0;
                }
                else
                {
                    errorMsg = "Argument 2 must be an \"=\"";
                    goto error;
                }
            }
            else
            {
                goto error;
            }

        error:
            Printlnf("Invalid syntax. " + errorMsg);
            return -1;
        }

        private void DoAlias(string name, string value)
        {
            if (Shell.Aliases.ContainsKey(name))
            {
                Shell.Aliases[name] = value;
            }
            else
            {
                Shell.Aliases.Add(name, value);
            }
        }
    }
}