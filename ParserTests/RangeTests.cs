using CommandLineParser;

namespace ParserTests;

public class RangeTests
{
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-n", "--number", HelpText = "number help text", MinParameterCount = 1, MaxParameterCount = 1)]
        [Boundaries<int>(lowerBound: 0, upperBound: 10)]
        public int? NumberWithBoundaries { get; set; }
        
        [Option("-l", "--list-test-boundaries", HelpText = "list test", MinParameterCount = 1, MaxParameterCount = 3)]
        [Boundaries<int>(lowerBound: 0, upperBound: 10)]
        public List<int> NumberWithBoundariesList { get; set; } = new List<int>();
    }
    
    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }
    
    [Test]
    public void NumberWithBoundariesThrowsExceptionWhenValueIsOutOfRange()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-n 50", _commandDefinition);       
        });
    }
    
    [Test]
    public void NumberWithBoundariesThrowsExceptionWhenValueIsOutOfRangeAndNegative()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-n -1", _commandDefinition);       
        });
    }
    
    [Test]
    public void NumberListWithBoundariesThrowsExceptionWhenOneValueIsOutOfRange()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-l 1 2 50", _commandDefinition);       
        });
    }
    
    [Test]
    public void NumberListWithBoundariesThrowsExceptionWhenOneValueIsOutOfRangeAndNegative()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-l 1 2 -1", _commandDefinition);       
        });
    }
}