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
            var libUser = new Lib.LibUser(this);

            bool isNextArgAParam = false; // Some options (e.g. --username) request a parameter (e.g. --username [username])
            for (int i = 0; i < args.Length; i++)
            {
                if (isNextArgAParam)
                {

                }
                else
                {
                    switch (args[i])
                    {
                        case "-u":
                        case "-username":
                            isNextArgAParam = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (args.Length == 1)
            {
                // I can't think of any yet
            }
            else if (args.Length == 2)
            {
                switch (args[0])
                {
                    // Just verify provided username
                    case "-u":
                    case "--username":
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ClearScreen();

                PrintF("Username: ");
                InputUsername = await ReadLine();

                PrintF("Password: ");
                InputPassword = await ReadLine();
            }



            return 0;
        }
    }
}