using System.Runtime.ConstrainedExecution;

namespace pstudio.PowerConsole.Parser
{
    public class Command //TODO: Changed from internal tu public for debugging reasons. Change class back to internal when done.
    {
        public string CommandName { get; }
        public ParseType[] Arguments { get; }

        public Command(string command, ParseType[] arguments)
        {
            CommandName = command;
            Arguments = arguments;
        }
    }

    public class Assignment //TODO: Changed from internal tu public for debugging reasons. Change class back to internal when done.
    {
        public string Variable { get; }
        public ParseType Value { get; }

        public Assignment(string variable, ParseType value)
        {
            Variable = variable;
            Value = value;
        }
    }

    public class Reflection //TODO: Changed from internal tu public for debugging reasons. Change class back to internal when done.
    {
        public string Variable { get; }
        public string[] Identifiers { get; }

        public Reflection(string variable, string[] identifiers)
        {
            Variable = variable;
            Identifiers = identifiers;
        }
    }

    public class PipeChain //TODO: Changed from internal tu public for debugging reasons. Change class back to internal when done.
    {
        public Command[] Commands { get; }

        public PipeChain(Command[] commands)
        {
            Commands = commands;
        }
    }
}
