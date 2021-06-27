using System;
using System.IO;

namespace ShellSimulator.FS
{
    public class ApplicationFile : File
    {
        public Func<Shell, Application> AppBuilder { get; }

        public override bool IsExecutable => true;

        public override bool CanOpenStream => false;

        public override bool CanRead => true;

        public override bool CanWrite => false;

        private byte[] data;

        public ApplicationFile(Func<Shell, Application> appBuilder)
        {
            AppBuilder = appBuilder;

            data = new byte[50];
            Random rand = new Random((int)((DateTime.Now.ToBinary() * DateTime.Now.ToBinary() * 2) % int.MaxValue)); //Hopefully this will make things really random

            rand.NextBytes(data);
        }

        public override Stream OpenStream()
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Read()
        {
            return data;
        }

        public override void Write(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        protected override int OnExecute(Shell shell, Application sender, string[] args)
        {
            return AppBuilder(shell).Start(sender ?? shell.RootProcess, args);
        }
    }
}