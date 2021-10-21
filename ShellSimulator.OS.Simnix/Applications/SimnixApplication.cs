namespace ShellSimulator.OS.Simnix.Applications
{
	public abstract class SimnixApplication : Application
	{
		protected override void ReceiveSignal(ApplicationSignal signal)
		{
			throw new System.NotImplementedException();
		}
	}
}