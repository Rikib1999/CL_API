using CommandLineParser;

namespace CommandLineParser.Tests
{
    public class OptionNoParameterParser : ICommandDefinition
    {
        [Option(names: new string[] { "-a", "--aaa" }, MaxParameterCount = 0)]
        public string FirstOption { get; set; }
    }

    public class OptionNoParameterMultipleNamesParser : ICommandDefinition
    {
        [Option(
            names: new string[] { "-a", "--aaa", "-_", "-#$%^&*(){}[];'.,/", "---a" },
            MaxParameterCount = 0
        )]
        public string Option { get; set; }
    }

    public class OptionWithOneStringParameterParser : ICommandDefinition
    {
        [Option(names: new string[] { "-a" }, MaxParameterCount = 1, MinParameterCount = 1)]
        public string FirstOption { get; set; }
    }

    public class SomeRequiredSomeNotPlainParametersParser : ICommandDefinition
    {
        [Argument(order: 3, IsRequired = true)]
        public string Required1 { get; set; }

        [Argument(order: 1, IsRequired = false)]
        public string NotRequired1 { get; set; }

        [Argument(order: 2, IsRequired = true)]
        public string Required2 { get; set; }
        public string PlainParameter2 { get; set; }

        [Argument(order: 0, IsRequired = false)]
        public string NotRequired2 { get; set; }
    }

    public class IntegerOptionWithBounderiesParser : ICommandDefinition
    {
        [Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-oneToTen" })]
        public int oneToTen { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: 1)]
        [Option(names: new string[] { "-oneToOne" })]
        public int oneToOne { get; set; }

        [Boundaries<int>(lowerBound: 1, upperBound: int.MaxValue)]
        [Option(names: new string[] { "-oneToMax" })]
        public int oneToMax { get; set; }

        [Boundaries<int>(lowerBound: int.MinValue, upperBound: 1)]
        [Option(names: new string[] { "-minToOne" })]
        public int minToOne { get; set; }
    }

    public class ListStringPlainParameterParser : ICommandDefinition
    {
        [Argument(order: 0, IsRequired = true)]
        public List<string> Arguments { get; set; }
    }

    public class DependencyOptionsParser : ICommandDefinition
    {
        [Option(names: new string[] { "-1" }, Dependencies = new string[] { "-2" })]
        public object first { get; set; }

        [Option(names: new string[] { "-2" }, Dependencies = new string[] { "-1" })]
        public object second { get; set; }
    }

    public class ExclusiveOptionsParser : ICommandDefinition
    {
        [Option(names: new string[] { "-1" }, Exclusivities = new string[] { "-2" })]
        public object first { get; set; }

        [Option(names: new string[] { "-2" }, Exclusivities = new string[] { "-1" })]
        public object second { get; set; }
    }

    public class ExclusiveAndDependencyTwoOptionsParser : ICommandDefinition
    {
        [Option(names: new string[] { "-1" }, Exclusivities = new string[] { "-2" })]
        public object first { get; set; }

        [Option(names: new string[] { "-2" }, Dependencies = new string[] { "-1" })]
        public object second { get; set; }
    }

    public class ExclusiveAndDependencySameOptionParser : ICommandDefinition
    {
        [Option(
            names: new string[] { "-1" },
            Exclusivities = new string[] { "-1" },
            Dependencies = new string[] { "-1" }
        )]
        public object first { get; set; }
    }


    public class FourInitigerOptionsParser : ICommandDefinition
    {
        [Option(names: new string[] { "-a", "--aaa" })]
        public int a { get; set; }

        [Option(names: new string[] { "-b", "--bbb" })]
        public int b { get; set; }

        [Option(names: new string[] { "-c", "--ccc" })]
        public int c { get; set; }

        [Option(names: new string[] { "-d", "--ddd" })]
        public int d { get; set; }
    }
}
