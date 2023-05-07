using CommandLineParser;
using NUnit.Framework;

namespace ParserTests;

[TestFixture]
public class ParameterCountTests
{
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-n", "--number", HelpText = "number help text", MinParameterCount = 1, MaxParameterCount = 1)]
        public int? NumberWithBoundaries { get; set; }

        [Option("-p", "--parameter-count-test", HelpText = "parameter count test help text", MinParameterCount = 1,
            MaxParameterCount = 3)]
        public string ParameterCountTest { get; set; } = null!;
    }
    
    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }
    
    [Test]
    public void ParameterCountTestThrowsExceptionWhenTooFewParametersAreGiven()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-p", _commandDefinition);       
        });
    }
    
    [Test]
    public void ParameterCountTestThrowsExceptionWhenTooManyParametersAreGiven()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-p 1 2 3 4", _commandDefinition);       
        });
    }
    
    [Test]
    public void ThrowsExceptionWhenParametersAreGiven()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-p 1", _commandDefinition);       
        });
    }
    
    [Test]
    public void NumberWithNoParametersThrowsException()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-n", _commandDefinition);       
        });
    }
    
    [Test]
    public void NumberWithTooManyParametersThrowsException()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-n 1 2 3 4", _commandDefinition);       
        });
    }
}