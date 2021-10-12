using System;
using System.Threading.Tasks;

namespace ShellSimulator.Impl.Console
{
	public class ConsoleKeyboard : ShellSimulator.Hardware.Keyboard
	{
		public override void OnConnect(OperatingSystem os)
		{

		}

		public async override void OnStart(OperatingSystem os)
		{
			while (os.IsRunning)
			{
				await Task.Run(() =>
				{
					var key = System.Console.ReadKey(true);
					KeyPressed(key);
				});
			}
		}
	}

	public class ConsoleTerminal : ShellSimulator.Hardware.Terminal
	{
		int cursorPositionX = 0;
		int cursorPositionY = 0;

		public override void Clear()
		{
			System.Console.Clear();
		}

		public override int GetTerminalHeight()
		{
			return System.Console.WindowHeight;
		}

		public override int GetTerminalWidth()
		{
			return System.Console.WindowWidth;
		}

		public override void OnConnect(OperatingSystem os)
		{
		}

		public override void OnStart(OperatingSystem os)
		{
			System.Console.Clear();
		}

		public override void ScrollLine()
		{
			System.Console.SetCursorPosition(System.Console.BufferWidth, System.Console.BufferHeight);
			System.Console.WriteLine();
			ResetCursor();
		}

		public override void SetCharacterUnderCursor(char c)
		{
			System.Console.Write(c);
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
			System.Console.SetCursorPosition(cursorPositionX, cursorPositionY);
		}
	}
}