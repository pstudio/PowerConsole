using System;
using System.Runtime.Serialization;

namespace pstudio.PowerConsole.Command
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string Verb { get; }
        public string Noun { get; }

        //public Type ReturnType { get; set; }

        public CommandAttribute(string verb, string noun)
        {
            Verb = verb;
            Noun = noun;
        }
    }


    public class MissingCommandAttributeException : Exception
    {
        public MissingCommandAttributeException()
        {
        }

        public MissingCommandAttributeException(string message) : base(message)
        {
        }

        public MissingCommandAttributeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingCommandAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
