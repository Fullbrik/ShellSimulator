using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class UserAdd : SimnixApplication
	{
		public override string Name => "User Add";

		private string username = null;
		private bool createHomeFolder = false;
		private string homeFolder = null;

		protected override Task<int> Main(string[] args)
		{
			var libUser = new Lib.LibUser(this);

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-m":
						createHomeFolder = true;
						break;
					case "-d":
						i++;
						homeFolder = args[i];
						break;
					default:
						if (username == null && !args[i].StartsWith("-"))
							username = args[i];
						else
							PrintFLN("Invalid option {0}", args[0]);
						break;
				}
			}

			if (username == null)
			{
				PrintFLN("Must pass in a username");
				return Task.FromResult(-1);
			}
			else
			{
				if (createHomeFolder)
				{
					// If we didn't supply a home folder, use the default one
					if (string.IsNullOrEmpty(homeFolder))
					{
						homeFolder = "/home/" + username;
					}

					MakeDirectory(homeFolder, true);
				}
			}

			return Task.FromResult(libUser.CreateUser(username, homeFolder, "/bin/sh") ? 0 : 1);
		}
	}
}