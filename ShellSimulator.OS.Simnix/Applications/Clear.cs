using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class Clear : Application
	{
		public override string Name => "Clear";

		protected override Task<int> Main(string[] args)
		{
			ClearScreen();

			return Task.FromResult(0);
		}
	}
}