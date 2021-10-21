using System;
using ShellSimulator.OS.Simnix.Applications;

namespace ShellSimulator.OS.Simnix
{
	public class SimnixUserSession : UserSession
	{
		protected override string UserHomeFolder => homeFolder;
		private string homeFolder;

		private string shell;

		public SimnixUserSession(string username) : base(username)
		{
		}

		protected override Application CreateRootUserProcess(string username, string[] args, out Application pipeTo, out string[] outArgs)
		{
			var libUser = new Lib.LibUser(this);

			pipeTo = GetDaemon<TerminalDaemon>();

			if (libUser.GetUserData(Username, out homeFolder, out shell))
			{
				CurrentWorkingDirectory = "~";

				var application = new Shell();
				pipeTo.PipeTo = application; // We want to do a circular thing, so the keyboard pipes into the app, and the app pipes into the terminal.

				outArgs = Array.Empty<string>();

				return application;
			}
			else
			{
				outArgs = new string[] { $"Failed to get info on user {Username}" };
				return new Echo();
			}
		}

		public override string GetEnvironmentVariable(string name)
		{
			// We want to hard code some of the variables
			switch (name)
			{
				case "CWD":
					return CurrentWorkingDirectory;
				default:
					return base.GetEnvironmentVariable(name);
			}

		}

		protected override string SimplifyCurrentWorkingDirectory(string cwd)
		{
			string simplified = base.SimplifyCurrentWorkingDirectory(cwd);

			// Replace home folder with ~
			if (simplified.StartsWith(HomeFolder))
			{
				simplified = simplified.Remove(0, HomeFolder.Length);
				simplified = "~" + simplified;
			}

			return simplified;
		}
	}
}