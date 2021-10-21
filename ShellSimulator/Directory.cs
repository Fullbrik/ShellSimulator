using System;
using System.Collections.Generic;
using System.Linq;

namespace ShellSimulator
{
	public class Directory
	{
		public string Name { get; }
		public virtual string FullPath { get => (Parent != null) ? Parent.FullPath + Name + OwningFileSystem.OS.PathSeperator : Name; } // Get the full path of this directory
		public Directory Parent { get; }
		public virtual FileSystem OwningFileSystem { get => Parent.OwningFileSystem; }

		public virtual string[] SubDirectories { get => subDirectories.Keys.ToArray(); }
		private readonly Dictionary<string, Directory> subDirectories = new Dictionary<string, Directory>();

		public virtual string[] Files { get => files.Keys.ToArray(); }
		private readonly Dictionary<string, File> files = new Dictionary<string, File>();

		public Directory(string name, Directory parent)
		{
			Name = name;
			Parent = parent;
		}

		#region Directory

		public virtual bool HasSubDirectory(string name)
		{
			return name == "." || name == ".." || subDirectories.ContainsKey(name);
		}

		public virtual Directory GetDirectory(string name)
		{
			if (name == ".") // This directory
			{
				return this;
			}
			else if (name == "..") // Go back one directory
			{
				return Parent;
			}
			else if (HasSubDirectory(name))
			{
				return subDirectories[name];
			}
			else
			{
				throw new DirectoryNotFoundException(name, this);
			}
		}

		private bool IsValidName(string name, bool canAlreadyExist)
		{
			return
				!name.Contains(OwningFileSystem.OS.PathSeperator) // Can't have a path separater.
				&& (canAlreadyExist || !(subDirectories.ContainsKey(name) || files.ContainsKey(name))) // If we don't want to allow directories that already exist, we filter them out.
			;
		}

		private bool IsValidDirectoryName(string name, bool canAlreadyExist)
		{
			return IsValidName(name, canAlreadyExist);
		}

		public virtual Directory MakeDirectory(string name)
		{
			if (!OwningFileSystem.IsReadOnlyFileSystem) // Make sure we can write to the file system
			{
				if (IsValidDirectoryName(name, false)) // Make sure name is valid
				{
					var directory = CreateDirectoryObject(name);
					subDirectories.Add(name, directory);
					return directory;
				}
				else
				{
					throw new InvalidFileNameException();
				}
			}
			else
			{
				throw new ReadOnlyFileSystemException();
			}
		}

		protected virtual Directory CreateDirectoryObject(string name)
		{
			return new Directory(name, this);
		}

		public virtual void MountFileSystem(FileSystem fileSystem, string name)
		{
			if (!OwningFileSystem.IsReadOnlyFileSystem) // Make sure we can write to the file system
			{
				if (IsValidDirectoryName(name, false)) // Make sure name is valid
				{
					subDirectories.Add(name, fileSystem);
					fileSystem.OnMount();
				}
				else
				{
					throw new InvalidFileNameException();
				}
			}
			else
			{
				throw new ReadOnlyFileSystemException();
			}
		}
		#endregion

		#region File

		private bool IsValidFileName(string name, bool canAlreadyExist)
		{
			return IsValidName(name, canAlreadyExist);
		}

		public virtual File GetFile(string name)
		{
			if (HasFile(name))
			{
				return files[name];
			}
			else
			{
				throw new FileNotFoundException(name, this);
			}
		}

		public virtual File CreateFile(string name, Func<string, Directory, File> fileBuilder)
		{
			if (!OwningFileSystem.IsReadOnlyFileSystem) // Make sure we can write to the file system
			{
				if (IsValidFileName(name, false))
				{
					var file = fileBuilder(name, this);
					files.Add(name, file);
					return file;
				}
				else
				{
					throw new InvalidFileNameException();
				}
			}
			else
			{
				throw new ReadOnlyFileSystemException();
			}
		}

		public virtual bool HasFile(string name)
		{
			return files.ContainsKey(name);
		}

		#endregion
	}
}