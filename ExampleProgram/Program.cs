using CommandLineParser;

namespace ExampleProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            Time timeCommand = new Time();

            string commandLineInput = Console.ReadLine();
            
            timeCommand = CommandParser<Time>.Parse(commandLineInput, timeCommand);

            if (timeCommand.IsPresent("--verbose"))
            {
                //...
            }

            var randomCommandHelpText = timeCommand.GetHelpText("-r");

            var wholeHelpText = timeCommand.GetHelpText();
        }
    }
}