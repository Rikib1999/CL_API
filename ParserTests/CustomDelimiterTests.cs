using CommandLineParser;
using NUnit.Framework;

namespace ParserTests;

[TestFixture]
public class CustomDelimiterTests
{
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-l", "--list", HelpText = "number help text", MinParameterCount = 1, Delimeter=';')]
        public List<string> ListWithCustomDelimiter { get; set; } = new();
    }
    
    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }
    
    [Test]
    public void ListWithCustomDelimiter()
    {
        CommandParser<CorrectCommandDefinition>.Parse("-l 1;2;3", _commandDefinition);
        Assert.Multiple(() =>
        {
            Assert.That(_commandDefinition.ListWithCustomDelimiter, Has.Count.EqualTo(3));
            Assert.That(_commandDefinition.ListWithCustomDelimiter[0], Is.EqualTo("1"));
            Assert.That(_commandDefinition.ListWithCustomDelimiter[1], Is.EqualTo("2"));
            Assert.That(_commandDefinition.ListWithCustomDelimiter[2], Is.EqualTo("3"));
        });
    }
    
    [Test]
    public void ListWithCustomDelimiterAndNoParameters()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-l", _commandDefinition);       
        });
    }
    
    [Test]
    public void ListWithBadCustomDelimiter()
    {
        // Should not throw an exception, but rather parse the string as a single parameter
        CommandParser<CorrectCommandDefinition>.Parse("-l 1,2,3,4", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(_commandDefinition.ListWithCustomDelimiter, Has.Count.EqualTo(1));
            Assert.That(_commandDefinition.ListWithCustomDelimiter[0], Is.EqualTo("1,2,3,4"));
        });
    }
}