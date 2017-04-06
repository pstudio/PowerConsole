namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Multiply two doubles to each other.
    /// </summary>
    [Command("Multiply", "Number")]
    public class MultiplyNumberCommand : Command
    {
        [Parameter(Position = 0, Mandatory = true, AllowPipe = true)]
        public double A { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double B { get; set; }

        public override object Process()
        {
            return A * B;
        }
    }
}
