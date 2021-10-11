namespace ShellSimulator
{
    public class Userspace
    {
        public OperatingSystem OS { get; }

        public string Username { get; }

        public Userspace(OperatingSystem os, string username, string homeFolder)
        {
            OS = os;
        }
    }
}