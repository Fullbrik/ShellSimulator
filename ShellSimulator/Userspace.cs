namespace ShellSimulator
{
	public class Userspace
	{
		public OperatingSystem OS { get; }
		public string Username { get; }
		public string HomeFolder { get; }

		public string CurrentWorkingDirectory { get; set; } = "~";

		public Userspace(OperatingSystem os, string username, string homeFolder)
		{
			OS = os;
			Username = username;
			HomeFolder = homeFolder;
		}


	}
}