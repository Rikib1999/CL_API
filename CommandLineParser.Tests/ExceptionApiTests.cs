using System;
using Xunit;
using CommandLineParser;

namespace CommandLineParser.Tests
{
    public class ExceptionApiTests
    {
        [Fact]
        public void SameOptionsNames()
        {
            Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => new OptionsWithSameNames()));
        }

        [Theory]
        [InlineData("-oneToTen 11")]
        [InlineData("-oneToTen 0")]
        [InlineData("-oneToTen 12")]
        [InlineData("-oneToTen -1")]
        [InlineData("-oneToMax -2147483648")]
        [InlineData("-oneToMax 0")]
        [InlineData("-minToOne 2147483647")]
        [InlineData("-minToOne 0")]
        public void OutOfBoundsOfIntegerBounderiesOfOption(string arg)
        {
            var parser = new IntegerOptionWithBounderiesParser();
            Assert.ThrowsAsync<Exception>(
                () =>
                    Task.Run(
                        () => CommandParser<IntegerOptionWithBounderiesParser>.Parse(arg, parser)
                    )
            );
        }

        [Theory]
        [InlineData("-a 2147483648")]
        [InlineData("-a -2147483649")]
        public void TooLargeIntegerOptionParameter(string arg)
        {
            var parser = new FourInitigerOptionsParser();

            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => CommandParser<FourInitigerOptionsParser>.Parse(arg, parser))
            );
        }

        [Theory]
        [InlineData("-1", "-1 depends on -2")]
        [InlineData("-2", "-2 depends on -1")]
        public void WrongDependecyArgument(string arg, string comment)
        {
            var parser = new DependencyOptionsParser();
            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => CommandParser<DependencyOptionsParser>.Parse(arg, parser))
            );
        }

        [Theory]
        [InlineData("-1 -2", "-1 exclisive with -2")]
        public void WrongExclusiveArgument(string arg, string comment)
        {
            var parser = new ExclusiveOptionsParser();
            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => CommandParser<ExclusiveOptionsParser>.Parse(arg, parser))
            );
        }

        [Fact]
        public void DependOnUnknownOptionParser()
        {
            Assert.ThrowsAsync<Exception>(() => Task.Run(() => new DependOnUnknownOptionParser()));
        }

        [Fact]
        public void DepedencyWithExclusivityBetweenTwoOptions()
        {
            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => new ExclusiveAndDependencyTwoOptionsParser())
            );

            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => new ExclusiveAndDependencySameOptionParser())
            );
        }

        [Theory]
        [InlineData("onlyOneArg")]
        public void MissingRequiredPlainArgument(string arg)
        {
            var parser = new SomeRequiredSomeNotPlainParametersParser();

            Assert.Throws<Exception>(
                () =>
                    Task.Run(
                            () =>
                                CommandParser<SomeRequiredSomeNotPlainParametersParser>.Parse(
                                    arg,
                                    parser
                                )
                        )
                        .Wait()
            );
        } 

        [Fact]
        public void WrongPlainArgumentOrder()
        {
            Assert.ThrowsAsync<Exception>(
                () => Task.Run(() => new WrongPlainArgumentOrderParser())
            );
        }

        [Theory]
        [InlineData("-a -b -c --aaa")]
        [InlineData("-a -b -c --ccc")]
        [InlineData("-a --bbb -c -b")]
        public void MoreSameOptionsInOnCMDL(string arg)
        {
            var parser = new FourInitigerOptionsParser();

            Assert.Throws<Exception>(
                () =>
                    Task.Run(() => CommandParser<FourInitigerOptionsParser>.Parse(arg, parser))
                        .Wait()
            );
        }
    }
}
