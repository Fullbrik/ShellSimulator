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

			MountFS("/proc", (os, name, parent) => new ProcessFileSystem(os, name, parent));

			InstallApplications();

			await CreateRootUser();

			return 0;
		}

		private void CreateDirectories()
		{
			MakeDirectory("/bin");
			MakeDirectory("/home");
			MakeDirectory("/tmp");
			MakeDirectory("/opt");
			MakeDirectory("/etc");
			MakeDirectory("/var");
			MakeDirectory("/var/log");
			MakeDirectory("/usr");
			MakeDirectory("/usr/local");
		}

		private void InstallApplications()
		{
			// Install Daemons
			InstallApplication<TerminalDaemon>("/bin/terminald");

			// Install Shell
			InstallApplication<Shell>("/bin/shell");
			InstallApplication<Shell>("/bin/sh");

			// Install posix utilities
			InstallApplication<Echo>("/bin/echo");
			InstallApplication<Clear>("/bin/clear");

			// Install other utilities
			InstallApplication<Shutdown>("/bin/shutdown");
			InstallApplication<UserAdd>("/bin/useradd");
			InstallApplication<Passwd>("/bin/passwd");
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