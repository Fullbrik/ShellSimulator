using System.Linq;
using ShellSimulator.FS;

namespace ShellSimulator.Applications
{
    public class Cat : Application
    {
        public Cat(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            string text = "";
            string outFileName = "";
            bool isSettingFileName = false;
            bool justAppendLine = false;


            try
            {
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
                            string fileName = FileSystem.RelativePathToAbsolutePath(arg);
                            string fileText = FileSystem.ReadAllFileText(fileName);
                            text += fileText + "\n";
                        }
                    }
                }
            }
            catch (FSException e)
            {
                Printlnf(e.Message);
                return -1;
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