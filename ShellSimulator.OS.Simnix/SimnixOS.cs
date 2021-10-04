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
			RootFS = new SimpleFileSystem(this, "/", null);
			AddRootFS("/", RootFS);

			MakeDirectory("/home");
			MakeDirectory("/tmp");
			MakeDirectory("/opt");
			MakeDirectory("/etc");
			MakeDirectory("/var");
			MakeDirectory("/var/log");
			MakeDirectory("/usr");
			MakeDirectory("/usr/local");

			MountFS("/proc", (os, name, parent) => new ProcessFileSystem(os, name, parent));
		}

		protected async override Task Init()
		{
			StartDaemon(new TerminalDaemon());
		}
	}
}