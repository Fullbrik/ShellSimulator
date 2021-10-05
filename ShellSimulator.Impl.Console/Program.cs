using System;
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
		PrintFLN("Hello World!");

		PrintF("Whats your name?: ");
		string name = await ReadLine();

		PrintFLN("Hi {0}!", name);

		return 0;
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

		PrintAllSubDirectories(os, "/", "");

		//TestApp app = new TestApp();
		//var appTask = os.StartApplication(app, null, td);

		//td.PipeTo = app;

		//appTask.Wait();

		//td.PipeTo = null;

		task.Wait();

		os.Shutdown();
	}

	private static void PrintAllSubDirectories(OperatingSystem os, string currentPath, string depth)
	{
		var dirs = os.GetAllSubDirectories(currentPath);
		var files = os.GetAllFilesInDirectory(currentPath);

		Console.WriteLine(depth + currentPath);

		foreach (var dir in dirs)
		{
			PrintAllSubDirectories(os, currentPath + dir + "/", depth + "\t");
		}

		foreach (var file in files)
		{
			Console.WriteLine(depth + "\t" + currentPath + file);
		}
	}
}
