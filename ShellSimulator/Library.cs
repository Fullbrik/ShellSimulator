namespace ShellSimulator
{
	public abstract class Library
	{
		public abstract string Name { get; }

		public Application Application { get; }

		public Library(Application application)
		{
			Application = application;
		}
	}
}