using System.Threading.Tasks;

namespace ShellSimulator
{
	public class ApplicationFile<T> : File
		where T : Application, new()
	{
		public ApplicationFile(string name, Directory parent) : base(name, parent)
		{
		}

		public override void ClearAllBytes()
		{
			throw new System.NotImplementedException();
		}

		public override void Close(Application application)
		{
			throw new System.NotImplementedException();
		}

		public override Task<int> Execute(Application parent, Application pipeTo, string[] args)
		{
			var os = Parent.OwningFileSystem.OS;

			return os.StartApplication(new T(), parent, pipeTo, args);
		}

		public override void Open(Application application)
		{
			throw new System.NotImplementedException();
		}

		public override byte ReadByte()
		{
			throw new System.NotImplementedException();
		}

		public override bool SetReadPosition(int position)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteByte(byte b)
		{
			throw new System.NotImplementedException();
		}
	}
}