using CommandLineParser;

namespace ParserTests;

public class RequiredTests
{
    public class RequiredCommandDefinition : ICommandDefinition
    {
        [Option("-r", "--required", HelpText = "required help text", IsRequired = true, MaxParameterCount = 1)]
        public string Required { get; set; } = null!;
    }
    
    private RequiredCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new RequiredCommandDefinition();
    }
    
    [Test]
    public void RequiredOptionThrowsExceptionWhenNotGiven()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<RequiredCommandDefinition>.Parse("5", _commandDefinition);       
        });
    }
    
    [Test]
    public void RequiredOptionDoesNotThrowExceptionWhenGiven()
    {
        CommandParser<RequiredCommandDefinition>.Parse("-r required", _commandDefinition);
    }
}