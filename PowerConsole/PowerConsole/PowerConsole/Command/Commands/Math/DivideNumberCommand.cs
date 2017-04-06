namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Divide two doubles from each other.
    /// </summary>
    [Command("Divide", "Number")]
    public class DivideNumberCommand : Command
    {
        [Parameter(Position = 0, Mandatory = true, AllowPipe = true)]
        public double A { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double B { get; set; }

        [Parameter]
        public bool FlipArguments { get; set; }

        public override object Process()
        {
            return FlipArguments ? B / A : A / B;
        }
    }
}
