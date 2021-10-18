using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
    public class Passwd : Application
    {
        public override string Username => usernameOveride ?? base.Username;
        private string usernameOveride = null;

        public override string Name => "Passwd";

        protected override Task<int> Main(string[] args)
        {
            var libUser = new Lib.LibUser(this);

            if (args.Length == 1)
            {
                usernameOveride = args[0];
            }
        }
    }
}