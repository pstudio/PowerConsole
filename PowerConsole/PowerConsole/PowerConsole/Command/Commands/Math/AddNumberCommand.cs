namespace pstudio.PowerConsole.Command.Commands.Math
{
    /// <summary>
    /// Add two doubles to each other.
    /// </summary>
    [Command("Add", "Number")]
    public class AddNumberCommand : Command
    {
        [Parameter(Position = 0, Mandatory = true)]
        public double A { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public double B { get; set; }

        public override object Process()
        {
            return A + B;
        }
    }
}
