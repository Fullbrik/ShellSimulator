using System;
using System.IO;
using System.Text;

namespace ShellSimulator
{
    public class STDIn : TextReader
    {
        public Func<int> RequestKey;

        public override int Read()
        {
            if (RequestKey == null) return -1;
            else return RequestKey();
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
    }
}