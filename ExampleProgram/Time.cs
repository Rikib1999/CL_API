using CommandLineParser;

namespace ExampleProgram
{
    public enum EnumTest
    {
        None,
        Test
    }

    public class TestClass
    {
        public string s;

        public TestClass(string s)
        {
            this.s = s;
        }
    }

    public class Time : ICommandDefinition
    {
        [Option(names: new string[] { "-b"}
            , MinParameterCount = 1
            , MaxParameterCount = 1, IsRequired = true
        )]
        public bool BoolOption { get; set; }

        [Option(names: new string[] { "-f", "--format" }
            , HelpText = "Specify output format, possibly overriding the format specified in the environment variable TIME."
            , MinParameterCount = 1
            , MaxParameterCount = 1
        )]
        public string Format { get; set; }

        [Option(names: new string[] { "-p", "--portability" }
            , HelpText = "Use the portable output format."
            , MaxParameterCount = 0
        )]
        public object Portability { get; set; }

        [Option(names: new string[] { "-o", "--output" }
            , HelpText = "Do not send the results to stderr, but overwrite the specified file."
            , MinParameterCount = 1
            , MaxParameterCount = 1
        )]
        public string Output { get; set; }

        [Option(names: new string[] { "-d", "--date" }
            , HelpText = "Date helptext."
            , MinParameterCount = 1
            , MaxParameterCount = 1
        )]
        public DateTime Date { get; set; }

        [Option(names: new string[] { "-e", "--enumtest" }
            , HelpText = "EnumTest."
            , MinParameterCount = 1
            , MaxParameterCount = 2
        )]
        public List<EnumTest> EnumTest { get; set; }

        [Option(names: new string[] { "-a", "--append" }, HelpText = "(Used together with -o.) Do not overwrite but append."
            , MaxParameterCount = 0
            , Dependencies = new string[] { "-o" }
        )]
        public object Append { get; set; }

        [Option(names: new string[] { "-v", "--verbose" }
            , HelpText = "Give very verbose output about all the program knows about."
            , MaxParameterCount = 0
        )]
        public string Verbose { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-n", "--number" }, MinParameterCount = 1, MaxParameterCount = 1)]
        public int Number { get; set; }

        [Option(names: new string[] { "-r", "--random" }
            , MinParameterCount = 0, MaxParameterCount = 0, Dependencies = new string[] { "--verbose" }
            , HelpText = "This is a random option"
            , Exclusivities = new string[] { "-n" }
        )]
        public object Random { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-c", "--coins" }, MinParameterCount = 1, MaxParameterCount = 5)]
        public List<float> Coins { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-dg", "--dogs" }, MinParameterCount = 1, MaxParameterCount = 5, Delimeter = ',')]
        public int[] Dogs { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-sc" }, MinParameterCount = 1, MaxParameterCount = 5, Delimeter = ',')]
        public TestClass[] StringClass { get; set; }

        [Option(names: new string[] { "-dt" }, MaxParameterCount = 1)]
        public double DoubleTest { get; set; }
        
        [Argument("command", order: 0, IsRequired = true)]
        public string Command { get; set; }
        
        [Argument("command2", order: 1, IsRequired = true)]
        public string Command2 { get; set; }

        [Argument("arguments", order: 2, IsRequired = false)]
        public List<string> Arguments { get; set; }
    }
}