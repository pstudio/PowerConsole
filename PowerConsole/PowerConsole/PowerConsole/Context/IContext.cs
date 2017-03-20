namespace pstudio.PowerConsole.Context
{
    public interface IContext
    {
        bool UseLastResultVariable { get; }
        string LastResultVariableIdentifier { get; }
    }

}
