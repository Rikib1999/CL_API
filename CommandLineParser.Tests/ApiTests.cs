using System;
using Xunit;
using CommandLineParser;


namespace CommandLineParser.Tests
{
    public class ApiTests
    {
        [Theory]
        [InlineData("-aaa")]
        [InlineData("--a")]
        [InlineData("_-a")]
        public void OptionsNameNotPresentedInCMD(string arg)
        {
            var parser = new OptionNoParameterMultipleNamesParser();
            parser.Option = "Hello";
            parser = CommandParser<OptionNoParameterMultipleNamesParser>.Parse(arg, parser);
            bool isOptionPresent = parser.IsPresent(arg);
            Assert.True(!isOptionPresent, $"Unknown option {arg} found.");
            Assert.Equal("Hello", parser.Option);
        }
        
        [Theory]
        [InlineData("--aaa")]
        [InlineData("-a")]
        public void OptionsNameIsPresentedInCMD(string arg)
        {
            var parser = new OptionNoParameterMultipleNamesParser(); 
            parser.Option = "Example";
            parser = CommandParser<OptionNoParameterMultipleNamesParser>.Parse(arg, parser);
            bool isOptionPresent = parser.IsPresent(arg);
            Assert.True(isOptionPresent, $"Option {arg} not found.");
            Assert.Equal("Example", parser.Option);
        }
        
        [Theory]
        [InlineData("-a")]
        [InlineData("--aaa")]
        [InlineData("-_")]
        [InlineData("-#$%^&*(){}[];'.,/")]
        [InlineData("---a")]
        public void OptionNames(string arg)
        {
            var parser = new OptionNoParameterMultipleNamesParser();   
            CommandParser<OptionNoParameterMultipleNamesParser>.Parse(arg, parser);
            bool isOptionPresent = parser.IsPresent(arg);
            Assert.True(isOptionPresent, $"Option {arg} not found.");
        }


        [Theory]
        [InlineData("", "empty parameter")]
        [InlineData("HelloWorld", "basic call")]
        [InlineData("Hello World", "call with empty space")]
        [InlineData("78&*($#`$@{}[]:_-=|;'\\)", "call with wierd chars")]
        public void OptionsParsedOutput(string arg, string comment)
        {
            var parser = new OptionWithOneStringParameterParser();
            parser = CommandParser<OptionWithOneStringParameterParser>.Parse("-a \"" + arg + "\"", parser);
            Assert.Equal(arg, parser.FirstOption);

        }

        [Fact]
        public void TwoRequiredTwoNotPlainParameters(){
            var parser = new SomeRequiredSomeNotPlainParametersParser();
            parser = CommandParser<SomeRequiredSomeNotPlainParametersParser>.Parse("required1 required2", parser);
            Assert.Equal("required1", parser.Required1);
            Assert.Equal("required2", parser.Required2);
        }
        
        
        [Theory]
        [InlineData("-oneToTen 1")]
        [InlineData("-oneToTen 2")]
        [InlineData("-oneToTen 5")]
        [InlineData("-oneToTen 8")]
        [InlineData("-oneToTen 9")]
        [InlineData("-oneToTen 10")]
        [InlineData("-oneToMax 2147483647")]
        [InlineData("-oneToMax 1")]
        [InlineData("-minToOne -2147483648")]
        [InlineData("-minToOne 1")]
        public void BounderiesOfIntegerValueOfOption(string arg){
            var parser = new IntegerOptionWithBounderiesParser();
            parser = CommandParser<IntegerOptionWithBounderiesParser>.Parse("-a " + arg, parser);
            // all should pass, exception shouldn't be raised
        }

        [Theory]
        [InlineData("this is a required list of string")]
        public void StringListOptionParameter(string arg) {
            var parser = new ListStringPlainParameterParser();
            parser = CommandParser<ListStringPlainParameterParser>.Parse("-a " + arg, parser);
            string[] correctOutput = arg.Split(" ");
            
            Assert.True(parser.Arguments.Count == correctOutput.Length);
            for (int i = 0; i < parser.Arguments.Count; i++)
            {
                Assert.Equal(correctOutput[i], parser.Arguments[i]); 
            }
        }

        [Theory]
        [InlineData("-a -b -c -d")]
        [InlineData("-a -b --ccc -ddd")]
        [InlineData("--aaa --bbb -c -ddd")]
        public void IsPresentOptionAfterParse(string arg)
        {
            var parser = new FourInitigerOptionsParser();
            parser = CommandParser<FourInitigerOptionsParser>.Parse(arg, parser);
            Assert.True(parser.IsPresent("-a"));
            Assert.True(parser.IsPresent("-b"));
            Assert.True(parser.IsPresent("-c"));
            Assert.True(parser.IsPresent("--ccc"));
            Assert.True(parser.IsPresent("--bbb"));
            Assert.True(parser.IsPresent("--ccc"));
            Assert.True(parser.IsPresent("--ddd"));
            Assert.True(parser.IsPresent("-d"));
            Assert.False(parser.IsPresent("--eee"));
            Assert.False(parser.IsPresent("--e"));
        }
    }
}

