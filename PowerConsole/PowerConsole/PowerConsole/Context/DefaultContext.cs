namespace pstudio.PowerConsole.Context
{
    public class DefaultContext : IContext
    {
        public bool UseLastResultVariable => true;
        public string LastResultVariableIdentifier => "last";
    }

}
