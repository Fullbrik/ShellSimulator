using System;
using System.Linq;
using System.Threading.Tasks;
using ShellSimulator;
using ShellSimulator.OS.Simnix;
using OperatingSystem = ShellSimulator.OperatingSystem;

class ConsoleKeyboard : ShellSimulator.Hardware.Keyboard
{
	public async override void OnConnect(OperatingSystem os)
	{
		while (true || os.IsRunning)
		{
			await Task.Run(() =>
			{
				var key = Console.ReadKey(true);
				KeyPressed(key);
			});
		}
	}
}

class ConsoleTerminal : ShellSimulator.Hardware.Terminal
{
	int cursorPositionX = 0;
	int cursorPositionY = 0;

	public override int GetTerminalHeight()
	{
		return Console.WindowHeight;
	}

	public override int GetTerminalWidth()
	{
		return Console.WindowWidth;
	}

	public override void OnConnect(OperatingSystem os)
	{
		Console.Clear();
	}

	public override void SetCharacterUnderCursor(char c)
	{
		Console.Write(c);
		ResetCursor();
	}

	public override void SetCursorPosition(int x, int y)
	{
		cursorPositionX = x;
		cursorPositionY = y;
		ResetCursor();
	}

	private void ResetCursor()
	{
		Console.SetCursorPosition(cursorPositionX, cursorPositionY);
	}
}

class TestApp : Application
{
	public override string Name => "Test";

	protected async override Task<int> Main(string[] args)
	{
		PrintAllSubDirectories(OS, "/", "");

		//await Task.Delay(1);

		PrintFLN(string.Join(',', OS.Processes.Select((p) => p.Name)));

		PrintFLN("Hello World!");

		PrintF("Whats your name?: ");
		string name = await ReadLine();

		PrintFLN("Hi {0}!", name);

		await ReadLine();

		return 0;
	}

	private void PrintAllSubDirectories(OperatingSystem os, string currentPath, string depth)
	{
		var dirs = os.GetAllSubDirectories(currentPath);
		var files = os.GetAllFilesInDirectory(currentPath);

		PrintFLN(depth + currentPath);

		foreach (var dir in dirs)
		{
			PrintAllSubDirectories(os, currentPath + dir + "/", depth + "\t");
		}

		foreach (var file in files)
		{
			PrintFLN(depth + "\t" + currentPath + file);
		}
	}
}

static class Program
{
	static void Main(string[] args)
	{
		SimnixOS os = new SimnixOS();

		os.ConnectDevice(new ConsoleKeyboard());
		os.ConnectDevice(new ConsoleTerminal());

		os.Install();

		var file = os.OpenFile("/home/hello.txt", null);
		file.WriteAllText("Hello World!");
		file.Close(null);

		var task = os.Run();

		var deamon = os.GetDaemon<TerminalDaemon>();

		TestApp app = new TestApp();
		var appTask = os.StartApplication(app, null, deamon);

		deamon.PipeTo = app;

		appTask.Wait();

		deamon.PipeTo = null;

		task.Wait();

		os.Shutdown();
	}


}
