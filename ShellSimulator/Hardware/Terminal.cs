namespace ShellSimulator.Hardware
{
    public abstract class Terminal : Device
    {
        public abstract int GetTerminalWidth();
        public abstract int GetTerminalHeight();

        public abstract void SetCursorPosition(int x, int y);
        public abstract void SetCharacterUnderCursor(char c);

        public abstract void ScrollLine();
    }
}