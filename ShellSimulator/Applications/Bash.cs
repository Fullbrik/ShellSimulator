using System;

namespace ShellSimulator.Applications
{
    public partial class Bash : Application
    {
        public Bash(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            if (args.Length > 0) // We will execute args without going into interactive mode
            {
                return Execute(string.Join(" ", args));
            }
            else // If we don't pass anything in, we will go into interactive mode
            {
                InteractiveMode();
            }

            return 0;
        }

        private void InteractiveMode()
        {
            while (IsRunning)
            {
                Printf("{0}@{1}:{2}$ ", Shell.Username, Shell.MachineName, Shell.CWD); // IE: mcp613@mcp613PC:~$

                string input = ReadLine();

                int result = Execute(input);

                if (result != 0) Printlnf("Process exited with code: {0}", result);
            }
        }

        public int Execute(string input, System.IO.TextWriter stdout = null)
        {
            //Parse
            var parser = new Parser(input, this);
            parser.Parse();
            var parts = parser.GetParts();

            //Tokenize
            var tokenizer = new Tokenizer(parts);
            tokenizer.Tokenize();
            var tokens = tokenizer.GetTokens();

            return ExecuteTokens(tokens, stdout);
        }

        private int ExecuteTokens(Token[] tokens, System.IO.TextWriter stdout)
        {
            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Command:
                        if (token is CommandToken command)
                        {
                            if (!command.PipeToNextCommand) // We can skip over it and come back to it later
                            {
                                int result = ExecuteCommandToken(command, stdout);

                                if (result != 0) return result; //We can only continue if the command succeeds
                            }
                        }
                        else
                        {
                            Printlnf("Bash Error: Unkown token received.");
                            return int.MinValue;
                        }
                        break;
                    default:
                        Printlnf("Bash Error: Unkown token received.");
                        return int.MinValue;
                }
            }

            return 0;
        }

        private int ExecuteCommandToken(CommandToken command, System.IO.TextWriter stdout)
        {
            string startingInput = null;

            if (command.PipeSource != null)
            {
                using (System.IO.TextWriter pipeOut = new System.IO.StringWriter())
                {
                    int result = ExecuteCommandToken(command.PipeSource, pipeOut);

                    if (result != 0) return result; //We can only continue if the command succeeds

                    startingInput = pipeOut.ToString();
                }
            }

            try
            {
                if (startingInput != null) Shell.STDIn.WriteToBuffer(startingInput + "\n");

                int result = ExecuteCommand(command.Command, stdout, command.Args);

                if (result != 0) return result;
            }
            catch (System.Exception e)
            {
                Printlnf("Error: {0}", e.Message);
                return -1;
            }

            return 0;
        }

        private int ExecuteCommand(string command, System.IO.TextWriter stdout, string[] args)
        {
            if (command.Contains('/'))
                return FileSystem.ExecuteFile(FileSystem.RelativePathToAbsolutePath(command), this, stdout, args);
            else
                return FileSystem.ExecuteFileOnPath(command, this, stdout, args);
        }

        public override void Exit()
        {
            IsRunning = false;
        }
    }
}