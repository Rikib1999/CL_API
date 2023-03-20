﻿using CommandLineParser;

namespace ExampleProgram
{
    public class Program
    {
        static void Main(string[] args)
        {
            Time timeCommand = new Time();
            string commandLineInput = Console.ReadLine();
            timeCommand = CommandParser<Time>.Parse(commandLineInput, timeCommand);
        }
    }
}