using System.Collections.Generic;
using System.IO;
using System;
using ShellSimulator.FS;

namespace ShellSimulator
{
    public class Shell
    {
        public STDIn STDIn { get; } = new STDIn();
        public TextWriter STDOut { get; set; } = new StringWriter();
        public event Action RequestClearScreen;

        public string CWD { get => Variables["CWD"]; set => Variables["CWD"] = FS.FlattenPath(value); }
        public string[] Path { get => Variables["PATH"].Split(":"); set => Variables["PATH"] = string.Join(':', value); }

        public string MachineName { get; set; } = Environment.MachineName;
        public string Username { get; set; } = Environment.UserName;

        public FileSystem FS { get; }

        public Shell()
        {
            FS = new FileSystem(this);
            Variables.Add("CWD", "/");
            Variables.Add("PATH", "");
        }

        public bool ClearScreen()
        {
            if (RequestClearScreen != null) RequestClearScreen();
            else if (STDOut is StringWriter sw) sw.GetStringBuilder().Clear();
            else return false;

            return true;
        }


        #region Process
        private List<Application> processes { get; } = new List<Application>();
        public Application[] Processes { get => processes.ToArray(); }

        public Application RootProcess { get => (processes.Count > 0) ? processes[0] : null; }

        public void OnStartProcess(Application process)
        {
            processes.Add(process);
        }

        public void OnEndProcess(Application process)
        {
            processes.Remove(process);
        }
        #endregion

        #region ShellVars
        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> Aliases { get; } = new Dictionary<string, string>();

        public string CheckAlias(string command)
        {
            if (Aliases.ContainsKey(command)) return Aliases[command];
            else return command;
        }
        #endregion
    }
}
