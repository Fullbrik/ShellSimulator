using System.Collections.Generic;
using System.Linq;

namespace ShellSimulator
{
	public abstract class FileSystem : Directory
	{
		public OperatingSystem OS { get; }

		protected FileSystem(OperatingSystem os, string name, Directory parent) : base(name, parent)
		{
			OS = os;
		}

		public override FileSystem OwningFileSystem => this;

		public abstract bool IsReadOnlyFileSystem { get; }

		public bool IsRootFS { get => !string.IsNullOrEmpty(RootPrefix); }
		public string RootPrefix { get; set; } = null;

		public override string FullPath => IsRootFS ? RootPrefix : base.FullPath;

		public abstract void OnMount();
	}
}