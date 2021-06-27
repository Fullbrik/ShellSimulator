namespace ShellSimulator.Applications
{
    public class Export : Application
    {
        public Export(Shell shell) : base(shell)
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
                    DoExport(args[0], args[1]);
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
                    DoExport(args[0], args[2]);
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

        private void DoExport(string name, string value)
        {
            name = name.ToUpper();

            if (Shell.Variables.ContainsKey(name))
            {
                Shell.Variables[name] = value;
            }
            else
            {
                Shell.Variables.Add(name, value);
            }
        }
    }
}