using System.Threading.Tasks;
using ShellSimulator.Hardware;

namespace ShellSimulator
{
	/// <summary>
	/// This class does a few things:
	/// <list type="number"> 
	/// <item>Takes a programs STDOut, and renders it on a terminal window.</item>
	/// <item>Sends data from a keyboard to a application. </item>
	/// </list>
	/// </summary>
	public class TerminalDaemon : Daemon
	{
		public override bool IsRunning => isRunning;

		public override string Name => "Terminal Daemon";

		private bool isRunning = false;

		Terminal terminal;
		Keyboard keyboard;

		int cursorX;
		int cursorY;

		void UpdateCursor()
		{
			if (cursorX < 0)
			{
				cursorX = 0;
				cursorY--;
			}
			else if (cursorX > terminal.GetTerminalWidth() - 1) // New line from overflow
			{
				cursorX = 0;
				cursorY++;
			}

			if (cursorY < 0)
			{
				cursorY = 0;
			}
			else if (cursorY > terminal.GetTerminalHeight() - 1) // Scroll the terminal
			{
				terminal.ScrollLine();
				cursorY = terminal.GetTerminalHeight() - 1;
			}

			terminal.SetCursorPosition(cursorX, cursorY);
		}


		protected async override Task<int> Main(string[] args)
		{
			isRunning = true;

			terminal = OS.GetDevice<Terminal>();
			keyboard = OS.GetDevice<Keyboard>();

			keyboard.OnKeyPressed += OnKeyboardKeyPressed;

			await ManageTerminal(); // Will run until a stop is requested

			keyboard.OnKeyPressed -= OnKeyboardKeyPressed; // Unbind so no memory leak. Memory leak is allegedly bad or something.

			return 0;
		}

		private void OnKeyboardKeyPressed(object sender, KeyboardKeyPressedEventArgs e)
		{
			if (e.KeyInfo.Key == System.ConsoleKey.Backspace)
			{
				// Backspace
				if (PrintBackspace()) // Skip if there is nothing to backspace in the buffer.
				{
					cursorX--;
					UpdateCursor();
					terminal.SetCharacterUnderCursor(' ');
				}
			}
			else if (e.KeyInfo.Key == System.ConsoleKey.Enter)
			{
				terminal.SetCharacterUnderCursor(' ');
				cursorX = 0;
				cursorY++;
				UpdateCursor();

				PrintC('\n');
			}
			else
			{
				terminal.SetCharacterUnderCursor(e.KeyInfo.KeyChar);
				cursorX++;
				UpdateCursor();

				PrintC(e.KeyInfo.KeyChar);
			}
		}

		private async Task ManageTerminal()
		{
			while (IsRunning)
			{
				await Task.Delay(1); // This fixes bugs. Leave it in please!

				while (IsCharAvailable())
				{
					char c = await ReadChar();

					if (c == '\n')
					{
						cursorX = 0;
						cursorY++;
					}
					else
					{
						terminal.SetCharacterUnderCursor(c);
						cursorX++;
					}
					UpdateCursor();
				}
			}
		}

		public override void RequestStop()
		{
			isRunning = false;
		}

		protected override void ClearScreen()
		{
			terminal.Clear();
			cursorX = 0;
			cursorY = 0;
			UpdateCursor();
		}

		public async Task WaitForBuffer()
		{
			while (IsCharAvailable())
				await Task.Delay(1);
		}
	}
}