using System;
using System.Globalization;
using CommandLineParser;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Numactl
{
    public enum Commands
    {
        DoSomething,
        DoSomethingElse
    }
    class Numactl : ICommandDefinition
    {
        [Option(names: new string[]{"--hardware","-H"},
            HelpText = "Print hardware configuration.",
            MinParameterCount = 0,
            MaxParameterCount = 0,
            IsRequired = false

        )]
        public string Hardware { get; set; }

        [Option(names: new string[] {"--show", "-s"},
            MinParameterCount =0,
            MaxParameterCount =0,
            IsRequired = false
        )]
        public string Show { get; set; }

        [Option(names:new string[] {"--physcupbind","-C"},
            MinParameterCount =1,
            MaxParameterCount =1
        )]
        public string? Physcupbind { get; set; } = null;


        [Option(names: new string[] { "--membind", "-m" },
            MinParameterCount = 1,
            MaxParameterCount = 1
        )]
        public string? Membind { get; set; } = null;

        [Option(names: new string[] { "--preffered", "-p" },
            MinParameterCount = 1,
            MaxParameterCount = 1
        )]
        public int? Preferred { get; set; } = null;

        [Option(names: new string[] { "--interleave", "-i" },
            MinParameterCount = 1,
            MaxParameterCount = 1
        )]
        public string? Interleave { get; set; } = null;


        [Argument(order: 0, IsRequired = false)]
        public Commands? plainArgumentForCommand { get; set; } = null;

    }

    
    class Program
    {
        //The API does not allow user to check wether and option was on command line???, so even with the reflection
        //I cannot tell wether any option at all was present on the command line... so I had to do this the dumb way.
        //Also you can access help string only via reflection on the Numactl class which is really inconvenient... so
        //the user will weven prefer this dumb way of doing it???
        static bool checkPlainCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: numactl [--interleave= | -i <nodes>] [--preferred= | -p <node>]");
                Console.WriteLine("               [--physcpubind= | -C <cpus>] [--membind= | -m <nodes>]");
                Console.WriteLine("               command args ...");
                Console.WriteLine("       numactl [--show | -s]");
                Console.WriteLine("       numactl [--hardware | -H]");
                Console.WriteLine("<nodes> is a comma delimited list of node numbers or A-B ranges or all.");
                Console.WriteLine("<cpus> is a comma delimited list of cpu numbers or A-B ranges or all.");
                Console.WriteLine("");
                Console.WriteLine("--interleave, -i   Interleave memory allocation across given nodes.");
                Console.WriteLine("--preferred, -p    Prefer memory allocations from given node.");
                Console.WriteLine("--membind, -m      Allocate memory from given nodes only.");
                Console.WriteLine("--physcpubind, -C  Run on given CPUs only.");
                Console.WriteLine("--show, -S         Show current NUMA policy.");
                Console.WriteLine("--hardware, -H     Print hardware configuration.");
                return true;
            }

            else return false;

        }

        //Again I would like to know how am I supposed to find out, whether this option was present on the command line?
        //I guess that if it took some parameters, the property in numactl class Hardware would be filled with it?
        //But this option does not take arguments??? I cant even do this with reflection again, because there is no property
        //in the Attribute Option, which would tell whether it was present on the command line????
        static bool checkHardwareOption(string[] args)
        {
            if (args.Length == 1 && (args[0].ToLower() == "--hardware" || args[0].ToLower() == "-h"))
            {
                Console.WriteLine("available: 2 nodes (0-1)");
                Console.WriteLine("node 0 cpus: 0 2 4 6 8 10 12 14 16 18 20 22");
                Console.WriteLine("node 0 size: 24189 MB");
                Console.WriteLine("node 0 free: 18796 MB");
                Console.WriteLine("node 1 cpus: 1 3 5 7 9 11 13 15 17 19 21 23");
                Console.WriteLine("node 1 size: 24088 MB");
                Console.WriteLine("node 1 free: 16810 MB");
                Console.WriteLine("node distances:");
                Console.WriteLine("node   0   1");
                Console.WriteLine("  0:  10  20");
                Console.WriteLine("  1:  20  10");
                return true;

            }
            else return false;
        }

        //same problem as with the previous option...
        static bool checkShowOption(string[] args) {

            if (args.Length == 1 && (args[0].ToLower() == "--show" || args[0].ToLower() == "-s"))
            {
                Console.WriteLine("policy: default");
                Console.WriteLine("preferred node: current");
                Console.WriteLine("physcpubind: 0 1 2 3 4 5 6 7 8");
                Console.WriteLine("cpubind: 0 1");
                Console.WriteLine("nodebind: 0 1");
                Console.WriteLine("membind: 0 1");
                return true;

            }
            else return false;
        }

        static bool validatePolicyParameters(string policyParameters)
        {
            string[] policyparams = policyParameters.Split(',');
            string pattern = @"^(\d+|(?:\d+):(?:\d+)|all)$";
            foreach(var param in policyparams)
            {
                Match match = Regex.Match(param, pattern);
                if (!match.Success) return false;
            }
            return true;
        }
        static void checkNumaPoliciy(Numactl numactl)
        {
            //command must not be empty
            if (numactl.plainArgumentForCommand == null) throw new ArgumentException();

            //check what kind of NUMA policies are present
            if(numactl.Physcupbind!=null)
            {
                if(numactl.Interleave!=null||numactl.Membind!=null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    var succes = validatePolicyParameters(numactl.Physcupbind);
                    if (succes)
                    {
                        Console.WriteLine($"I run with Physcupbind and parameters {numactl.Physcupbind}");
                    }
                    else throw new ArgumentException();
                }
            }

            if(numactl.Interleave!=null)
            {
                if (numactl.Physcupbind != null || numactl.Membind != null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    var succes = validatePolicyParameters(numactl.Interleave);
                    if (succes)
                    {
                        Console.WriteLine($"I run with Interleave and parameters {numactl.Interleave}");
                    }
                    else throw new ArgumentException();
                }
            }

            if(numactl.Membind!=null)
            {
                if (numactl.Interleave != null || numactl.Physcupbind != null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    var succes = validatePolicyParameters(numactl.Membind);
                    if (succes)
                    {
                        Console.WriteLine($"I run with Membind and parameters {numactl.Membind}");
                    }
                    else throw new ArgumentException();
                }
            }

            if (numactl.Preferred != null)
            {
                
               Console.WriteLine($"I run with Preferred and parameters {numactl.Preferred}");
                
            }

           
        }

        static void CheckParsedValues(Numactl numactl, string[] args)
        {
            if (!checkPlainCommand(args))
            {
                bool was_show_or_hardware = checkShowOption(args) || checkHardwareOption(args);
                if (!was_show_or_hardware)
                {
                    checkNumaPoliciy(numactl);
                }
            }
            

            
        }
        static void Main(string[] args)
        {
            Numactl numactl= new Numactl();
            string input = string.Join(" ", args);
            var filledNumactl = CommandParser<Numactl>.Parse(input, numactl);

            CheckParsedValues(numactl, args);

        }
    }
}