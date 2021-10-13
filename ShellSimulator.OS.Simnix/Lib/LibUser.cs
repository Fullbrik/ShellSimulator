using System.Collections.Generic;
using System.Linq;

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
                    throw new System.Exception("Invalid parameters in file");
                }
            }

            public UserData(string name, int uid, string homeDir, string shell)
            {
                Name = name;
                UID = uid;
                HomeDirectory = homeDir;
                Shell = shell;
            }
        }

        public LibUser(Application application) : base(application)
        {
        }

        public override string Name => "User";

        public bool HasUser(string username)
        {
            var users = Loadusers();

            // We loop through each user. if we find one that has the right username, we return true right away. Otherwise, we return false
            foreach (var user in users)
                if (user.Name == username) return true;

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
            var file = Application.OS.OpenFile("/etc/passwd", Application);
            string text = file.ReadAllText();
            file.Close(Application);

            return text;
        }

        private IEnumerable<UserData> ParseUserData(string text)
        {
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                yield return UserData.Parse(line);
            }
        }
    }
}