using CommandLineParser.Models;
using CommandLineParser.ParsedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParser.Parsers
{
    public class CommandParser
    {
        private readonly string line;
        private readonly Command command;

        private Option LastOption { get; set; }
        private bool MultipleArguments { get; set; }
        private List<string> Arguments { get; set; }
        private bool IsCorrect { get; set; }
        private bool ShouldBeParameter { get; set; }
        private bool ShouldBeOption { get; set; }

        public ParsedCommand ParsedCommand { get; private set; }

        public CommandParser(string line, Command command)
        {
            this.line = line;
            this.command = command;
        }

        public void ParseCommand()
        {

        }

        public bool IsLineCorrect()
        {
            return false;
        }
    }
}
