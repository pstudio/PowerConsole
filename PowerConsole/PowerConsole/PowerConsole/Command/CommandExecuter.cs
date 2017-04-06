using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using pstudio.PowerConsole.Host;
using pstudio.PowerConsole.Parser;
using UniRx.InternalUtil;

namespace pstudio.PowerConsole.Command
{
    internal static class CommandExecuter
    {
        public static object Execute(Parser.Command parseCommand, CommandContext commandContext, IHost host,
            Dictionary<string, object> variables, object pipeValue = null)
        {
            // TODO: When the project is in a mature state, consider alternative ways to assign values to properties
            // Build setters in CommandContext.CommandInfo
            // http://stackoverflow.com/a/16082916
            // http://geekswithblogs.net/Madman/archive/2008/06/27/faster-reflection-using-expression-trees.aspx
            // http://www.palmmedia.de/Blog/2012/2/4/reflection-vs-compiled-expressions-vs-delegates-performance-comparision

            var command = commandContext[parseCommand.CommandName.ToUpper()];
            //if (command == null)
            //{
            //    throw new MissingCommandException($"The command: '{parseCommand.CommandName}' does not exist."); 
            //}

            command.Reset();

            var mandatoryProperties =
                command.PositionalProperties.Union(command.NamedProperties)
                    .Where(prop => prop.Attribute.Mandatory)
                    .ToList();

            // Handle pipe value
            Command.CommandProperty pipeProperty = null;
            if (pipeValue != null)
            {
                pipeProperty =
                    command.PositionalProperties.FirstOrDefault( // First check positional parameters for a place to put piped value
                        prop =>
                            prop.Attribute.AllowPipe &&
                            (prop.Property.PropertyType.IsInstanceOfType(pipeValue) ||
                             IsNumericAssignment(prop.Property.PropertyType, pipeValue.GetType()))) ??
                    command.NamedProperties.FirstOrDefault( // Otherwise check named parameters for a place to put piped value
                                 prop =>
                                     prop.Attribute.AllowPipe &&
                                     (prop.Property.PropertyType.IsInstanceOfType(pipeValue) ||
                                      IsNumericAssignment(prop.Property.PropertyType, pipeValue.GetType())));

                if (pipeProperty == null)
                    throw new InvalidPipeArgumentTypeException(
                        $"There is no parameter in command: '{parseCommand.CommandName}' that accepts pipe value of type: '{pipeValue.GetType()}'");
                
                pipeProperty.Property.SetValue(command, Convert.ChangeType(pipeValue, pipeProperty.Property.PropertyType), null);
                if (pipeProperty.Attribute.Mandatory)
                    mandatoryProperties.Remove(pipeProperty);
                
            }

            var position = 0;

            for (var i = 0; i < parseCommand.Arguments.Length; i++)
            {
                var parseType = parseCommand.Arguments[i];

                // Handle positional property
                if (parseType.ParsedType != ParseType.Type.Parameter)
                {
                    if (position >= command.PositionalProperties.Count)
                        throw new UnexpectedPositionalArgument($"Unexpected positional argument '{parseType.Value}' received.");

                    var property = command.PositionalProperties[position++];

                    // Check to see if parameter has already been assigned through piping
                    if (pipeProperty == property)
                    {
                        if (position >= command.PositionalProperties.Count)
                            throw new UnexpectedPositionalArgument($"Unexpected positional argument '{parseType.Value}' received.");

                        property = command.PositionalProperties[position++];
                    }

                    TrySetPropertyValue(command, property.Property, parseType, variables);

                    if (property.Attribute.Mandatory)
                        mandatoryProperties.Remove(property);

                    continue;;
                }

                // Handle named property
                var namedProperty = command.NamedProperties.First(prop => string.Equals(prop.Property.Name, ((string)parseType.Value), StringComparison.CurrentCultureIgnoreCase));

                if (namedProperty == null)
                    throw new InvalidNamedParameterException($"The named parameter '{parseType.Value}' is not recognized.");

                // Check to see if parameter has already been assigned through piping
                if (pipeProperty == namedProperty)
                    throw new NamedParameterAssignedThroughPiping($"The named parameter '{parseType.Value}' has already been assigned through piping.");

                // Check if the property is a flag
                if (namedProperty.Property.PropertyType == typeof(bool))
                {
                    namedProperty.Property.SetValue(command, true, null);
                    continue;
                }

                // If the property is not a flag we must check the next argument for the value to assign to the property

                if (++i >= parseCommand.Arguments.Length)
                    throw new MissingNamedArgumentException($"The named parameter '{parseType.Value}' is missing an argument of type '{namedProperty.Property.PropertyType}'.");

                var argumentType = parseCommand.Arguments[i];

                TrySetPropertyValue(command, namedProperty.Property, argumentType, variables);

                if (namedProperty.Attribute.Mandatory)
                    mandatoryProperties.Remove(namedProperty);
            }

            //// Positional properties are mandatory and must be set by the user
            //if (command.PositionalProperties.Count() > parseCommand.Arguments.Length)
            //{
            //    throw new MissingPositionalArgumentException($"The command '{parseCommand.CommandName}' expects {command.PositionalProperties.Count()} positional arguments. Only {parseCommand.Arguments.Length} arguments were provided.");
            //}

            //foreach (var positionalProperty in command.PositionalProperties)
            //{
            //    var parseType = parseCommand.Arguments[positionalProperty.Attribute.Position];

            //    TrySetPropertyValue(command, positionalProperty.Property, parseType, variables);
            //}

            //// Keep track of mandatory named properties
            ////var mandatoryProperties = command.NamedProperties.Where(prop => prop.Attribute.Mandatory).ToList();

            //for (var i = command.PositionalProperties.Count; i < parseCommand.Arguments.Length; i++)
            //{
            //    var parseType = parseCommand.Arguments[i];

            //    if (parseType.ParsedType != ParseType.Type.Parameter)
            //        throw new InvalidArgumentTypeException($"A named parameter identifier was expected but received argument value '{parseType.Value}'");

            //    var namedProperty = command.NamedProperties.First(prop => string.Equals(prop.Property.Name, ((string) parseType.Value), StringComparison.CurrentCultureIgnoreCase));

            //    if (namedProperty == null)
            //        throw new InvalidNamedParameterException($"The named parameter '{parseType.Value}' is not recognized.");

            //    // Check if the property is a flag
            //    if (namedProperty.Property.PropertyType == typeof(bool))
            //    {
            //        namedProperty.Property.SetValue(command, true, null);
            //        continue;
            //    }

            //    // If the property is not a flag we must check the next argument for the value to assign to the property

            //    if (++i >= parseCommand.Arguments.Length)
            //        throw new MissingNamedArgumentException($"The named parameter '{parseType.Value}' is missing an argument of type '{namedProperty.Property.PropertyType}'.");

            //    var argumentType = parseCommand.Arguments[i];

            //    TrySetPropertyValue(command, namedProperty.Property, argumentType, variables);

            //    // Remove property from list to indicate it has been set
            //    mandatoryProperties.Remove(namedProperty);
            //}

            // Make sure all mandatory parameters are set
            if (mandatoryProperties.Count > 0)
                throw new MissingMandatoryParameterException($"The mandatory parameter '{mandatoryProperties.First().Property.Name}' is missing.");

            command._host = host;
            return command.Process();
        }

        private static void TrySetPropertyValue(Command command, PropertyInfo property, ParseType parseType, Dictionary<string, object> variables)
        {
            if (parseType.ParsedType == ParseType.Type.Parameter)
                throw new InvalidArgumentTypeException($"An argument value type (string, number, variable) was expected but received a named parameter identifier '{parseType.Value}'");

            var parameter = parseType.ParsedType == ParseType.Type.Variable ? variables[(string)parseType.Value] : parseType.Value;

            if (!property.PropertyType.IsInstanceOfType(parameter) && !IsNumericAssignment(property.PropertyType, parameter.GetType()))
                throw new InvalidArgumentTypeException($"Expected type {property.PropertyType} but received {parameter.GetType()}");

            property.SetValue(command, Convert.ChangeType(parameter, property.PropertyType), null);
        }

        // http://stackoverflow.com/a/1750002
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>()
        {
            typeof(byte), typeof(char), typeof(decimal), typeof(double),
            typeof(float), typeof(int), typeof(long), typeof(sbyte),
            typeof(short), typeof(uint), typeof(ulong), typeof(ushort)
        };

        private static bool IsNumericAssignment(Type target, Type value) =>
            NumericTypes.Contains(target) && NumericTypes.Contains(value);

        public static object ExecuteChain(PipeChain pipeChain, CommandContext commandContext, IHost host, Dictionary<string, object> variables)
        {
            var result = Execute(pipeChain.Commands[0], commandContext, host, variables);
            for (var i = 1; i < pipeChain.Commands.Length; i++)
            {
                result = Execute(pipeChain.Commands[i], commandContext, host, variables, result);
            }

            return result;
        }
    }

}
