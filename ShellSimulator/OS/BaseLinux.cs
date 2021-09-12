using ShellSimulator.Applications;
using ShellSimulator.FS;

namespace ShellSimulator.OS
{
    public class BaseLinux : OS
    {
        public override void Install(FileSystem fs)
        {
            CreateRootDirectories(fs);
            CreateUsrDirectories(fs);

            InstallApplications(fs);

            CreateHomeDirectory(fs);
        }

        private void CreateRootDirectories(FileSystem fs)
        {
            fs.CreateNullDirectory("/boot");
            fs.CreateNullDirectory("/dev");
            fs.CreateNullDirectory("/etc");
            fs.CreateDirectory("/home");
            fs.CreateDirectory("/mnt");
            fs.CreateNullDirectory("/opt");
            fs.CreateDirectory("/proc", DirectoryType.Processes);
            fs.CreateNullDirectory("/run");
            fs.CreateDirectory("/tmp");
            fs.CreateDirectory("/usr");
            fs.CreateNullDirectory("/var");
        }

        private void CreateUsrDirectories(FileSystem fs)
        {
            fs.CreateDirectory("/usr/bin");
            fs.CreateNullDirectory("/usr/include");
            fs.CreateNullDirectory("/usr/lib");
            fs.CreateNullDirectory("/usr/lib32");
            fs.CreateNullDirectory("/usr/local");
            fs.CreateNullDirectory("/usr/share");
            fs.CreateNullDirectory("/usr/src");
        }

        private void InstallApplications(FileSystem fs)
        {
            fs.InstallApplication("usr/bin/bash", (s) => new Bash(s));
            fs.InstallApplication("usr/bin/sh", (s) => new Bash(s));

            fs.InstallApplication("/usr/bin/clear", (s) => new Clear(s));
            fs.InstallApplication("/usr/bin/echo", (s) => new Echo(s));
            fs.InstallApplication("/usr/bin/whoami", (s) => new WhoAmI(s));
            fs.InstallApplication("/usr/bin/export", (s) => new Export(s));
            //fs.InstallApplication("/usr/bin/alias", (s) => new Alias(s)); // I don't want to implement this yet
            fs.InstallApplication("/usr/bin/ls", (s) => new LS(s));
            fs.InstallApplication("/usr/bin/cd", (s) => new CD(s));
            fs.InstallApplication("/usr/bin/mkdir", (s) => new MKDir(s));
            fs.InstallApplication("/usr/bin/touch", (s) => new Touch(s));
            fs.InstallApplication("/usr/bin/cat", (s) => new Cat(s));
            fs.InstallApplication("/usr/bin/exit", (s) => new Exit(s));
            fs.InstallApplication("/usr/bin/test", (s) => new Test(s));
            fs.InstallApplication("/usr/bin/grep", (s) => new Grep(s));
        }

        private void CreateHomeDirectory(FileSystem fs)
        {
            string homeDir = fs.AppendPaths("/home", fs.Shell.Username);

            fs.CreateDirectory(fs.AppendPaths(homeDir, "Desktop"));
            fs.CreateDirectory(fs.AppendPaths(homeDir, "Documents"));
            fs.CreateDirectory(fs.AppendPaths(homeDir, "Downloads"));
            fs.CreateDirectory(fs.AppendPaths(homeDir, "Music"));
            fs.CreateDirectory(fs.AppendPaths(homeDir, "Pictures"));
        }

        public override void Setup(Shell shell)
        {
            shell.Path = new string[] { "/usr/bin/" };
        }

        public override void StartRootProcess(Shell shell)
        {
            Bash bash = new Bash(shell);
            bash.Start(null);
        }
    }
}