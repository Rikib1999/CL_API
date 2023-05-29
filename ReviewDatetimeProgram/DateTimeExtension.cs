using CommandLineParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewDatetimeProgram
{
    
    public class DateTimeExtension : ICommandDefinition
    {
        [Option(names: new string[] { "--date" }
            , MinParameterCount = 1
            , MaxParameterCount = 1, IsRequired = true
            , Delimeter = '='
        )]
        public DateTime Datetime_option { get; set; }

        [Option(names: new string[] {"--format" }
            , HelpText = "Specify output format, possibly overriding the format specified in the environment variable TIME."
            , MinParameterCount = 1
            , MaxParameterCount = 1
            , Delimeter = '='
            , Dependencies = new string[] { "--date" }
        )]
        public string Format { get; set; }
    }
}
