using System;
using System.Collections.Generic;
using System.Linq;
using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Context;
using pstudio.PowerConsole.Host;
using pstudio.PowerConsole.Parser;

namespace pstudio.PowerConsole
{
    public class PowerConsole
    {
        private readonly List<string> _inputHistory = new List<string>(); 
        private readonly List<ParseType> _history = new List<ParseType>();
        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

        private readonly IContext _context;
        private object _lastValue;

        private readonly IHost _host;

        /// <summary>
        /// Creates a new Power Console.
        /// </summary>
        /// <param name="host">The host that will handle string input/output.</param>
        /// <param name="context">The context the console will work in.</param>
        public PowerConsole(IHost host, IContext context)
        {
            _host = host;

            _context = context;
        }

        public void Execute(string input)
        {
            _inputHistory.Add(input);
            /*TODO: Add method to get previous input history
             * Examine Powershell commandlet implementation.
             * should execute just fire a commandlet and then send a callback to the caller (UnityPowerConsole) when the command is done?
             * TODO: Implement first draft of a Command Manager. Load commands from assembly/namespace, allow execution of commands
             * TODO: ////!!!!!!!!Work on piping Next.!!!!!!!!//////////////
             */
            try
            {
                var parseResult = PowerParser.ParseInput(input);

                _lastValue = HandleParseType(parseResult);
                

                //_history.Add(parseResult);
                //_host.Write(parseResult.ParsedType.ToString());
                _host.Write(_lastValue?.ToString() ?? _host.FormatColor("NULL", OutputColorType.Accented)); //TODO: Use output formatters - don't output null - let handlers output null explicit if needed
            }
            catch (PowerParser.IncompleteParseException exception)
            {
                _host.WriteError(
                    $"{exception.Message}\n{_host.FormatColor($"Remainder: {exception.Input.Substring(exception.Position)}", OutputColorType.Default)}");
            }
            catch (Exception exception)
            {
                _host.WriteError($"[{exception}]: {exception.Message}");
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

        /// Grammar: Assignment | PipeChain | Reflection | Command | Variable | QuotedString | Number
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private object HandleParseType(ParseType type)
        {
            switch (type.ParsedType)
            {
                case ParseType.Type.Unknown:
                    throw new Exception("An unknown parse result was returned. This should never happen. Contact the developer.");
                case ParseType.Type.String:
                    return (string)type.Value;
                case ParseType.Type.Number:
                    return (double)type.Value;
                case ParseType.Type.Variable:
                    return HandleVariable(type.Value as string);
                case ParseType.Type.Command:
                    return HandleCommand(type.Value as Parser.Command);
                case ParseType.Type.Reflection:
                    return HandleReflection(type.Value as Reflection);
                case ParseType.Type.PipeChain:
                    return HandlePipeChain(type.Value as PipeChain);
                case ParseType.Type.Assignment:
                    return HandleAssignment(type.Value as Assignment);
                default:
                    throw new ArgumentOutOfRangeException($"Parse result: ({type.ParsedType}) out of range.");
            }
        }
        
        private object HandleAssignment(Assignment assignment)
        {
            if (_context.UseLastResultVariable && assignment.Variable == _context.LastResultVariableIdentifier)
            {
                throw new ArgumentException($"It is not allowed to assign to the Last Result Variable {_host.FormatColor($"'{_context.LastResultVariableIdentifier}'", OutputColorType.Accented)}");
            }

            var value = HandleParseType(assignment.Value);
            _variables[assignment.Variable] = value;
            return value;
        }

        private object HandlePipeChain(PipeChain pipeChain)
        {
            return CommandExecuter.ExecuteChain(pipeChain, _context.CommandContext, _host, _variables);
        }

        private object HandleReflection(Reflection reflection)
        {
            return null;
        }

        private object HandleCommand(Parser.Command command)
        {
            return CommandExecuter.Execute(command, _context.CommandContext, _host, _variables);
        }

        private object HandleVariable(string identifier)
        {
            if (_context.UseLastResultVariable && identifier == _context.LastResultVariableIdentifier)
            {
                return _lastValue;
            }

            if (!_variables.ContainsKey(identifier))
            {
                _variables[identifier] = null;
            }

            return _variables[identifier];
        }

    }
}
