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
            Command,
            Reflection
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
