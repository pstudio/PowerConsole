﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pstudio.PowerConsole.Command
{
    /// <summary>
    /// A Command Context holds the list of all <see cref="Command"/>'s that are available for the <see cref="PowerConsole"/>.
    /// </summary>
    public class CommandContext
    {
        //private readonly Dictionary<string, Type> _commandTypes; 
        private readonly Dictionary<string, Command> _commands;

        /// <summary>
        /// The number of commands registered.
        /// </summary>
        public int CommandsRegistered => _commands.Count;

        /// <summary>
        /// Indicates if a <see cref="Command"/> has been registered.
        /// </summary>
        /// <typeparam name="T">The command type</typeparam>
        /// <returns>True if command is registered</returns>
        public bool IsCommandRegistered<T>() where T : Command => _commands.Values.Any(c => c is T);

        /// <summary>
        /// Indicates if a <see cref="Command"/> has been registered.
        /// </summary>
        /// <param name="command">The command name (e.g. Add-Number)</param>
        /// <returns>True if command is registered</returns>
        public bool IsCommandRegistered(string command) => _commands.ContainsKey(command.ToUpper());

        public CommandContext()
        {
            //_commandTypes = new Dictionary<string, Type>();
            _commands = new Dictionary<string, Command>();
        }

        public void RegisterCommand<T>() where T : Command
        {
            //TODO: Consider Lazy initialization of Commands
            //var command = Activator.CreateInstance(typeof (T)) as Command;
            var commandAttribute = (CommandAttribute) Attribute.GetCustomAttribute(typeof (T), typeof (CommandAttribute));

            if (commandAttribute == null)
                throw new MissingCommandAttributeException($"The type parameter T: {typeof (T)} is missing the Command attribute.");

            var commandName = $"{commandAttribute.Verb.ToUpper()}-{commandAttribute.Noun.ToUpper()}";

            if (_commands.ContainsKey(commandName))
                throw new CommandNameRegisteredException($"A command is already registered with the name: '{commandName}'");

            //_commandTypes[verbNoun] = typeof (T);

            //Commands[verbNoun] = new CommandInfo(command);

            Command command;
            try
            {
                command = (Command) Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            { 
                throw new CreateCommandFailedException($"Could not create the command of type: '{typeof(T)}'", e);
            }

            command.Initialize();
            _commands[commandName] = command;
        }

        internal Command this[string commandName] => _commands[commandName];
        //{
        //    get
        //    {
        //        if (_commands.ContainsKey(commandName)) return _commands[commandName];

        //        if (!_commandTypes.ContainsKey(commandName))
        //            throw new MissingCommandException($"The command: '{commandName}' does not exist.");

        //        var command = Activator.CreateInstance(_commandTypes[commandName]) as Command;
        //        command.Initialize();
        //        _commands[commandName] = command;

        //        return command;
        //    }
        //}
    }

    //internal class CommandInfo
    //{
    //    public Command Command { get; }
    //    public List<CommandProperty> PositionalProperties { get; }
    //    public List<CommandProperty> NamedProperties { get; }  

    //    public CommandInfo(Command command)
    //    {
    //        Command = command;

    //        var properties =
    //            (from prop in Command.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //             where prop.CanRead && prop.CanWrite
    //             let attr = prop.GetCustomAttributes(typeof(ParameterAttribute), true)
    //             let def = prop.GetGetMethod(false).Invoke(Command, null)
    //             where attr.Length == 1
    //             select new CommandProperty(prop, (ParameterAttribute)attr.First(), def)).ToList();

    //        PositionalProperties = properties.Where(prop => prop.Attribute.Position >= 0).OrderBy(prop => prop.Attribute.Position).ToList();
    //        VerifyPositionalProperties();

    //        NamedProperties = properties.Except(PositionalProperties).ToList();
    //        VerifyNamedProperties();
    //    }

    //    public void Reset()
    //    {
    //        // TODO: Considering Positional Properties are currently mandatory this is unnecessary. 
    //        // Check if positional properties are mandatory in PowerShell.
    //        // Consider if positional properties should be mandatory.
    //        foreach (var property in PositionalProperties)
    //        {
    //            property.Property.SetValue(Command, property.Default, null);
    //        }

    //        foreach (var property in NamedProperties)
    //        {
    //            property.Property.SetValue(Command, property.Default, null);
    //        }
    //    }

    //    private void VerifyPositionalProperties()
    //    {
    //        // Verify positional properties start at position 0 and rises monotonically strictly with 1
    //        sbyte positionVerifier = 0;
    //        var positionIncorrect =
    //            PositionalProperties.Where(
    //                positionalProperty => positionalProperty.Attribute.Position != positionVerifier++).ToList();
    //        if (positionIncorrect.Any())
    //        {
    //            throw new InvalidParameterPositionException($"Invalid ParameterAttribute.Position value {positionIncorrect.First().Attribute.Position} for property '{positionIncorrect.First().Property.Name}'");
    //        }
    //    }

    //    private void VerifyNamedProperties()
    //    {
    //        // Boolean properties are treated as flag. If the user names the property it is set to true. Otherwise it is false.
    //        // Hence boolean properties can't be mandatory since that would result in them always being true. 
    //        var invalidProperties = NamedProperties.Where(prop => prop.Property.PropertyType == typeof (bool) && prop.Attribute.Mandatory).ToList();
    //        if (invalidProperties.Count > 0)
    //            throw new InvalidNamedParameterException($"Boolean property '{invalidProperties[0].Property.Name}' cannot be mandatory.");
    //    }

    //    //private void SetupProperties()
    //    //{
    //    //    var properties =
    //    //        (from prop in Command.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //    //        where prop.CanRead && prop.CanWrite
    //    //        let attr = prop.GetCustomAttributes(typeof (ParameterAttribute), true)
    //    //        where attr.Length == 1
    //    //        select new CommandProperty(prop, (ParameterAttribute) attr.First())).ToList();

    //    //    PositionalProperties = properties.Where(prop => prop.Attribute.Position >= 0).OrderBy(prop => prop.Attribute.Position).ToList();


    //    //}

    //    public class CommandProperty
    //    {
    //        public PropertyInfo Property { get; }
    //        public ParameterAttribute Attribute { get; }
    //        public object Default { get; }

    //        public CommandProperty(PropertyInfo property, ParameterAttribute attribute, object @default)
    //        {
    //            Property = property;
    //            Attribute = attribute;
    //            Default = @default;
    //        }
    //    }
    //}
}
