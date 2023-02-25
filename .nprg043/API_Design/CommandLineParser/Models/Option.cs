using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParser.Models
{
    public class Option
    {
        public string Name { get; set; }
        public List<string> Synonyms { get; set; }
        public Parameter Arguments { get; set; }
        public string HelpText { get; set; }

        public bool ContainsThisName(string optionName)
        {
            return false;
        }

        public bool IsOptionCorrect()
        {
            return false;
        }
    }
}
