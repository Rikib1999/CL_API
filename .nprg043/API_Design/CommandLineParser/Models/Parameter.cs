using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParser.Models
{
    public class Parameter
    {
        public ParameterType Type { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsParameterCorrect(string value)
        {
            return false;
        }
        private bool TryParse()
        {
            return false;
        }
    }
}
