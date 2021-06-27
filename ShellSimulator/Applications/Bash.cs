using System.Linq;
using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Bash : Application
    {
        public Bash(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            while (true)
            {
                PrintPreThingy();
                string enteredCommand = ReadLine();

                string[] commandAndArgs = ParseCommandArguments(enteredCommand);
                string command = Shell.CheckAlias(commandAndArgs[0]);

                string[] argsNoCommand = commandAndArgs.ToList().DoChained((caa) => caa.RemoveAt(0)).ToArray(); //Hacked way to remove the first element so all the args are just args and not the command's name.

                try
                {
                    int result = ExecuteCommand(command, argsNoCommand);

                    if (result != 0)
                    {
                        Printlnf("Application {0} exited with code {1}", command, result);
                    }
                }
                catch (CommandNotFoundException e)
                {
                    Printlnf(e.Message);
                }
#if !DEBUG
                catch (System.Exception e)
                {
                    Printlnf(e.message);

                }
#endif
            }
        }

        private void PrintPreThingy()
        {
            Printf("{0}@{1}:{2}$ ", Shell.Username, Shell.MachineName, Shell.CWD);
        }

        private string[] ParseCommandArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray(); //Create char array
            bool inQuote = false; //See weather we are in a quote (b/c if we are, we don't split by space)
            bool escapeNext = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (!escapeNext)
                {
                    switch (parmChars[index])
                    {
                        case ' ':
                            if (!inQuote)
                                parmChars[index] = '\n';
                            break;
                        case '\\':
                            escapeNext = true;
                            parmChars[index] = (char)2;
                            break;
                        case '"':
                            inQuote = !inQuote;
                            parmChars[index] = (char)2;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    escapeNext = false;
                }
            }
            return (new string(parmChars)).Replace(((char)2).ToString(), "").ReplaceShellVars(Shell).Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
        }

        private int ExecuteCommand(string command, string[] args)
        {
            if (command.Contains('/'))
                return FileSystem.ExecuteFile(FileSystem.RelativePathToAbsolutePath(command), this, args);
            else
                return FileSystem.ExecuteFileOnPath(command, this, args);
        }
    }
}