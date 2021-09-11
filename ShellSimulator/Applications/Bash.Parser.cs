using System.IO;
using System;
using System.Collections.Generic;

namespace ShellSimulator.Applications
{
    public partial class Bash
    {
        class Parser
        {
            public string Input { get; }
            public Bash Bash { get; }
            public Shell Shell { get => Bash.Shell; } //This is only here because I don't want to type Bash.Shell a bunch. Sue me; I dare you. Actually pls don't I'm broke.

            List<string> parts = new List<string>(); // The parts of the current command
            string current = "";
            string currentEmbed = "";

            bool isInString = false;
            bool isEscaping = false;
            bool isEmbeding = false;
            bool isEmbedingCommand = false;
            bool isInEmbeddingString = false;

            bool failed = false;

            public Parser(string input, Bash bash)
            {
                Input = input;
                Bash = bash;
            }

            public bool Parse()
            {
                foreach (char character in Input)
                {
                    if (failed) return false; //We call this a bunch to catch a fail asap

                    //Printlnf("State {0}, {1}, {2}, {3}", isInString, isEscaping, isEmbeding, isEmbedingCommand);

                    bool isPartFinished = false;

                    if (isInString) //Are we in a string? If so, ignore all special characters (Except Escape)
                    {
                        HandleString(character, ref isPartFinished);
                    }
                    else if (isEmbeding)
                    {
                        HandleEmbedCharacter(character, ref isPartFinished);
                    }
                    else
                    {
                        HandleOther(character, ref isPartFinished);
                    }

                    if (isPartFinished)
                    {
                        if (!string.IsNullOrWhiteSpace(current)) //Don't add empty buffers
                            parts.Add(current);

                        current = "";
                    }

                    if (failed) return false;
                }

                if (failed) return false;

                // If we didn't end with a space, we need to add whatever is in the current buffers when we gen the end of the string
                if (isEmbeding)
                {
                    bool _ = false;
                    CompleteEmbed('\0', ref _);
                }

                if (failed) return false;

                if (!string.IsNullOrWhiteSpace(current)) parts.Add(current);

                return !failed;
            }

            private void HandleEmbedCharacter(char character, ref bool isPartFinished)
            {
                if (currentEmbed.Length == 0 && character == '(') //If we just started, we need to check if we are embedding a command, or just a variable
                {
                    isEmbedingCommand = true;
                }
                else if ((character == ' ' || character == '\t') && !isEmbedingCommand)
                {
                    CompleteEmbed(character, ref isPartFinished);
                }
                else if (isInEmbeddingString && isEscaping)
                {
                    currentEmbed += character;
                }
                else if (character == '\'' || character == '"')
                {
                    currentEmbed += character;
                    isInEmbeddingString = !isInEmbeddingString;
                }
                else if (character == '\\' && isInEmbeddingString)
                {
                    currentEmbed += character;
                    isEscaping = true;
                }
                else if (character == ')' && isEmbedingCommand && !isInEmbeddingString)
                {
                    CompleteEmbed(character, ref isPartFinished);
                }
                else
                {
                    currentEmbed += character;
                }
            }

            private void CompleteEmbed(char character, ref bool isPartFinished)
            {
                isEmbeding = false;

                if (isEmbedingCommand) //We need to execute the command to get it's stdout, and use that as a part
                {
                    parts.Add(ExecuteAndGetOutput(currentEmbed));
                }
                else
                {
                    if (Shell.Variables.ContainsKey(currentEmbed)) //If the variable doesn't exist, we will just give a blank
                        current += Shell.Variables[currentEmbed];

                    if (!isInString || character == '\0') //We only want to end if we aren't in a string
                        isPartFinished = true;
                    else //If we are in a string, we need to also push a space, because that's what the user prob expects
                        current += character;
                }
            }

            private string ExecuteAndGetOutput(string command)
            {
                using (var stdout = new StringWriter())
                {
                    int result = Bash.Execute(command, stdout);

                    if (result != 0)
                    {
                        failed = true;
                        return null;
                    }
                    else
                    {
                        return stdout.ToString();
                    }
                }
            }

            private void HandleString(char character, ref bool isPartFinished)
            {
                if (isEscaping) //We have escaped the character
                {
                    current += TranslateEscapedCharacter(character);
                    isEscaping = false;
                }
                else
                {
                    switch (character)
                    {
                        case '"':
                        case '\'':
                            isInString = false;
                            isPartFinished = true;
                            break;
                        case '\\':
                            isEscaping = true;
                            break;
                        default:
                            current += character;
                            break;
                    }
                }
            }

            private char TranslateEscapedCharacter(char character)
            {
                switch (character)
                {
                    case 'a':
                        return '\a';
                    case 'b':
                        return '\b';
                    case 't':
                        return '\t';
                    case 'r':
                        return '\r';
                    case 'v':
                        return '\v';
                    case 'f':
                        return '\f';
                    case 'n':
                        return '\n';
                    default:
                        return character;
                }
            }

            private void HandleOther(char character, ref bool isPartFinished)
            {
                // I used a switch statement because I'm not Yandre Dev
                switch (character)
                {
                    case ' ': // We are moving on to the next part
                    case '\t':
                        isPartFinished = true;
                        break;
                    case '"':
                    case '\'': //We want to be in a string
                        isInString = true;
                        isPartFinished = true;
                        break;
                    case '$':
                        isEmbeding = true;
                        currentEmbed = "";
                        break;
                    default:
                        current += character;
                        break;
                }
            }

            public string[] GetParts()
            {
                return parts.ToArray();
            }
        }
    }
}