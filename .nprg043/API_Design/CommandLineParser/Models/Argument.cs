using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParser.Models
{
    public class Argument
    {
        public Type Type { get; set; }
        public Count Count { get; set; }
        public bool IsMandatory { get; set; }
    }
}
