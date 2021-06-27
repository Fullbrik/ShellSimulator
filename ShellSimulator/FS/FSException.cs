using System;
using System.Runtime.Serialization;

namespace ShellSimulator.FS
{
    public class FSException : Exception
    {
        public FSException()
        {
        }

        public FSException(string message) : base(message)
        {
        }

        public FSException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FSException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}