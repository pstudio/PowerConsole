using System.Collections.Generic;

namespace pstudio.PowerConsole.Variables
{
    public class VariableContext
    {
        private Dictionary<string, object> _variables;

        public VariableContext()
        {
            _variables = new Dictionary<string, object>();
        }

        public object this[string variable]
        {
            get { return _variables[variable]; }

            set { _variables[variable] = value; }
        }

        public bool VariableExists(string variable) => _variables.ContainsKey(variable);
    }

}
