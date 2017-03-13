using System;
using System.Collections.Generic;
using System.Linq;
using pstudio.PowerConsole.Parser;

namespace pstudio.PowerConsole
{
    public class PowerConsole
    {
        private readonly List<ParseType> _history = new List<ParseType>(20);

        public PowerConsole()
        {
            
        }

        public string Execute(string input)
        {
            try
            {
                var parseResult = PowerParser.ParseInput(input);
                _history.Add(parseResult);
                return parseResult.ParsedType.ToString();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string[] GetHistory(int items = 0)
        {
            if (items <= 0)
            {
                return _history.Select(item => item.Value.ToString()).ToArray();
            }

            return _history.GetRange(_history.Count - items - 1, items).Select(item => item.Value.ToString()).ToArray();
        }
    }
}
