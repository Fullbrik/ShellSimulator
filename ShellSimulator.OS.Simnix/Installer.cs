using System.Threading.Tasks;
using ShellSimulator.OS.Simnix.Applications;

namespace ShellSimulator.OS.Simnix
{
	public class Installer : Application
	{
		public override string Name => "Installer";

		protected async override Task<int> Main(string[] args)
		{
			CreateDirectories();

			OS.MountFS("/proc", (os, name, parent) => new ProcessFileSystem(os, name, parent));

			InstallApplications();

			await CreateRootUser();

			return 0;
		}

		private void CreateDirectories()
		{
			OS.MakeDirectory("/bin");
			OS.MakeDirectory("/home");
			OS.MakeDirectory("/tmp");
			OS.MakeDirectory("/opt");
			OS.MakeDirectory("/etc");
			OS.MakeDirectory("/var");
			OS.MakeDirectory("/var/log");
			OS.MakeDirectory("/usr");
			OS.MakeDirectory("/usr/local");
		}

		private void InstallApplications()
		{
			// Install Daemons
			OS.InstallApplication<TerminalDaemon>("/bin/terminald");

			// Install Shell
			OS.InstallApplication<Shell>("/bin/shell");
			OS.InstallApplication<Shell>("/bin/sh");

			// Install posix utilities
			OS.InstallApplication<Echo>("/bin/echo");
			OS.InstallApplication<Clear>("/bin/clear");

			// Install other utilities
			OS.InstallApplication<Shutdown>("/bin/shutdown");
			OS.InstallApplication<UserAdd>("/bin/useradd");
			OS.InstallApplication<Passwd>("/bin/passwd");
		}

		private async Task CreateRootUser()
		{
			var libUser = new Lib.LibUser(this);

			var result = await StartApplication(new UserAdd(), null, "-m", "-d", "/root", "root");

			if (result != 0) throw new System.Exception("Error creating root user");

			if (!libUser.SetUserPassword("root", "root")) throw new System.Exception("Error setting password for root user");
		}
	}
}