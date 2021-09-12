using System;
using System.Text.RegularExpressions;
namespace ShellSimulator.Applications
{
    public class Grep : Application
    {
        public Grep(Shell shell) : base(shell)
        {
        }

        protected override int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Printlnf("Error: Please provide at least a pattern.");
                return -1;
            }
            else if (args.Length == 1)
            {
                string pattern = args[0];
                string text = ReadLine(); //Get text from stdin

                SearchText(text, pattern, false);

                return 0;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void SearchText(string text, string pattern, bool isRegex, string prefix = "")
        {
            if (!isRegex) pattern = Regex.Escape(pattern);

            var regex = new Regex(pattern);

            SearchText(text, regex, prefix);
        }

        private void SearchText(string text, Regex regex, string prefix = "")
        {
            var matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                Printlnf("{0}{1}", prefix, text.GetLineTextOfStringIndex(match.Index));
            }
        }
    }
}