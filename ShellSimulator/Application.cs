using System;
using ShellSimulator.FS;

namespace ShellSimulator
{
    public abstract class Application
    {
        public Shell Shell { get; }
        public FileSystem FileSystem { get => Shell.FS; }

        public Application(Shell shell)
        {
            Shell = shell;
        }

        public Application Parent { get; set; }

        public int Start(Application parent, params string[] args)
        {
            Parent = parent;
            Shell.OnStartProcess(this);
            int result = Main(args);
            Shell.OnEndProcess(this);
            return result;
        }

        protected abstract int Main(string[] args);

        #region STDIO API
        protected void Printf(string str, params object[] args)
        {
            str = string.Format(str, args); //Format the string first first
            Shell.STDOut.Write(str);
        }

        protected void Printlnf(string str, params object[] args)
        {
            if (args.Length > 0)
                str = string.Format(str, args); //Format the string first if we have any args to format with
            Shell.STDOut.WriteLine(str);
        }

        protected string ReadLine()
        {
            return Shell.STDIn.ReadLine();
        }
        #endregion

        #region Application Management API
        protected int StartApplication(Application application, params string[] args)
        {
            return application.Start(this, args);
        }
        #endregion
    }
}