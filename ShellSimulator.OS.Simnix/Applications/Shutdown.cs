using System.Threading;
using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
	public class Shutdown : SimnixApplication
	{
		public override string Name => throw new System.NotImplementedException();

		protected override Task<int> Main(string[] args)
		{
			//new Thread(() =>
			//{
			RequestShutdown();
			//}).Start();

			return Task.FromResult<int>(0);
		}
	}
}