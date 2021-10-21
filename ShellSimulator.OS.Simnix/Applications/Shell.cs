using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class Shell : SimnixApplication
	{
		public bool IsRunning { get => IsOSRunning && isRunning; }
		private bool isRunning;

		public override string Name => "Shell";

		protected override async Task<int> Main(string[] args)
		{
			ClearScreen();

			isRunning = true;
			while (IsRunning)
			{
				PrintF("{0}@{1}: {2}$ ", Username, "HostName", CurrentWorkingDirectory);
				string command = await ReadLine();

				switch (command)
				{
					case "clear":
						ClearScreen();
						break;
					case "username":
						PrintFLN(Username);
						break;
					case "home":
						PrintFLN(HomeFolder);
						break;
					case "ls":
						await StartApplication(new LS(), PipeTo);
						break;
					case "cd":
						PrintF("Enter the new cwd: ");
						await StartApplication(new CD(), PipeTo, await ReadLine());
						break;
					case "exit":
						isRunning = false;
						break;
					case "shutdown":
						isRunning = false;
						await StartApplication(new ShellSimulator.OS.Simnix.Applications.Shutdown(), PipeTo);
						break;
					default:
						PrintFLN("Command {0} not found", command);
						break;
				}

			}

			return 0;
		}
	}
}