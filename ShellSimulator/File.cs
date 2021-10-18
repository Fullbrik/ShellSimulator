using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShellSimulator
{
	public abstract class File
	{
		public string Name { get; }
		public Directory Parent { get; }

		public File(string name, Directory parent)
		{
			Name = name;
			Parent = parent;
		}

		public abstract void Open(Application application);
		public abstract void Close(Application application);

		public string ReadAllText()
		{
			string str = "";

			for (int i = 0; SetReadPosition(i); i++)
			{
				str += ReadChar();
			}

			return str;
		}

		public char ReadChar()
		{
			return Convert.ToChar(ReadByte());
		}



		public abstract byte ReadByte();
		public abstract bool SetReadPosition(int position);

		public void WriteAllText(string text)
		{
			SetReadPosition(0);

			ClearAllBytes();

			var bytes = Encoding.UTF8.GetBytes(text);

			for (int i = 0; i < bytes.Length; i++)
			{
				SetReadPosition(i);
				WriteByte(bytes[i]);
			}
		}

		public void WriteChar(char c)
		{
			WriteByte(Convert.ToByte(c));
		}

		public abstract void WriteByte(byte b);

		public abstract void ClearAllBytes();

		public abstract Task<int> Execute(Application parent, Application pipeTo, string[] args);
	}

	public class TextFile : File
	{
		public List<byte> Bytes { get; } = new List<byte>();

		public TextFile(string name, Directory parent) : base(name, parent)
		{
		}

		public bool IsOpen { get => OpenApplication != null; }
		public Application OpenApplication { get; private set; } = null;

		int readPosition = 0;

		public override void Open(Application application)
		{
			if (!IsOpen)
				OpenApplication = application;
		}

		public override void Close(Application application)
		{
			if (application == OpenApplication)
			{
				OpenApplication = null;
			}
		}

		public override Task<int> Execute(Application parent, Application pipeTo, string[] args)
		{
			throw new NotImplementedException();
		}


		public override byte ReadByte()
		{
			if (readPosition < Bytes.Count) // Only read if we are in bounds
				return Bytes[readPosition];
			else
				throw new IndexOutOfRangeException();
		}

		public override bool SetReadPosition(int position)
		{
			readPosition = position;
			return readPosition < Bytes.Count;
		}

		public override void WriteByte(byte b)
		{
			if (readPosition < Bytes.Count) // If we are within the file, replace the bytes
				Bytes[readPosition] = b;
			else if (readPosition == Bytes.Count) // When we want to write to the next byte, add a byte
				Bytes.Add(b);
			else
				throw new IndexOutOfRangeException();
		}

		public override void ClearAllBytes()
		{
			Bytes.Clear();
		}
	}
}