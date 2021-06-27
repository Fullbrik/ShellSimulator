using ShellSimulator.FS;

namespace ShellSimulator.OS
{
    public abstract class OS
    {
        public void Start(Shell shell)
        {
            Install(shell.FS);
            Setup(shell);
            StartRootProcess(shell);
        }

        public abstract void Install(FileSystem fs);

        public abstract void Setup(Shell shell);

        public abstract void StartRootProcess(Shell shell);
    }
}