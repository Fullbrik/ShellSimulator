using System.Threading;
using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class Shutdown : Application
	{
		public override string Name => throw new System.NotImplementedException();

		protected override Task<int> Main(string[] args)
		{
			new Thread(() =>
			{
				OS.Shutdown();
			}).Start();

			return Task.FromResult<int>(0);
		}
	}
}