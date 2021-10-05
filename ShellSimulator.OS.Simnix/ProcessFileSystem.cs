using System.Linq;

namespace ShellSimulator.OS.Simnix
{
	public class ProcessFileSystem : FileSystem
	{
		public ProcessFileSystem(OperatingSystem os, string name, Directory parent) : base(os, name, parent)
		{
		}

		public override bool IsReadOnlyFileSystem => true;

		public override void OnMount()
		{

		}

		public override string[] Files => OS.Processes.Select(proc => proc.Name).ToArray();
	}
}