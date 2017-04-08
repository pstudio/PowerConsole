using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Variables;

namespace pstudio.PowerConsole.Context
{
    /// <summary>
    /// Base interface for a <see cref="PowerConsole"/> context.
    /// A context holds the information needed by the Power Console to execute its job.
    /// E.g. a context defines the <see cref="pstudio.PowerConsole.Command.Command"/>'s that the Power Console can run.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Should the Power Console save the result of the last execution to a predefined variable.
        /// </summary>
        bool UseLastResultVariable { get; }

        /// <summary>
        /// The variable identifier name to save the latest result in.
        /// </summary>
        string LastResultVariableIdentifier { get; }

        CommandContext CommandContext { get; }
        VariableContext VariableContext { get; }
    }

}
