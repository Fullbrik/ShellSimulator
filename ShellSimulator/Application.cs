using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShellSimulator
{
	public abstract class Application
	{
		public abstract string Name { get; }

		public OperatingSystem OS { get; private set; }
		public Application Parent { get; private set; }
		public Application PipeTo { get; set; }

		public Task<int> Run(OperatingSystem os, Application parent, Application pipeTo, params string[] args)
		{
			OS = os;
			Parent = parent;
			PipeTo = pipeTo;

			try
			{
				return Main(args);
			}
			catch (Exception e)
			{
				PrintF("Exception thrown of type \"{0}\" with message \"{1)\"", e.GetType().Name, e.Message);
				return Task.Run<int>(() => int.MinValue);
			}
		}

		protected abstract Task<int> Main(string[] args);

		#region STDIO
		private readonly List<char> inBuffer = new List<char>();

		/// <summary>
		/// Write a character to the inBuffer
		/// </summary>
		/// <param name="c"></param>
		private void WriteChar(char c)
		{
			inBuffer.Add(c);
		}

		/// <summary>
		/// Write a string to the inBuffer
		/// </summary>
		/// <param name="s"></param>
		private void WriteString(string s)
		{
			inBuffer.AddRange(s);
		}

		/// <summary>
		/// Print a character to the Standard Output
		/// </summary>
		/// <param name="c"></param>
		protected void PrintC(char c)
		{
			if (PipeTo != null)
				PipeTo.WriteChar(c);
		}

		/// <summary>
		/// Print a formatted string to the Standard Output.
		/// </summary>
		/// <param name="str">A string to be formatted. Insert arg with {#} (IE: PrintF("Hello {0}!", "World").</param>
		/// <param name="args">Any variables to insert into the string.</param>
		protected void PrintF(string str, params object[] args)
		{
			str = string.Format(str, args);

			if (PipeTo != null)
				PipeTo.WriteString(str);
		}

		/// <summary>
		/// Print a formatted string to the Standard Output, followed by a new line.
		/// </summary>
		/// <param name="str">A string to be formatted. Insert arg with {#} (IE: PrintF("Hello {0}!", "World").</param>
		/// <param name="args">Any variables to insert into the string.</param>
		protected void PrintFLN(string str, params object[] args)
		{
			str = string.Format(str, args);

			if (PipeTo != null)
			{
				PipeTo.WriteString(str);
				PipeTo.WriteChar('\n');
			}
		}

		protected bool IsCharAvailable() => inBuffer.Count > 0;

		protected async Task<char> ReadChar()
		{
			while (!IsCharAvailable())
			{
				await Task.Delay(1); // Wait until we have something in the buffer
			}

			// Pop a character off the back of the buffer.
			char c = inBuffer[0]; // Store it in a variable
			inBuffer.RemoveAt(0); // Remove it so we don't read it again
			return c; // Return it
		}

		protected async Task<string> ReadLine()
		{
			while (!IsCharAvailable() || inBuffer[inBuffer.Count - 1] != '\n') // Wait until we have a new line character as the last character in the buffer
			{
				await Task.Delay(1);
			}

			inBuffer.RemoveAt(inBuffer.Count - 1); // Remove the newline from the inBuffer before we turn it into a string.

			string result = new string(inBuffer.ToArray()); // Get the buffer as a string

			inBuffer.Clear(); // Clear the buffer

			return result; // Return the result
		}
		#endregion
	}
}