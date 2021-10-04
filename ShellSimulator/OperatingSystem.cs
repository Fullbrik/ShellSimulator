using System;
using System.Linq;
using System.Collections.Generic;
using ShellSimulator.Hardware;
using System.Threading.Tasks;

namespace ShellSimulator
{
	public abstract class OperatingSystem
	{
		public bool IsRunning { get; private set; }

		/// <summary>
		/// Mounts the root file system, and installs any needed OS files and applications.
		/// </summary>
		public abstract void Install();

		public async Task Run()
		{
			await Init();
		}

		/// <summary>
		/// Startup any daemons and launch the users shell.
		/// </summary>
		protected abstract Task Init();

		#region Devices
		private readonly List<Device> devices = new List<Device>();

		public void ConnectDevice(Device device)
		{
			if (device == null) throw new NullReferenceException();

			devices.Add(device);

			device.OnConnect(this);
		}

		public T GetDevice<T>()
			where T : Device
		{
			return devices.FirstOrDefault((d) => d is T) as T;
		}

		public T[] GetDevices<T>()
			where T : Device
		{
			return devices.Where((d) => d is Device).Cast<T>().ToArray();
		}
		#endregion

		#region Processes
		private readonly List<Application> processes = new List<Application>();
		private readonly Dictionary<Daemon, Task> daemons = new Dictionary<Daemon, Task>();

		public async Task<int> StartApplication(Application application, Application parent, Application pipeTo, params string[] args)
		{
			if (application == null) throw new NullReferenceException("Application was null");

			var task = application.Run(this, parent, pipeTo, args); // Run the application

			processes.Add(application); // Add the process to the list

			int result = await task; // Wait until we get a result

			processes.Remove(application); // Remove the process because it is done runing

			return result;
		}

		public void StartDaemon(Daemon daemon, params string[] args)
		{
			// Make sure the daemon isn't null and isn't already running
			if (daemon == null) throw new NullReferenceException("Daemon was null");
			if (daemon.IsRunning) throw new Exception("Daemon is already running");

			var task = daemon.Run(this, null, null, args); // Run the daemon and get it's Task. This lets us wait for it later

			// Add the daemon to the process list and daemon list
			processes.Add(daemon);
			daemons.Add(daemon, task);

			// Remove the daemon when it is done
			task.ContinueWith((task) =>
			{
				processes.Remove(daemon);
				daemons.Remove(daemon);
			});
		}
		#endregion

		#region File Stuff
		public abstract char PathSeperator { get; }

		private Dictionary<string, FileSystem> FSRoots = new Dictionary<string, FileSystem>();

		/// <summary>
		/// Add a root file system. Different operating systems might want to do this differently.
		/// Unix style operating systems might just have a /, while NT style operating systems might have a differnet prefix for each drive (C:\, D:\, etc.).
		/// </summary>
		/// <param name="prefix">The prefix (/, C:\, D:\, etc.).</param>
		/// <param name="fs">The actual file system it goes to</param>
		protected void AddRootFS(string prefix, FileSystem fs)
		{
			FSRoots.Add(prefix, fs);
			fs.RootPrefix = prefix;
			fs.OnMount();
		}

		public void MakeDirectory(string path, bool recursive = true)
		{
			var directory = GetDirectory(path, recursive, !recursive, out string last);

			if (!recursive) // If we did it recursively, we already created any missing directories.
			{
				directory.MakeDirectory(last);
			}
		}

		public void MountFS(string path, Func<OperatingSystem, string, Directory, FileSystem> fsBuilder)
		{
			var directory = GetDirectory(path, false, true, out string last);

			var fs = fsBuilder(this, last, directory);

			directory.MountFileSystem(fs, last);
		}

		public string[] GetAllSubDirectories(string path)
		{
			var directory = GetDirectory(path, false, false, out string last);

			return directory.SubDirectories;
		}

		private Directory GetDirectory(string path, bool createMissing, bool ignoreLast, out string last)
		{
			last = null; // If we don't ignore last, we don't need to provide a last.

			var root = GetRootFSForPath(path, out string pathNoRoot);

			if (pathNoRoot.EndsWith(PathSeperator)) // If the path ends with a path seperator, remove it so we don't confuse the algorithms
				pathNoRoot = pathNoRoot.Remove(pathNoRoot.Length - 1);

			var pathSplit = (string.IsNullOrEmpty(pathNoRoot)) ? Array.Empty<string>() : SplitPath(pathNoRoot); // If the path was just a root, we now have an empty directory. Make sure we just access the root.

			Directory currentDirectory = root;
			foreach (var part in pathSplit)
			{
				if (currentDirectory.HasSubDirectory(part))
				{
					currentDirectory = currentDirectory.GetDirectory(part);
				}
				else if (createMissing)
				{
					currentDirectory = currentDirectory.MakeDirectory(part);
				}
				else if (ignoreLast && pathSplit.Last() == part)
				{
					last = part;
					// Do nothing
				}
				else
				{
					throw new DirectoryNotFoundException(part, currentDirectory);
				}
			}

			return currentDirectory;
		}

		private FileSystem GetRootFSForPath(string path, out string pathNoRoot)
		{
			var rootKVP = FSRoots.FirstOrDefault((kvp) => path.StartsWith(kvp.Key));

			string rootName = rootKVP.Key;
			var root = rootKVP.Value;

			pathNoRoot = path.Remove(0, rootName.Length);

			return root;
		}

		private string[] SplitPath(string path)
		{
			return path.Split(PathSeperator);
		}
		#endregion

		public virtual void Shutdown()
		{
			StopAllDaemons();
		}

		/// <summary>
		/// End all tasks daemons and wait for them to close
		/// </summary>
		public void StopAllDaemons()
		{
			var daemonProcesses = daemons.Keys;
			var daemonTasks = daemons.Values;

			var stopTask = Task.WhenAll(daemonTasks);

			// Request all daemons stop
			foreach (var process in daemonProcesses)
			{
				process.RequestStop();
			}

			// And wait for them all to stop
			if (!stopTask.IsCompleted)
				stopTask.Wait();
		}
	}
}