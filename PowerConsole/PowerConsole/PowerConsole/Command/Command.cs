using pstudio.PowerConsole.Host;

namespace pstudio.PowerConsole.Command
{
    public abstract class Command
    {
        internal IHost _host;
        protected IHost Host => _host;

        public abstract object Process();
    }

}
