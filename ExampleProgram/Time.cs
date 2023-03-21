using CommandLineParser;

namespace ExampleProgram
{
    public class Time : ICommandDefinition
    {
        [Option(names: new string[] { "-f", "--format" }
            ,HelpText = "Specify output format, possibly overriding the format specified in the environment variable TIME."
            ,MinParameterCount = 1
            ,MaxParameterCount = 1
        )]
        public string Format { get; set; }

        [Option(names: new string[] { "-p", "--portability" }
            ,HelpText = "Use the portable output format."
            ,MaxParameterCount = 0
        )]
        public object Portability { get; set; }

        [Option(names: new string[] { "-o", "--output" }
            ,HelpText = "Do not send the results to stderr, but overwrite the specified file."
            ,MinParameterCount = 1
            ,MaxParameterCount = 1
        )]
        public string Output { get; set; }

        [Option(names: new string[] { "-a", "--append" }, HelpText = "(Used together with -o.) Do not overwrite but append."
            ,MaxParameterCount = 0
        )]
        public object Append { get; set; }

        [Option(names: new string[] { "-v", "--verbose" }
            ,HelpText = "Give very verbose output about all the program knows about."
            ,MaxParameterCount = 0
        )]
        public string Verbose { get; set; }

        [Argument(order: 0, IsRequired = true)]
        public string Command { get; set; }

        [Argument(order: 1, IsRequired = true)]
        public List<string> Arguments { get; set; }
    }
}