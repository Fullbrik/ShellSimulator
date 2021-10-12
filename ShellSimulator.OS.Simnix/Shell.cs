using System.Threading.Tasks;

namespace ShellSimulator.OS.Simnix
{
    public class Shell : Application
    {
        public override string Name => "Shell";

        protected override Task<int> Main(string[] args)
        {
            throw new System.NotImplementedException();
        }
    }
}