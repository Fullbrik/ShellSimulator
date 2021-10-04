using System;

namespace ShellSimulator.Hardware
{
    public abstract class Keyboard : Device
    {
        public event EventHandler<KeyboardKeyPressedEventArgs> OnKeyPressed;
        protected void KeyPressed(ConsoleKeyInfo keyInfo) { OnKeyPressed?.Invoke(this, new KeyboardKeyPressedEventArgs(keyInfo)); }
    }

    public class KeyboardKeyPressedEventArgs : EventArgs
    {
        public ConsoleKeyInfo KeyInfo { get; }

        public KeyboardKeyPressedEventArgs(ConsoleKeyInfo keyInfo)
        {
            KeyInfo = keyInfo;
        }
    }
}