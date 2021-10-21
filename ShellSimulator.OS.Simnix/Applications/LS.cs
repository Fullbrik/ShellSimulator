using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class LS : SimnixApplication
	{
		public override string Name => "LS";

		protected override Task<int> Main(string[] args)
		{
			return Task.Run<int>(() =>
			{
				string path = (args.Length > 0) ? args[0] : "";

				var subFolders = GetAllSubDirectories(path);

				foreach (var folder in subFolders)
					PrintFLN(folder);

				var subFiles = GetAllFilesInDirectory(path);

				foreach (var file in subFiles)
					PrintFLN(file);

				return 0;
			});
		}
	}
}