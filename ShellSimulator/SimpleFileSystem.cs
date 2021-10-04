namespace ShellSimulator
{
	public class SimpleFileSystem : FileSystem
	{
		public SimpleFileSystem(OperatingSystem os, string name, Directory parent) : base(os, name, parent)
		{
		}

		public override bool IsReadOnlyFileSystem => false;

		public override void OnMount()
		{

		}
	}
}