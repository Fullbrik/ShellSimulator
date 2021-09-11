using System;
using System.Collections.Generic;
using System.Linq;
namespace ShellSimulator.Applications
{
    public class Test : Application
    {
        string value1;
        string value2;
        string comparison;

        bool invert = false;

        bool failed = false;

        Dictionary<string, Predicate<string>> comparisons = new Dictionary<string, Predicate<string>>();

        public Test(Shell shell) : base(shell)
        {
            comparisons.Add("-e", (_) => FileSystem.DoesFileExist(value1));
            comparisons.Add("-d", (_) => FileSystem.DoesDirectoryExist(value1));
            comparisons.Add("-w", (_) => FileSystem.DoesFileExist(value1) && FileSystem.CanWriteToFile(value1));

            comparisons.Add("-n", (_) => value1.Length > 0);
            comparisons.Add("-z", (_) => value1.Length == 0);
            comparisons.Add("=", (_) => value1 == value2);
            comparisons.Add("!=", (_) => value1 != value2);

            comparisons.Add("-eq", (_) => GetNumValues(out double num1, out double num2) && num1 == num2);
            comparisons.Add("-ne", (_) => GetNumValues(out double num1, out double num2) && num1 != num2);
            comparisons.Add("-gt", (_) => GetNumValues(out double num1, out double num2) && num1 > num2);
            comparisons.Add("-lt", (_) => GetNumValues(out double num1, out double num2) && num1 >= num2);
            comparisons.Add("-ge", (_) => GetNumValues(out double num1, out double num2) && num1 < num2);
            comparisons.Add("-le", (_) => GetNumValues(out double num1, out double num2) && num1 <= num2);
        }

        private bool GetNumValues(out double num1, out double num2)
        {
            num1 = 0; // We do this to make c# happy
            num2 = 0;

            return double.TryParse(value1, out num1) && double.TryParse(value2, out num2);
        }

        protected override int Main(string[] args)
        {
            if (!ParseArgs(args)) return 2;

            return Compare() ? 0 : 1;
        }

        private bool ParseArgs(string[] args)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith('-') || arg == "=" || arg == "!=") //Arg is supposed to be a comparison type
                {
                    if (comparison != null) // This is because we might into a scenerio where we try 'test -hi -n', so we only do the last one
                        if (value1 == null)
                            value1 = comparison;
                        else if (value2 == null)
                            value2 = comparison;

                    comparison = arg;
                }
                else if (arg == "!")
                {
                    invert = true;
                }
                else if (value1 == null)
                {
                    value1 = arg;
                }
                else if (value2 == null)
                {
                    value2 = arg;
                }
                else
                {
                    Printlnf("Error: Too many arguments.");
                    return false;
                }
            }

            //Make sure values aren't null
            if (value1 == null) value1 = "";
            if (value2 == null) value2 = "";

            if (!comparisons.ContainsKey(comparison)) //Make sure the comparison is a valid one
            {
                Printlnf("Invalid comparison type {0}", comparison);
                return false;
            }

            return true;
        }

        private bool Compare()
        {
            return (invert) ? !comparisons[comparison](comparison) : comparisons[comparison](comparison);
        }
    }
}