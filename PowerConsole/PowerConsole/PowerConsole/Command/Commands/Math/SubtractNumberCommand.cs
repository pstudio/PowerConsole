namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Subtract two doubles from each other.
    /// </summary>
    [Command("Subtract", "Number")]
    public class SubtractNumberCommand : Command
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double B { get; set; }

        public override object Process()
        {
            return A - B;
        }
    }
}
