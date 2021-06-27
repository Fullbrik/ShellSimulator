using System.Linq;
using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Echo : Application
    {
        public Echo(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string text = "";
            string outFileName = "";
            bool isSettingFileName = false;
            bool justAppendLine = false;

            foreach (var arg in args)
            {
                if (arg == ">")
                {
                    isSettingFileName = true;
                }
                else if (arg == ">>")
                {
                    isSettingFileName = true;
                    justAppendLine = true;
                }
                else
                {
                    if (isSettingFileName)
                    {
                        outFileName += arg + " ";
                    }
                    else
                    {
                        text += arg + " ";
                    }
                }
            }

            if (isSettingFileName)
            {
                string fileName = FileSystem.RelativePathToAbsolutePath(outFileName);

                try
                {
                    if (justAppendLine)
                    {
                        var lines = FileSystem.ReadAllFileLines(fileName).ToList();
                        lines.Add(text);
                        FileSystem.WriteAllFileLines(fileName, lines.ToArray());
                    }
                    else
                    {
                        FileSystem.WriteAllFileText(fileName, text);
                    }
                }
                catch (FSException e)
                {
                    Printlnf(e.Message);
                    return -1;
                }
            }
            else
            {
                Printlnf(text);
            }

            return 0;
        }
    }
}