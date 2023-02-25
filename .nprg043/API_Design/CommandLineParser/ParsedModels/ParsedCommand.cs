using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParser.ParsedModels
{
    public class ParsedCommand
    {
        public string Name { get; set; }
        public List<ParsedArgument> ParsedArguments { get; set; }
        public List<ParsedOption> ParsedOptions { get; set; }
    }
}
