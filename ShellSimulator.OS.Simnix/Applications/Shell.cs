using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
    public class Shell : Application
    {
        public bool IsRunning { get => OS.IsRunning && isRunning; }
        private bool isRunning;

        public override string Name => "Shell";

        protected override async Task<int> Main(string[] args)
        {
            ClearScreen();

            isRunning = true;
            while (IsRunning)
            {
                PrintF("Command> ");
                string command = await ReadLine();

                switch (command)
                {
                    case "clear":
                        ClearScreen();
                        break;
                    case "username":
                        PrintFLN(Username);
                        break;
                    case "exit":
                        isRunning = false;
                        break;
                    case "shutdown":
                        isRunning = false;
                        await StartApplication(new ShellSimulator.OS.Simnix.Applications.Shutdown(), null);
                        break;
                    default:
                        PrintFLN("Command {0} not found", command);
                        break;
                }

            }

            return 0;
        }
    }
}