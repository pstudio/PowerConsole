namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Subtract two doubles from each other.
    /// </summary>
    [Command("Subtract", "Number")]
    public class SubtractNumberCommand : Command
    {
        [Parameter(Position = 0, Mandatory = true, AllowPipe = true)]
        public double A { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double B { get; set; }

        [Parameter]
        public bool FlipArguments { get; set; }

        public override object Process()
        {
            return FlipArguments ? B - A : A - B;
        }
    }
}
