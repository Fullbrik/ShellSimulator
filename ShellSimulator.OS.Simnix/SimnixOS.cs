using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
    public class SimnixOS : OperatingSystem
    {
        public override void Install()
        {
            throw new System.NotImplementedException();
        }

        protected async override Task Init()
        {
            StartDaemon(new TerminalDaemon());
        }
    }
}