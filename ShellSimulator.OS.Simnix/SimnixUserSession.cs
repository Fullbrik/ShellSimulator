using System;

namespace ShellSimulator.OS.Simnix
{
    public class SimnixUserSession : UserSession
    {
        public SimnixUserSession(string username) : base(username)
        {
        }

        protected override Application CreateRootUserProcess(string username, string[] args, out Application pipeTo, out string[] outArgs)
        {
            var application = new Shell();
            pipeTo = OS.GetDaemon<TerminalDaemon>();
            pipeTo.PipeTo = application; // We want to do a circular thing, so the keyboard pipes into the app, and the app pipes into the terminal.

            outArgs = Array.Empty<string>();

            return application;
        }
    }
}