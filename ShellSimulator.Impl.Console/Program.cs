using System;
using ShellSimulator;
using ShellSimulator.OS;

static class Program
{
    static void Main(string[] args)
    {
        Console.Clear();

        Shell shell = new Shell();
        shell.STDOut = Console.Out;
        shell.RequestClearScreen += () => Console.Clear();
        shell.STDIn.RequestKey += () =>
        {
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.Backspace)
            {
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(" ");
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                return -2;
            }
            else
            {
                if (key.Key == ConsoleKey.Enter) Console.WriteLine();

                return (int)key.KeyChar;
            }
        };

        BaseLinux os = new BaseLinux();
        os.Start(shell);
    }
}
