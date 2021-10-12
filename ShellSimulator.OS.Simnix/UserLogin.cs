using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
    public class UserLogin : Application
    {
        public override string Name => "User Login";

        public string InputUsername { get; private set; }
        public string InputPassword { get; private set; }

        protected override async Task<int> Main(string[] args)
        {
            PrintF("Username: ");
            InputUsername = await ReadLine();

            PrintF("Password: ");
            InputPassword = await ReadLine();

            return 0;
        }
    }
}