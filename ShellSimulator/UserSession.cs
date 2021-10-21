using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShellSimulator
{
	public abstract class UserSession : Application
	{
		public override string Username => username;
		private string username;

		public override string HomeFolder => UserHomeFolder;
		protected abstract string UserHomeFolder { get; }

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

		public override string CurrentWorkingDirectory { get => _CurrentWorkingDirectory; set => _CurrentWorkingDirectory = SimplifyCurrentWorkingDirectory(value); }
		private string _CurrentWorkingDirectory = "";

		protected virtual string SimplifyCurrentWorkingDirectory(string cwd)
		{
			return SimplifyPath(cwd, false);
		}

		public Dictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();
		public override string GetEnvironmentVariable(string name)
		{
			if (EnvironmentVariables.ContainsKey(name)) return EnvironmentVariables[name];
			else return "";
		}

		protected override void ReceiveSignal(ApplicationSignal signal)
		{
			throw new System.NotImplementedException();
		}

	}
}