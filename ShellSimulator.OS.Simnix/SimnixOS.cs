using System.Threading.Tasks;
using ShellSimulator.OS.Simnix.Applications;

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

			var task = StartApplication(new Installer(), null, null);
			task.Wait();
		}



		public override async Task<int> StartUserSession(string username, string password, Application pipeTo)
		{
			// Start a user login task to verify the user. This will allow us to use LibUser to make our lives easier
			int result = await StartApplication(new UserLogin(), null, null, "-u", username, "-p", password);

			if (result == 0)
				return await StartApplication(new SimnixUserSession(username), null, null);
			else
				return int.MinValue;
		}

		protected async override Task Init()
		{
			// Create the terminal daemon
			var terminal = new TerminalDaemon();
			StartDaemon(terminal);

			while (IsRunning) // We will do this until we end
			{
				// Clear the screen
				await StartApplication(new Clear(), null, terminal);

				// Prompt the user to login
				var login = new UserLogin();
				terminal.PipeTo = login;
				int result = await StartApplication(login, null, terminal);

				// Get the login details
				string username = login.InputUsername;
				string password = login.InputPassword;

				// Login
				int sessionResult = await StartUserSession(username, password, terminal);
				if (sessionResult != 0 && IsRunning) await Task.Delay(5000); // If we got an error, hold for 5 seconds so the user can read any output.

				// Wait for the terminal buffer to finnish before continueing
				await terminal.WaitForBuffer();
			}
		}
	}
}