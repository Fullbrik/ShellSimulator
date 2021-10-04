namespace ShellSimulator
{
    public abstract class Daemon : Application
    {
        public abstract bool IsRunning { get; }

        public abstract void RequestStop();
    }
}