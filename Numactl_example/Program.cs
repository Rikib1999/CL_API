
using CommandLineParser;

namespace ExampleProgram2
{
    public class Program
    {
        static void Main(string[] args)
        {
            Numactl numactlCommand = new Numactl();
            string commandLineInput = Console.ReadLine();
            numactlCommand = CommandParser<Numactl>.Parse(commandLineInput, numactlCommand);
            if(numactlCommand.Hardware){
                Console.WriteLine("Writing hardware configuration");
            }else{
                if(numactlCommand.Policy){
                    Console.WriteLine("policy: default");
                }else{
                    Console.WriteLine("will run command: " + numactlCommand.Command);
                    Console.WriteLine("CPU node bind: " + numactlCommand.CPUs);
                    Console.WriteLine("Interleave across nodes: " + numactlCommand.Interleave);
                    Console.WriteLine("Preferred node: " + numactlCommand.Preffered);
                    Console.WriteLine("Allocate from given nodes: " + numactlCommand.Membind);
                }
            }
        }
    }
}