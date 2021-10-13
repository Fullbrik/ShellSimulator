namespace ShellSimulator.OS.Simnix.Lib
{
	public class LibUser : Library
	{
		public LibUser(Application application) : base(application)
		{
		}

		public override string Name => "User";

		public bool HasUser(string username)
		{

		}

		private string ReadUserDataFile()
		{
			// Get the text of the file and close it right away
			var file = Application.OS.OpenFile("/etc/passwd", Application);
			string text = file.ReadAllText();
			file.Close(Application);

			return text;
		}

		private UserData[] ParseUserData(string text)
		{

		}
	}
}