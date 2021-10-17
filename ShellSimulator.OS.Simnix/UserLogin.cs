using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
	public class UserLogin : Application
	{
		public override string Name => "User Login";

		public string InputUsername { get; private set; } = null;
		public string InputPassword { get; private set; } = null;

		private bool isJustCheckingUsername = false;

		protected override async Task<int> Main(string[] args)
		{
			var libUser = new Lib.LibUser(this);

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-u":
					case "-username":
						i++;
						InputUsername = args[i];
						break;
					case "-p":
					case "--password":
						i++;
						InputPassword = args[i];
						break;
					case "-C":
					case "--username-check":
						isJustCheckingUsername = true;
						break;
					default:
						break;
				}
			}

			if (InputUsername == null)
			{
				PrintF("Username: ");
				InputUsername = await ReadLine();
			}

			if (!isJustCheckingUsername && InputPassword == null) // We can skip if we are only checking the username
			{
				PrintF("Password: ");
				InputPassword = await ReadLine();
			}

			int result = DoCheck(libUser);

			switch (result)
			{
				case 0:
					break;
				case 1:
					PrintFLN("Invalid Username.");
					break;
				case 2:
					PrintFLN("Incorrect password.");
					break;
				default:
					PrintFLN("Unknown error.");
					break;
			}

			return result;
		}

		private int DoCheck(Lib.LibUser libUser)
		{
			if (isJustCheckingUsername)
			{
				return libUser.UserExists(InputUsername) ? 0 : 1; // One means invalid username
			}
			else
			{
				if (!libUser.UserExists(InputUsername)) return 1;
				else return 0;
			}
		}
	}
}