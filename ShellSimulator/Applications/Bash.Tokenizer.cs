using System.Collections.Generic;
namespace ShellSimulator.Applications
{
    public partial class Bash
    {
        enum TokenType
        {
            Command,
        }

        abstract class Token
        {
            public abstract TokenType Type { get; }
        }

        class CommandToken : Token
        {
            public override TokenType Type => TokenType.Command;

            public string Command { get; }

            public string[] Args { get => _args.ToArray(); }
            private List<string> _args = new List<string>();
            public void AddArg(string arg) => _args.Add(arg);

            public bool PipeToNextCommand { get; set; } = false;
            public CommandToken PipeSource { get; set; } = null;

            public CommandToken(string command, CommandToken pipeSource)
            {
                Command = command;
                PipeSource = pipeSource;
            }
        }

        class Tokenizer
        {
            public string[] Parts { get; }

            private List<Token> tokens = new List<Token>();

            CommandToken currentCommandToken = null;

            bool doCommandNext = false; // If it false, we are working on args.

            public Tokenizer(string[] parts)
            {
                Parts = parts;
            }

            public void Tokenize()
            {
                doCommandNext = true; // We want the first token to be a command.

                foreach (var part in Parts)
                {
                    switch (part)
                    {
                        case "|":
                            currentCommandToken.PipeToNextCommand = true;
                            doCommandNext = true;
                            break;
                        default:
                            if (doCommandNext)
                            {
                                CommandToken pipeSource = null;

                                if (currentCommandToken != null && currentCommandToken.PipeToNextCommand)
                                    pipeSource = currentCommandToken;

                                currentCommandToken = new CommandToken(part, pipeSource); // Create a new command token
                                tokens.Add(currentCommandToken);
                                doCommandNext = false;
                            }
                            else
                            {
                                currentCommandToken.AddArg(part);
                            }
                            break;
                    }
                }
            }

            public Token[] GetTokens() => tokens.ToArray();
        }
    }
}