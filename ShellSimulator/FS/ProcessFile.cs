using System.Text;
using System.IO;

namespace ShellSimulator.FS
{
    public class ProcessFile : File
    {
        public Application Process { get; }

        public ProcessFile(Application process)
        {
            Process = process;
        }

        public override bool IsExecutable => false;

        public override bool CanOpenStream => false;

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override Stream OpenStream()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Read()
        {
            return Encoding.UTF8.GetBytes(Process.GetType().Name);
        }

        public override void Write(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        protected override int OnExecute(Shell shell, Application sender, string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}