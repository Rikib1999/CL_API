using CommandLineParser.ParsedModels;

namespace CommandLineParser.Models
{
    public class Command
    {
        public readonly string name;

        public List<Option> Options { get; private set; }
        public List<Option> MandatoryOptions { get; private set; }
        public Argument Argument { get; private set; }

        public Command(string name)
        {
            this.name = name;
        }
        
        public bool IsOptionCorrect(string argument)
        {
            return false;
        }

        public Option ReturnOption(string name)
        {
            return null;
        }

        public void PrintHelpText()
        { 
            
        }

        public bool IsCommandCorrect(ParsedCommand parsedCommand)
        {
            return false;
        }
    }
}