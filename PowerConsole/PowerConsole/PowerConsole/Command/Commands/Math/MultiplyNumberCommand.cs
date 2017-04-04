namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Multiply two doubles to each other.
    /// </summary>
    [Command("Multiply", "Number")]
    public class MultiplyNumberCommand : Command
    {
        [Parameter(Position = 0)]
        public double A { get; set; }

        [Parameter(Position = 1)]
        public double B { get; set; }

        public override object Process()
        {
            return A * B;
        }
    }
}
