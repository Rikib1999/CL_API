using CommandLineParser;
using System.Text.RegularExpressions;
namespace ExampleProgram2
{
    public class Numactl : ICommandDefinition
    {
        [Option(names: new string[]{"--hardware", "-H"},
            HelpText = "Prints the current hardware configuration",
            MaxParameterCount = 0
            )]
            public bool Hardware { get; set; }

        [Option(names: new string[]{"--show", "-s"},
            HelpText = "Prints the current policy",
            MaxParameterCount = 0
            )]
            public bool Policy {get; set;}
        
        [Option(names: new string[]{"-m", "--membind"},
            HelpText = "Allocate memory from given nodes only - takes a comma-delimited list of node numbers or A-B ranges or all",
            MaxParameterCount = 1,
            MinParameterCount = 1
            )]
            public string Membind {
                get{
                    return Membind;
                }
                set
                {
                    if (value=="all" || Regex.IsMatch(value, @"^([0-3]-[0-3]|[0-3])(,([0-3]-[0-3]|[0-3]))*$")){
                        Membind = value;
                    }else{
                        throw new CommandParserException(value + " is not a valid value for Membind field");
                    }
                } 
            }

        [Option(names: new string[]{"-p", "--preferred"},
            HelpText = "Prefer memory allocations from a given node",
            MaxParameterCount = 1,
            MinParameterCount = 1
            )]
            public string Preffered {
                get{
                    return Preffered;
                } 
                set{
                    if(value == "0" || value == "1" || value == "2" || value == "3"){
                        Preffered = value;
                    }else{
                        throw new CommandParserException(value + " is not a valid value for Preferred field");
                    }
                }}
        
        [Option(names: new string[]{"-i", "--interleave"},
            HelpText = "Interleave memory allocation accros given nodes",
            MaxParameterCount = 0
            )]
            public string Interleave {
                get{
                    return Interleave;
                }
                set
                {
                    if (value=="all" || Regex.IsMatch(value, @"^([0-3]-[0-3]|[0-3])(,([0-3]-[0-3]|[0-3]))*$")){
                        Interleave = value;
                    }else{
                        throw new CommandParserException(value + " is not a valid value for Interleave field");
                    }
                } 
                
            }

        [Option(names: new string[]{"-C", "--physcpubind"},
            HelpText = "Run on given cpus only - takes a comma delimited list of cpu numbers or A-B ranges or all",
            MaxParameterCount = 1,
            MinParameterCount = 1
            )]
            public string CPUs {
            get
            {
                return CPUs;
            }
            set{
                {
                    if (value=="all" || Regex.IsMatch(value, @"^(\d+-\d+|\d+)(,(\d+-\d+|\d+))*$")){
                        CPUs = value;
                    }else{
                        throw new CommandParserException(value + " is not a valid value for CPUs field");
                    }
                } 

            }}

        [Argument(order: 0, IsRequired = true)]
        public string Command {get; set;}

    }
}