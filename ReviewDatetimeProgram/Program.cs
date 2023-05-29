using CommandLineParser;

namespace ReviewDatetimeProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTimeExtension dateTimeExtension = new DateTimeExtension();

            string arg = string.Join(" ", args);

            dateTimeExtension = CommandParser<DateTimeExtension>.Parse(arg,dateTimeExtension);

            if (dateTimeExtension.IsPresent("--format"))
            {
                var format = dateTimeExtension.Format.Replace('M', (char)3);
                format = format.Replace('m', 'M');
                format = format.Replace((char)3, 'M');
                Console.WriteLine(dateTimeExtension.Datetime_option.ToString(format));
            }
            else
                Console.WriteLine(dateTimeExtension.Datetime_option);
        }
    }
}