using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using pstudio.PowerConsole.Host;

namespace pstudio.PowerConsole.Command
{
    /// <summary>
    /// This is the base class all commands must inherit from.
    /// </summary>
    public abstract class Command
    {
        internal IHost _host;
        protected IHost Host => _host;

        internal List<CommandProperty> PositionalProperties { get; private set; }
        internal List<CommandProperty> NamedProperties { get; private set; }

        protected Command()
        {
            
        }

        public abstract object Process();

        internal void Initialize()
        {
            var properties =
                (from prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 where prop.CanRead && prop.CanWrite
                 let attr = prop.GetCustomAttributes(typeof(ParameterAttribute), true)
                 let def = prop.GetGetMethod(false).Invoke(this, null)
                 where attr.Length == 1
                 select new CommandProperty(prop, (ParameterAttribute)attr.First(), def)).ToList();

            PositionalProperties = properties.Where(prop => prop.Attribute.Position >= 0).OrderBy(prop => prop.Attribute.Position).ToList();
            VerifyPositionalProperties();

            NamedProperties = properties.Except(PositionalProperties).ToList();
            VerifyNamedProperties();
        }

        internal void Reset()
        {
            foreach (var property in PositionalProperties)
            {
                property.Property.SetValue(this, property.Default, null);
            }

            foreach (var property in NamedProperties)
            {
                property.Property.SetValue(this, property.Default, null);
            }
        }

        private void VerifyPositionalProperties()
        {
            // Verify positional properties start at position 0 and rises monotonically strictly with 1
            sbyte positionVerifier = 0;
            var positionIncorrect =
                PositionalProperties.Where(
                    positionalProperty => positionalProperty.Attribute.Position != positionVerifier++).ToList();
            if (positionIncorrect.Any())
            {
                throw new InvalidParameterPositionException($"Invalid ParameterAttribute.Position value {positionIncorrect.First().Attribute.Position} for property '{positionIncorrect.First().Property.Name}'");
            }
        }

        private void VerifyNamedProperties()
        {
            // Boolean properties are treated as flag. If the user names the property it is set to true. Otherwise it is false.
            // Hence boolean properties can't be mandatory since that would result in them always being true. 
            var invalidProperties = NamedProperties.Where(prop => prop.Property.PropertyType == typeof(bool) && prop.Attribute.Mandatory).ToList();
            if (invalidProperties.Count > 0)
                throw new InvalidNamedParameterException($"Boolean property '{invalidProperties[0].Property.Name}' cannot be mandatory.");
        }

        internal class CommandProperty
        {
            public PropertyInfo Property { get; }
            public ParameterAttribute Attribute { get; }
            public object Default { get; }

            public CommandProperty(PropertyInfo property, ParameterAttribute attribute, object @default)
            {
                Property = property;
                Attribute = attribute;
                Default = @default;
            }
        }
    }

}
