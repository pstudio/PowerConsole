﻿using pstudio.PowerConsole.Command;

namespace pstudio.PowerConsole.Context
{
    public class DefaultContext : IContext
    {
        public bool UseLastResultVariable => true;
        public string LastResultVariableIdentifier => "last";
        public CommandContext CommandContext { get; }

        public DefaultContext()
        {
            CommandContext = new CommandContext();
        }
    }
}