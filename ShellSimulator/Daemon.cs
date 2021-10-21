namespace ShellSimulator
{
	public abstract class Daemon : Application
	{
		public abstract bool IsRunning { get; }

		public abstract void RequestStop();

		protected override void ReceiveSignal(ApplicationSignal signal)
		{
			throw new System.NotImplementedException();
		}
	}
}