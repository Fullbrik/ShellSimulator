using System;
using System.Linq;
using System.Threading.Tasks;
using ShellSimulator;
using ShellSimulator.Impl.Console;
using ShellSimulator.OS.Simnix;
using OperatingSystem = ShellSimulator.OperatingSystem;

class TestApp : Application
{
	public override string Name => "Test";

	protected async override Task<int> Main(string[] args)
	{
		//PrintAllSubDirectories(OS, "/", "");

		//PrintFLN(string.Join(',', OS.Processes.Select((p) => p.Name)));

		PrintFLN("Hello World!");

		PrintF("Whats your name?: ");
		string name = await ReadLine();

		//PrintFLN("Hi {0}!", name);

		// string fileText = await ReadLine();

		// var file = OS.OpenFile("/usr/text.txt", this);
		// file.SetReadPosition(0);
		// file.WriteAllText(fileText);
		// file.Close(this);

		// file = OS.OpenFile("/usr/text.txt", this);
		// file.SetReadPosition(0);
		// var text = file.ReadAllText();

		//PrintFLN(text);

		//PrintFLN("");

		//PrintAllSubDirectories(OS, "/", "");

		for (int i = 0; i < 200; i++)
		{
			PrintC(i.ToString().Last());
		}

		return 0;
	}

	protected override void ReceiveSignal(ApplicationSignal signal)
	{
		throw new NotImplementedException();
	}

	private void PrintAllSubDirectories(string currentPath, string depth)
	{
		var dirs = GetAllSubDirectories(currentPath);
		var files = GetAllFilesInDirectory(currentPath);

		PrintFLN(depth + currentPath);

		foreach (var dir in dirs)
		{
			PrintAllSubDirectories(currentPath + dir + "/", depth + "\t");
		}

		foreach (var file in files)
		{
			PrintFLN(depth + "\t" + currentPath + file);
		}
	}
}

static class Program
{
	static async Task Main(string[] args)
	{
		SimnixOS os = new SimnixOS();

		os.Install();

		os.ConnectDevice(new ConsoleKeyboard());
		os.ConnectDevice(new ConsoleTerminal());

		await os.Run();
	}
}
