using System.Runtime.ConstrainedExecution;

namespace pstudio.PowerConsole.Parser
{
    internal class Command
    {
        public string CommandName { get; }
        public ParseType[] Arguments { get; }

        public Command(string command, ParseType[] arguments)
        {
            CommandName = command;
            Arguments = arguments;
        }
    }

    internal class Assignment
    {
        public string Variable { get; }
        public ParseType Value { get; }

        public Assignment(string variable, ParseType value)
        {
            Variable = variable;
            Value = value;
        }
    }

    internal class Reflection
    {
        public string Variable { get; }
        public string[] Identifiers { get; }

        public Reflection(string variable, string[] identifiers)
        {
            Variable = variable;
            Identifiers = identifiers;
        }
    }

    internal class PipeChain
    {
        public Command[] Commands { get; }

        public PipeChain(Command[] commands)
        {
            Commands = commands;
        }
    }
}
