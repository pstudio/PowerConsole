namespace pstudio.PowerConsole.Parser
{
    public class ParseType //TODO: Changed from internal tu public for debugging reasons. Change class back to internal when done.
    {
        public enum Type
        {
            Unknown,
            Identifier,
            String,
            Number,
            Variable,
            Command,
            Reflection,
            PipeChain,
            Assignment
        }

        public Type ParsedType { get; }
        public object Value { get; }

        public ParseType(object val, Type type)
        {
            Value = val;
            ParsedType = type;
        }
    }

}
