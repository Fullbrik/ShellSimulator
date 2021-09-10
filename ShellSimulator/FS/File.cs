using System.IO;
namespace ShellSimulator.FS
{
    public abstract class File
    {
        public abstract bool IsExecutable { get; }
        public abstract bool CanOpenStream { get; }
        public abstract bool CanRead { get; }
        public abstract bool CanWrite { get; }

        public abstract Stream OpenStream();

        public abstract byte[] Read();

        public abstract void Write(byte[] bytes);

        public int Execute(Shell shell, Application sender, System.IO.TextWriter STDOut, params string[] args)
        {
            if (IsExecutable) return OnExecute(shell, sender, STDOut ?? shell.STDOut, args);
            else shell.STDOut.WriteLine("Cannot execute file.");

            return 20567;
        }

        protected abstract int OnExecute(Shell shell, Application sender, System.IO.TextWriter STDOut, string[] args);
    }
}