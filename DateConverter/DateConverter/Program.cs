using CommandLineParser;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DateConverter {
    class Options : ICommandDefinition {
        [Option(names: new string[] { "--date", "-d" },
            HelpText = "Date to be displayed",
            MaxParameterCount = 1,
            MinParameterCount = 1,
            IsRequired = true
            )]
        public DateTime date { get; set; }
        [Option(names: new string[] { "--format", "-f" },
            HelpText = "Format for the date to be displayed as",
            MaxParameterCount = 1,
            MinParameterCount = 1,
            IsRequired = false
            )]
        public string? format { get; set; }
    }
    internal class Program {
        static void Main(string[] args) {
            args = new string[] { "--date", "2018-12-31T14:45", "--format", "MM/dd/yyyy-HH:mm" };
            Options options = new Options();
            options = CommandParser<Options>.Parse(args, options);

            Console.WriteLine($"Your chosen date is {options.date}");
            Console.WriteLine($"Your chosen format is {options.format}");
            Console.WriteLine($"Your formatted date is {options.date.ToString(options.format)}");
        }
    }
}