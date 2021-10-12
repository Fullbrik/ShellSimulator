using System.Threading.Tasks;

namespace ShellSimulator
{
    public abstract class UserSession : Application
    {
        public override string Username => username;
        private string username;

        public override string Name => Username;

        public UserSession(string username)
        {
            this.username = username;
        }

        protected override Task<int> Main(string[] args)
        {
            var root = CreateRootUserProcess(Username, args, out Application pipeTo, out string[] outArgs);

            return StartApplication(root, pipeTo, outArgs);
        }

        protected abstract Application CreateRootUserProcess(string username, string[] args, out Application pipeTo, out string[] outArgs);
    }
}