using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class CD : SimnixApplication
	{
		public override string Name => "CD";

		protected override Task<int> Main(string[] args)
		{
			return Task.Run<int>(() =>
			{
				string path = "";

				if (args.Length == 1)
				{
					path = args[0];
				}
				else
				{
					PrintFLN("Please provide a path to change to.");
					return 1;
				}

				string fullPath = GetFullPath(args[0]);

				CurrentWorkingDirectory = fullPath;

				return 0;
			});
		}
	}
}