using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix.Applications
{
    public class Echo : Application
    {
        public override string Name => "Echo";

        protected override Task<int> Main(string[] args)
        {
            string print = string.Join(' ', args); // Join the args together with a space
            PrintFLN(print);

            return Task.FromResult(0);
        }
    }
}