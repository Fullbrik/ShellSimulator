using System;
using System.IO;
using System.Text;

namespace ShellSimulator
{
    public class STDIn : TextReader
    {
        public Func<int> RequestKey;

        string buffer = "";

        public override int Read()
        {
            if (RequestKey == null) return -1;
            else if (buffer.Length > 0) return PopBuffer(); //If we have anything in the buffer, we should use that first.
            else return RequestKey();
        }

        private char PopBuffer()
        {
            char next = buffer[0];
            buffer = buffer.Remove(0, 1);
            return next;
        }

        public override string ReadLine()
        {
            string line = null;
            while (line == null)
            {
                line = _ReadLine();
            }
            return line;
        }

        private string _ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int ch = Read();
                if (ch == -1) break;
                if (ch == -2) sb.Length--;
                if (ch == '\r' || ch == '\n')
                {
                    if (ch == '\r' && Peek() == '\n') Read();
                    return sb.ToString();
                }
                if (ch >= 0) sb.Append((char)ch);
            }
            if (sb.Length > 0) return sb.ToString();
            return null;
        }

        public void WriteToBuffer(string text)
        {
            if (text != null)
                buffer += text;
        }
    }
}