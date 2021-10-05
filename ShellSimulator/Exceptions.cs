namespace ShellSimulator
{
	[System.Serializable]
	public class ReadOnlyFileSystemException : System.Exception
	{
		public ReadOnlyFileSystemException() : base("Attempt to write to read only file system!") { }
		public ReadOnlyFileSystemException(string message) : base(message) { }
		public ReadOnlyFileSystemException(string message, System.Exception inner) : base(message, inner) { }
		protected ReadOnlyFileSystemException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[System.Serializable]
	public class InvalidFileNameException : System.Exception
	{
		public InvalidFileNameException() : base("Attempt to access file/directory with an invalid name.") { }
		public InvalidFileNameException(string message) : base(message) { }
		public InvalidFileNameException(string message, System.Exception inner) : base(message, inner) { }
		protected InvalidFileNameException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

	[System.Serializable]
	public class DirectoryNotFoundException : System.Exception
	{
		public DirectoryNotFoundException(string name, Directory parent) : base($"Unable to find directory {name} in directory {parent.FullPath}") { }
	}

	[System.Serializable]
	public class FileNotFoundException : System.Exception
	{
		public FileNotFoundException(string name, Directory parent) : base($"Unable to find file {name} in directory {parent.FullPath}") { }
	}
}