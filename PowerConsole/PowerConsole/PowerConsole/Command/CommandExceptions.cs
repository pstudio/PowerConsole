using System;
using System.Runtime.Serialization;

namespace pstudio.PowerConsole.Command
{
    /// <summary>
    /// Thrown when the user tries to execute a command that does not exist in the current <see cref="CommandContext"/>
    /// </summary>
    public class MissingCommandException : Exception
    {
        public MissingCommandException()
        {
        }

        public MissingCommandException(string message) : base(message)
        {
        }

        public MissingCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when trying to register a command with a name already registered.
    /// </summary>
    public class CommandNameRegisteredException : Exception
    {
        public CommandNameRegisteredException()
        {
        }

        public CommandNameRegisteredException(string message) : base(message)
        {
        }

        public CommandNameRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandNameRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown if the command could not be created.
    /// </summary>
    public class CreateCommandFailedException : Exception
    {
        public CreateCommandFailedException()
        {
        }

        public CreateCommandFailedException(string message) : base(message)
        {
        }

        public CreateCommandFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CreateCommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when an invalid <see cref="ParameterAttribute"/> Position value is provided.
    /// Position values must start at 0 and increase by exactly one for additional properties.
    /// </summary>
    public class InvalidParameterPositionException : Exception
    {
        public InvalidParameterPositionException()
        {
        }

        public InvalidParameterPositionException(string message) : base(message)
        {
        }

        public InvalidParameterPositionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidParameterPositionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user specifies an invalid named parameter.
    /// </summary>
    public class InvalidNamedParameterException : Exception
    {
        public InvalidNamedParameterException()
        {
        }

        public InvalidNamedParameterException(string message) : base(message)
        {
        }

        public InvalidNamedParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidNamedParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user has not provided enough positional arguments.
    /// </summary>
    public class MissingPositionalArgumentException : Exception
    {
        public MissingPositionalArgumentException()
        {
        }

        public MissingPositionalArgumentException(string message) : base(message)
        {
        }

        public MissingPositionalArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingPositionalArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user has not provided an argument value for a named parameter.
    /// </summary>
    public class MissingNamedArgumentException : Exception
    {
        public MissingNamedArgumentException()
        {
        }

        public MissingNamedArgumentException(string message) : base(message)
        {
        }

        public MissingNamedArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingNamedArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user has not provided all mandatory named parameters.
    /// </summary>
    public class MissingMandatoryParameterException : Exception
    {
        public MissingMandatoryParameterException()
        {
        }

        public MissingMandatoryParameterException(string message) : base(message)
        {
        }

        public MissingMandatoryParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingMandatoryParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user has provided an invalid argument of a type different from the expected type.
    /// </summary>
    public class InvalidArgumentTypeException : Exception
    {
        public InvalidArgumentTypeException()
        {
        }

        public InvalidArgumentTypeException(string message) : base(message)
        {
        }

        public InvalidArgumentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidArgumentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    /// Thrown when the user pipes a value to a command that doesn't accept that type through piping.
    /// </summary>
    public class InvalidPipeArgumentTypeException : Exception
    {
        public InvalidPipeArgumentTypeException()
        {
        }

        public InvalidPipeArgumentTypeException(string message) : base(message)
        {
        }

        public InvalidPipeArgumentTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidPipeArgumentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    ///  Thrown if the user provides more positional arguments than expected.
    /// </summary>
    public class UnexpectedPositionalArgument : Exception
    {
        public UnexpectedPositionalArgument()
        {
        }

        public UnexpectedPositionalArgument(string message) : base(message)
        {
        }

        public UnexpectedPositionalArgument(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedPositionalArgument(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    /// <summary>
    ///  Thrown if the use tries to assign a value to a named parameter that has already been assigned through piping.
    /// </summary>
    public class NamedParameterAssignedThroughPiping : Exception
    {
        public NamedParameterAssignedThroughPiping()
        {
        }

        public NamedParameterAssignedThroughPiping(string message) : base(message)
        {
        }

        public NamedParameterAssignedThroughPiping(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NamedParameterAssignedThroughPiping(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
