using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShellSimulator.Hardware;

namespace ShellSimulator
{
	public enum ApplicationSignal
	{

	}

	public abstract class Application
	{
		public abstract string Name { get; }

		private OperatingSystem OS { get; set; }
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

		public void SendSignal(ApplicationSignal signal)
		{
			ReceiveSignal(signal);
		}
		protected abstract void ReceiveSignal(ApplicationSignal signal);

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
		/// Remove the last character on the buffer
		/// </summary>
		private bool Backspace()
		{
			if (inBuffer.Count > 0)
			{
				inBuffer.RemoveAt(inBuffer.Count - 1);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Print a character to the Standard Output
		/// </summary>
		/// <param name="c"></param>
		protected void PrintC(char c)
		{
			if (PipeTo != null)
				if (c == 0x08) // If we do a backspace character
					PipeTo.Backspace();
				else
					PipeTo.WriteChar(c);
		}

		protected bool PrintBackspace()
		{
			if (PipeTo != null)
				return PipeTo.Backspace();
			else
				return false;
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

		protected bool IsCharAvailable() => OS.IsRunning && inBuffer.Count > 0;

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
			while (OS.IsRunning && (!IsCharAvailable() || inBuffer[inBuffer.Count - 1] != '\n')) // Wait until we have a new line character as the last character in the buffer, as long as the OS is still running
			{
				await Task.Delay(1);
			}

			if (!OS.IsRunning) return ""; // If the os is no longer running, return a blank string

			inBuffer.RemoveAt(inBuffer.Count - 1); // Remove the newline from the inBuffer before we turn it into a string.

			string result = new string(inBuffer.ToArray()); // Get the buffer as a string

			inBuffer.Clear(); // Clear the buffer

			return result; // Return the result
		}

		protected virtual void ClearScreen()
		{
			if (PipeTo != null) PipeTo.ClearScreen();
		}
		#endregion

		#region Syscall Wrappers
		public bool IsOSRunning { get => OS.IsRunning; }

		public Task<int> StartApplication(Application application, Application pipeTo, params string[] args)
		{
			return OS.StartApplication(application, this, pipeTo, args);
		}

		public char PathSeperator { get => OS.PathSeperator; }

		public string[] GetAllSubDirectories(string path)
		{
			return OS.GetAllSubDirectories(GetFullPath(path));
		}

		public string[] GetAllFilesInDirectory(string path)
		{
			return OS.GetAllFilesInDirectory(GetFullPath(path));
		}

		public void MakeDirectory(string path, bool recursive = true)
		{
			OS.MakeDirectory(GetFullPath(path), recursive);
		}

		public void MountFS(string path, Func<OperatingSystem, string, Directory, FileSystem> fsBuilder)
		{
			OS.MountFS(GetFullPath(path), fsBuilder);
		}

		public File OpenFile(string path)
		{
			return OS.OpenFile(GetFullPath(path), this);
		}

		public void InstallApplication<T>(string path)
			where T : Application, new()
		{
			OS.InstallApplication<T>(GetFullPath(path));
		}

		public string SimplifyPath(string path, bool isPathFile)
		{
			return OS.SimplifyPath(GetFullPath(path), isPathFile);
		}

		public string GetFullPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return OS.TranslatePathFromApplication(CurrentWorkingDirectory, this);
			}
			else if (OS.HasRootDir(path))
			{
				return OS.TranslatePathFromApplication(path, this);
			}
			else
			{
				return OS.TranslatePathFromApplication(CombinePaths(CurrentWorkingDirectory, path), this);
			}
		}

		public string CombinePaths(params string[] paths)
		{
			if (paths.Length == 1) return paths[0];
			else return string.Join
			(
				PathSeperator,
				paths
					.Select // Remove any trailing / so that they won't interfere with String.Join
					(
						(path) => (path.EndsWith(PathSeperator)
							? path.Remove(path.Length - 1)
							: path)
					)
			);
		}

		public T GetDaemon<T>()
			where T : Daemon
		{
			return OS.GetDaemon<T>();
		}

		public T GetDevice<T>()
			where T : Device
		{
			return OS.GetDevice<T>();
		}

		public T[] GetDevices<T>()
			where T : Device
		{
			return OS.GetDevices<T>();
		}

		public void RequestShutdown()
		{
			OS.Shutdown();
		}
		#endregion

		#region User
		public virtual string Username { get => Parent?.Username ?? ""; }
		public virtual string HomeFolder { get => Parent?.HomeFolder ?? ""; }


		public virtual string CurrentWorkingDirectory { get => (Parent != null) ? Parent.CurrentWorkingDirectory : ""; set => Parent.CurrentWorkingDirectory = value; }
		public virtual string GetEnvironmentVariable(string name)
		{
			if (Parent != null) return Parent.GetEnvironmentVariable(name);
			else return "";
		}
		#endregion
	}
}