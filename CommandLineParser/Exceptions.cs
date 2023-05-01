namespace CommandLineParser
{
    /// <summary>
    /// User defined command class does not implement ICommandDefinition interface.
    /// </summary>
    public class MissingInterfaceException : Exception
    {
        public MissingInterfaceException() { }

        public MissingInterfaceException(string message) : base(message) { }

        public MissingInterfaceException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Command could not be parsed.
    /// </summary>
    public class CommandParserException : Exception
    {
        public CommandParserException() { }

        public CommandParserException(string message) : base(message) { }

        public CommandParserException(string message, Exception inner) : base(message, inner) { }
    }

    public class MissingRequiredOptionException : Exception
    { 
        public MissingRequiredOptionException(string message) : base(message) { }
    }

    public class IncorrectExtremesException : Exception
    { 
        public IncorrectExtremesException(string message) : base(message) { }
    }

    public class InvalidPropertyTypeException : Exception 
    {
        public InvalidPropertyTypeException(string message) : base(message) { }
    }

    public class InvalidValueException : Exception 
    {
        public InvalidValueException(string message) : base(message) { }
    }
}