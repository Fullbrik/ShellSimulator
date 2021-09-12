using System;
namespace ShellSimulator
{
    public static class Extentions
    {
        public static T DoChained<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }

        public static string ReplaceShellVars(this string commandLine, Shell shell)
        {
            foreach (var v in shell.Variables)
                commandLine = commandLine.Replace($"${v.Key.ToUpper()}", v.Value);

            return commandLine;
        }

        public static string GetLineTextOfStringIndex(this string text, int index)
        {
            // Get the start index of the line
            int startLineIndex = index;

            while (startLineIndex > 0 && text[startLineIndex] != '\n')
            {
                startLineIndex--;
            }

            // Get the end index of the line
            int endLineIndex = index + 1; // We do plus one in case the index is a new line
            while (endLineIndex < text.Length && text[endLineIndex] != '\n' && text[endLineIndex] != '\r')
            {
                endLineIndex++;
            }

            return text.Substring(startLineIndex, endLineIndex - startLineIndex);
        }
    }
}