using System.Threading.Tasks;
using ShellSimulator.OS.Simnix.Applications;

namespace ShellSimulator.OS.Simnix
{
    public class SimnixOS : OperatingSystem
    {
        public FileSystem RootFS { get; private set; }

        public override char PathSeperator => '/';

        public override void Install()
        {
            // Create and add the root fs
            RootFS = new SimpleFileSystem(this, "/", null);
            AddRootFS("/", RootFS);

            MakeDirectory("/bin");
            MakeDirectory("/home");
            MakeDirectory("/tmp");
            MakeDirectory("/opt");
            MakeDirectory("/etc");
            MakeDirectory("/var");
            MakeDirectory("/var/log");
            MakeDirectory("/usr");
            MakeDirectory("/usr/local");

            MountFS("/proc", (os, name, parent) => new ProcessFileSystem(os, name, parent));

            InstallApplication<TerminalDaemon>("/bin/terminald");
        }

        protected override bool CreateuserSession(string username, string password, out string error, out UserSession session)
        {
            throw new System.NotImplementedException();
        }

        protected async override Task Init()
        {
            // Create the terminal daemon
            var terminal = new TerminalDaemon();
            StartDaemon(terminal);

            while (IsRunning) // We will do this until we end
            {
                // Prompt the user to login
                var login = new UserLogin();
                terminal.PipeTo = login;
                await StartApplication(login, null, terminal);

                // Get the login details
                string username = login.InputUsername;
                string password = login.InputPassword;

                // Login
                await StartUserSession(username, password, out string sessionError);

                // Print any errors to the terminal
                await StartApplication(new Echo(), null, terminal, sessionError); // We just use echo because it will print it out to the terminal
            }
        }
    }
}