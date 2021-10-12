namespace ShellSimulator.Hardware
{
	public abstract class Device
	{
		public abstract void OnConnect(OperatingSystem os);

		public abstract void OnStart(OperatingSystem os);
	}
}