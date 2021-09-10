using System;
using System.IO;

namespace ShellSimulator.FS
{
    public class UserCreatedFile : File
    {
        public override bool IsExecutable => false;

        public override bool CanOpenStream => false;

        public override bool CanRead => true;

        public override bool CanWrite => true;

        byte[] data = new byte[] { };

        public override Stream OpenStream()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Read()
        {
            byte[] bytes = new byte[data.Length];
            Array.Copy(data, bytes, data.Length);
            return bytes;
        }

        public override void Write(byte[] bytes)
        {
            data = new byte[bytes.Length];
            Array.Copy(bytes, data, bytes.Length);
        }

        protected override int OnExecute(Shell shell, Application sender, System.IO.TextWriter stdout, string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}