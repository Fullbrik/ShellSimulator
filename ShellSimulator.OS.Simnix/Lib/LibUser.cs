using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ShellSimulator.OS.Simnix.Lib
{
	public class LibUser : Library
	{
		public struct UserData
		{
			public string Name { get; }
			public int UID { get; }
			public string HomeDirectory { get; }
			public string Shell { get; }

			public static UserData Parse(string line)
			{
				var parts = line.Split(':');

				// Note: /etc/passwd is in the same format as it is on GNU/Linux (I'm specifying GNU because I don't know what android uses).
				// We don't use all the fields but they have to be there anyway
				if (parts.Length == 7)
				{
					// Linux format: name:password:UID:GID:GECOS:directory:shell
					// We need: name (0), UID (2), (home) directory (5), shell (6)
					if (int.TryParse(parts[2], out int uid)) // This param is supposed to be a number, so lets make sure it is.
					{
						return new UserData(parts[0], uid, parts[5], parts[6]);
					}
					else
					{
						throw new System.Exception("UID parameter (2) isn't a integer.");
					}
				}
				else
				{
					throw new System.Exception($"Invalid parameters in file. Parts.Length = {parts.Length}");
				}
			}

			public UserData(string name, int uid, string homeDir, string shell)
			{
				Name = name;
				UID = uid;
				HomeDirectory = homeDir;
				Shell = shell;
			}

			public override string ToString()
			{
				return $"{Name}:x:{UID}:{UID}:{Name}:{HomeDirectory}:{Shell}";
			}
		}

		public struct PasswordData
		{
			public enum PasswordAlgorithm
			{
				/// <summary>
				/// Unsupported
				/// </summary>
				MD5 = 1,
				/// <summary>
				/// Unsupported
				/// </summary>
				Blowfish = 2,
				/// <summary>
				/// Supported
				/// </summary>
				SHA256 = 5,
				/// <summary>
				/// Supported
				/// </summary>
				SHA512 = 6
			}

			public string Username { get; }

			public PasswordAlgorithm Algorithm { get; }
			public string Hash { get; }
			public string Salt { get; }

			public PasswordData(string username, PasswordAlgorithm algorithm, string hash, string salt)
			{
				Username = username;
				Algorithm = algorithm;
				Hash = hash;
				Salt = salt;
			}

			/// <summary>
			/// Create basic password data with a password.
			/// </summary>
			/// <param name="username">The username of the person</param>
			/// <param name="password">The password</param>
			/// <param name="algorithm">The (optional)</param>
			public PasswordData(string username, string password, PasswordAlgorithm algorithm = PasswordAlgorithm.SHA512)
			{
				Username = username;
				Algorithm = algorithm;

				// I don't know what to do with this yet
				Salt = "";

				Hash = ""; // I have to do this because of C#
				Hash = GetPasswordHash(password);
			}

			public static PasswordData Parse(string line)
			{
				var parts = line.Split(':');

				// Note: /etc/shadow is in the same format as it is on GNU/Linux (I'm specifying GNU because I don't know what android uses).
				// We don't use all the fields but they have to be there anyway
				if (parts.Length == 8)
				{
					// Linux format: username:password:changed:min:max:warn:inactive:expire
					// We need: username (0), password (1)

					string username = parts[0];
					string password = parts[1];

					// We then need to get the algorithm, salt, and hash
					var passwordParts = password.Split('$');

					// Format: nothing$algorithm$salt$hash
					if (passwordParts.Length == 4)
					{
						if (byte.TryParse(passwordParts[1], out byte algorithmByte))
						{
							return new PasswordData(username, (PasswordAlgorithm)algorithmByte, passwordParts[3], passwordParts[2]);
						}
						else
						{
							throw new System.Exception("Algorithm parameter in password isn't a integer.");
						}
					}
					else
					{
						throw new System.Exception($"Invalid parameters in password. PasswordParts.Length = {passwordParts.Length}");
					}
				}
				else
				{
					throw new System.Exception($"Invalid parameters in file. Parts.Length = {parts.Length}");
				}
			}

			public bool IsCorrectPassword(string password)
			{
				string hash = GetPasswordHash(password);

				return hash == this.Hash;
			}

			public string GetPasswordHash(string password)
			{
				switch (Algorithm)
				{
					case PasswordAlgorithm.SHA256:
						using (SHA256 sha256 = SHA256.Create())
						{
							// Make hash and return it
							var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
							return string.Join("", hash.Select((b) => b.ToString())); // Will work better for file storing
						}
					case PasswordAlgorithm.SHA512:
						using (SHA512 sha512 = SHA512.Create())
						{
							// Make hash and return it
							var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
							return string.Join("", hash.Select((b) => b.ToString())); // Will work better for file storing
						}
					default:
						return null;
				}
			}

			public override string ToString()
			{
				string password = $"${(byte)Algorithm}${Salt}${Hash}";
				return $"{Username}:{password}:x:x:x:x:x:x";
			}
		}

		public LibUser(Application application) : base(application)
		{
		}

		public override string Name => "User";

		public bool CreateUser(string username, string homeFolder, string shell)
		{
			var users = Loadusers().ToList();

			if (UserExists(username, users)) return false;

			UserData data = new UserData(username, 0, homeFolder, shell);

			users.Add(data);

			SaveUsers(users);

			return true;
		}

		public bool UserExists(string username)
		{
			var users = Loadusers();

			return UserExists(username, users);
		}

		private bool UserExists(string username, IEnumerable<UserData> users)
		{
			// We loop through each user. if we find one that has the right username, we return true right away. Otherwise, we return false
			foreach (var user in users)
				if (user.Name == username)
					return true;

			return false;
		}

		public bool GetUserData(string username, out string homeFolder, out string shell)
		{
			var users = Loadusers();

			// We loop through each user. if we find one that has the right username, we set the data and return true right away. Otherwise, we return false
			foreach (var user in users)
			{
				if (user.Name == username)
				{
					homeFolder = user.HomeDirectory;
					shell = user.Shell;
					return true;
				}
			}

			// If we couldn't find the user, set everything to null
			homeFolder = null;
			shell = null;
			return false;
		}

		private UserData[] Loadusers()
		{
			string text = ReadUserDataFile();
			return ParseUserData(text).ToArray();
		}

		private string ReadUserDataFile()
		{
			// Get the text of the file and close it right away
			var file = Application.OpenFile("/etc/passwd");
			string text = file.ReadAllText();
			file.Close(Application);

			return text;
		}

		private IEnumerable<UserData> ParseUserData(string text)
		{
			var lines = text.Split('\n');

			foreach (var line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line))
					yield return UserData.Parse(line);
			}
		}

		private void SaveUsers(IEnumerable<UserData> users)
		{
			var userStrings = users.Select((user) => user.ToString());

			var text = string.Join('\n', userStrings);

			var file = Application.OpenFile("/etc/passwd");
			file.WriteAllText(text);
			file.Close(Application);
		}

		public bool IsCorrectPassword(string username, string password)
		{
			var passwords = LoadPasswordData();

			//Go through each password data. If the username and password match, return true. If there are no matches, return false.
			foreach (var data in passwords)
				if (data.Username == username && data.IsCorrectPassword(password)) return true;

			return false;
		}

		public bool SetUserPassword(string username, string password)
		{
			var users = Loadusers();

			if (!UserExists(username, users)) return false;

			var passwords = LoadPasswordData().ToList();

			bool setPassword = false;

			for (int i = 0; i < passwords.Count; i++)
			{
				if (passwords[i].Username == username)
				{
					passwords[i] = new PasswordData(username, password);
					setPassword = true;
					break;
				}
			}

			if (!setPassword) passwords.Add(new PasswordData(username, password));

			SavePasswordData(passwords);

			return true;
		}

		private PasswordData[] LoadPasswordData()
		{
			string text = ReadPasswordDataFile();
			return ParsePasswordData(text).ToArray();
		}

		private string ReadPasswordDataFile()
		{
			// Get the text of the file and close it right away
			var file = Application.OpenFile("/etc/shadow");
			string text = file.ReadAllText();
			file.Close(Application);

			return text;
		}

		private IEnumerable<PasswordData> ParsePasswordData(string text)
		{
			var lines = text.Split('\n');

			foreach (var line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line))
					yield return PasswordData.Parse(line);
			}
		}

		public void SavePasswordData(IEnumerable<PasswordData> passwordDatas)
		{
			var text = string.Join('\n', passwordDatas.Select((user) => user.ToString()));

			var file = Application.OpenFile("/etc/shadow");
			file.WriteAllText(text);
			file.Close(Application);
		}
	}
}