using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Variables;

namespace pstudio.PowerConsole.Context
{
    public class DefaultContext : IContext
    {
        public bool UseLastResultVariable => true;
        public string LastResultVariableIdentifier => "last";
        public CommandContext CommandContext { get; }
        public VariableContext VariableContext { get; }

        public DefaultContext()
        {
            CommandContext = new CommandContext();
            VariableContext = new VariableContext();
        }
    }
}