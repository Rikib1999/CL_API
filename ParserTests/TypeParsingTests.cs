using CommandLineParser;

namespace ParserTests;

public class TypeParsingTests
{
    /*
     * In these tests, I assume that the default value of an option is just a default value of that type
     * So if type is nullable, then the default value is null
     * I also assume that boolean options are false by default and act as flags
     */
    
    private class CorrectCommandDefinition : ICommandDefinition
    {
        [Option("-t", "--test", HelpText = "test help text")]
        public string? TestString { get; set; }
        
        [Option("-n", "--number", HelpText = "number help text")]
        public int? TestNumber { get; set; }
        
        [Option("-b", "--bool", HelpText = "bool help text",
            MaxParameterCount = 0)]
        public bool TestBool { get; set; }
        
        [Option("-s", "--string-list", HelpText = "string list help text")]
        public List<string> TestStringList { get; set; } = new();
        
        [Option("-d", "--double")]
        public double? TestDouble { get; set; }
    }

    private CorrectCommandDefinition _commandDefinition = null!;
    
    [SetUp]
    public void Setup()
    {
        _commandDefinition = new CorrectCommandDefinition();
    }

    [Test]
    public void CommandWithPresentStringOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("-t test", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestString, Is.EqualTo("test"));
            Assert.That(command.IsPresent("-t"), Is.True);
        });
    }
    
    [Test]
    public void CommandWithMissingStringOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestString, Is.Null);
            Assert.That(command.IsPresent("-t"), Is.False);
        });
    }
    
    [Test]
    [TestCase(5)]
    [TestCase(0)]
    [TestCase(-5)]
    [TestCase(int.MaxValue)]
    [TestCase(int.MinValue)]
    public void CommandWithPresentNumberOptionIsParsedCorrectly(int number)
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse($"-n {number}", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestNumber, Is.EqualTo(number));
            Assert.That(command.IsPresent("-n"), Is.True);
        });
    }
    
    [Test]
    [TestCase(5.5)]
    [TestCase(0.0)]
    [TestCase(-5.5)]
    [TestCase(double.MaxValue)]
    [TestCase(double.MinValue)]
    public void CommandWithPresentDoubleOptionIsParsedCorrectly(double doubleNumber)
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("-d {doubleNumber}", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestDouble, Is.EqualTo(doubleNumber));
            Assert.That(command.IsPresent("-d"), Is.True);
        });
    }
    
    [Test]
    public void CommandWithMissingNumberOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestNumber, Is.Null);
            Assert.That(command.IsPresent("-n"), Is.False);
        });
    }
    
    [Test]
    public void CommandWithPresentBoolOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("-b", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestBool, Is.True);
            Assert.That(command.IsPresent("-b"), Is.True);
        });
    }
    
    [Test]
    public void CommandWithMissingBoolOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestBool, Is.False);
            Assert.That(command.IsPresent("-b"), Is.False);
        });
    }
    
    [Test]
    public void CommandWithPresentStringListOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("-s test1 test2", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestStringList, Is.EquivalentTo(new List<string> {"test1", "test2"}));
            Assert.That(command.IsPresent("-s"), Is.True);
        });
    }
    
    [Test]
    public void CommandWithMissingStringListOptionIsParsedCorrectly()
    {
        var command = CommandParser<CorrectCommandDefinition>.Parse("", _commandDefinition);
        
        Assert.Multiple(() =>
        {
            Assert.That(command.TestStringList, Is.Empty);
            Assert.That(command.IsPresent("-s"), Is.False);
        });
    }
}