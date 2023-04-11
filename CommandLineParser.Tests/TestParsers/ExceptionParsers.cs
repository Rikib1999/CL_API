using CommandLineParser;

namespace CommandLineParser.Tests
{
    public class OptionsWithSameNames : ICommandDefinition
    {
        [Option(names: new string[] { "-a", "--aaa" }, MaxParameterCount = 0)]
        public string FirstOption { get; set; }

        [Option(names: new string[] { "-b", "--aaa" }, MaxParameterCount = 0)]
        public string SecondOption { get; set; }
    }

    public class DependOnUnknownOptionParser : ICommandDefinition
    {
        [Option(names: new string[] { "-1" }, Dependencies = new string[] { "-2" })]
        public object Random { get; set; }
    }

    public class WrongPlainArgumentOrderParser : ICommandDefinition
    {
        [Argument(order: 2, IsRequired = true)]
        public string Required1 { get; set; }

        [Argument(order: 1, IsRequired = false)]
        public string NotRequired1 { get; set; }

        [Argument(order: 1, IsRequired = true)]
        public string Required2 { get; set; }
        public string PlainParameter2 { get; set; }

        [Argument(order: 0, IsRequired = false)]
        public string NotRequired2 { get; set; }
    }
}
