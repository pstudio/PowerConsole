namespace pstudio.PowerConsole.Parser
{
    internal class ParseType
    {
        public enum Type
        {
            Unknown,
            Identifier,
            String,
            Number,
            Variable,
            Parameter,
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
