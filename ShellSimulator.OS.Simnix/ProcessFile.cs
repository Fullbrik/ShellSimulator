using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
	public class ProcessFile : File
	{
		public ProcessFile(string name, Directory parent) : base(name, parent)
		{
		}

		public override void Close(Application application)
		{
		}

		public override Task<int> Execute(Application parent, Application pipeTo, string[] args)
		{
			throw new System.NotImplementedException();
		}

		public override void Open(Application application)
		{
		}

		public override byte ReadByte()
		{
			return 0x00;
		}

		public override bool SetReadPosition(int position)
		{
			return position == 0;
		}

		public override void WriteByte(byte b)
		{
			throw new System.NotImplementedException();
		}
	}
}