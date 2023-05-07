using CommandLineParser;

namespace ParserTests;

[TestFixture]
public class DependencyExclusivityTests
{
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-a", MaxParameterCount = 1)]
        public string Independent { get; set; }

        [Option("-d", MaxParameterCount = 1, Dependencies = new[] {"-a"})]
        public string Dependent { get; set; } = null!;
        
        [Option("-e", MaxParameterCount = 1, Exclusivities = new[] {"-a"})]
        public string Exclusivity { get; set; } = null!;
        
        [Option("-f", MaxParameterCount = 1, Dependencies = new[] {"-a"}, Exclusivities = new[] {"-e"})]
        public string DependencyAndExclusivity { get; set; } = null!;
    }
    
    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }
    
    /*
     * I added some comments to make up for the long test names.
     */
    
    [Test]
    public void DependentOptionThrowsExceptionWhenDependencyIsNotGiven()
    {
        Assert.Throws<CommandParserException>(() =>
        {
            // D is given, but dependency A is not => exception.
            CommandParser<CorrectCommandDefinition>.Parse("-d dependent", _commandDefinition);       
        });
    }
    
    [Test]
    public void DependentOptionDoesNotThrowExceptionWhenDependencyIsGiven()
    {
        // A is given before D, so it should be okay
        CommandParser<CorrectCommandDefinition>.Parse("-a independent -d dependent", _commandDefinition);
    }
    
    [Test]
    public void ExclusivityOptionThrowsExceptionWhenExclusivityIsGiven()
    {
        // E is given, so A should not be given.
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-a independent -e exclusivity", _commandDefinition);       
        });
    }
    
    [Test]
    public void ExclusivityOptionDoesNotThrowExceptionWhenExclusivityIsNotGiven()
    {
        CommandParser<CorrectCommandDefinition>.Parse("-e exclusivity", _commandDefinition);
    }
    
    [Test]
    public void DependencyAndExclusivityOptionThrowsExceptionWhenDependencyIsNotGiven()
    {
        // F is given, so A should be given, but it's not => exception.
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-f dependencyAndExclusivity", _commandDefinition);       
        });
    }
    
    [Test]
    public void DependencyAndExclusivityOptionThrowsExceptionWhenExclusivityIsGiven()
    {
        // E is given, so F should throw an exception.
        Assert.Throws<CommandParserException>(() =>
        {
            CommandParser<CorrectCommandDefinition>.Parse("-a independent -e exclusivity -f dependencyAndExclusivity", _commandDefinition);       
        });
    }
    
    [Test]
    public void DependencyAndExclusivityOptionDoesNotThrowExceptionWhenDependencyIsGiven()
    {
        // Only dependency is given
        CommandParser<CorrectCommandDefinition>.Parse("-a independent -f dependencyAndExclusivity", _commandDefinition);
    }
}