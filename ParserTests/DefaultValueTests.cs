using CommandLineParser;

namespace ParserTests;

public class DefaultValueTests
{
    private const int DefaultNumber = 5;
    private const string DefaultString = "test";
    private const bool DefaultBool = true;
    
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-n", "--number", HelpText = "number help text", MinParameterCount = 1, MaxParameterCount = 1,
            DefaultValue = DefaultNumber)]
        public int NumberWithDefaultValue { get; set; }

        [Option("-s", "--string-test-default-value", HelpText = "string test", MinParameterCount = 1, MaxParameterCount = 1,
            DefaultValue = DefaultString)]
        public string StringWithDefaultValue { get; set; } = null!;
        
        [Option("-b", "--bool-test-default-value", HelpText = "bool test", MinParameterCount = 1, MaxParameterCount = 1,
            DefaultValue = DefaultBool)]
        public bool BoolWithDefaultValue { get; set; }
    }
    
    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }
    
    [Test]
    public void NumberWithDefaultValueIsSet()
    {
        CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        Assert.That(_commandDefinition.NumberWithDefaultValue, Is.EqualTo(DefaultNumber));
    }
    
    [Test]
    public void StringWithDefaultValueIsSet()
    {
        CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        Assert.That(_commandDefinition.StringWithDefaultValue, Is.EqualTo(DefaultString));
    }
    
    [Test]
    public void BoolWithDefaultValueIsSet()
    {
        CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        Assert.That(_commandDefinition.BoolWithDefaultValue, Is.EqualTo(DefaultBool));
    }
}