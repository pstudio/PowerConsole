using System;

namespace pstudio.PowerConsole.Command
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public sbyte Position { get; set; }
        public bool Mandatory { get; set; }

        public ParameterAttribute()
        {
            Position = -1;
            Mandatory = false;
        }
    }

}
