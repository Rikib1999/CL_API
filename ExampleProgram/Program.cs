using CommandLineParser;

namespace ExampleProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            Time timeCommand = new Time();

            double doubleNumber = 5.5;
            string c = $"-b false -f format arg1 --portability -o=aaa --append --verbose -n 9 -c=-5 02.5 0.4 -33 -e 1 None arg2 -d 2022-02-06 -dg=5,4,-1,005 -sc \"aloha\\\\ \" \" \\\"praha\" praha -dt {doubleNumber}";
            //c = "required1 required2";
            string commandLineInput = c;// Console.ReadLine();\

            timeCommand = CommandParser<Time>.Parse(commandLineInput, timeCommand);

            if (timeCommand.IsPresent("--verbose"))
            {
                var b = 2;
            }

            var randomCommandHelpText = timeCommand.GetHelpText("-r");

            var wholeHelpText = timeCommand.GetHelpText();

            Console.WriteLine(timeCommand.IsPresent("-b"));
            Console.WriteLine(timeCommand.IsPresent("-f"));
            Console.WriteLine(timeCommand.IsPresent("--portability"));
            Console.WriteLine(timeCommand.IsPresent("-o"));
            Console.WriteLine(timeCommand.IsPresent("--append"));
            Console.WriteLine(timeCommand.IsPresent("--verbose"));
            Console.WriteLine(timeCommand.IsPresent("-n"));
            Console.WriteLine(timeCommand.IsPresent("-c"));
            Console.WriteLine(timeCommand.IsPresent("-e"));
            Console.WriteLine(timeCommand.IsPresent("-d"));
            Console.WriteLine(timeCommand.IsPresent("-dg"));
            Console.WriteLine(timeCommand.IsPresent("-sc"));
            Console.WriteLine(timeCommand.IsPresent("-dt"));

            var x = 5;
        }
    }
}