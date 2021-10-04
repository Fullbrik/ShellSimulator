using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
    public class SimnixOS : OperatingSystem
    {
        public FileSystem RootFS { get; private set; }

        public override char PathSeperator => '/';

        public override void Install()
        {
            // Create and add the root fs
            RootFS = new SimpleFileSystem("/", null);
            AddRootFS("/", RootFS);
        }

        protected async override Task Init()
        {
            StartDaemon(new TerminalDaemon());
        }
    }
}