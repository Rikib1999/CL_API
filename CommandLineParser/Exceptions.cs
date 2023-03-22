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
}