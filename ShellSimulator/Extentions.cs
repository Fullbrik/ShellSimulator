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
    }
}