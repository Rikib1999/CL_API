using System.Reflection;

namespace CommandLineParser
{
    internal class CommandReferenceBag
    {
        internal WeakReference Reference { get; set; }
        internal Dictionary<string, bool> ArgsPresence { get; set; }
        internal Dictionary<string, string> HelpTexts { get; set; }
        internal string AllHelpText { get; set; }

        public CommandReferenceBag(WeakReference reference, Type t)
        {
            Reference = reference;

            CreateArgsDictionary(t, out Dictionary<string, bool> presence, out Dictionary<string, string> helpTexts);
            ArgsPresence = presence;
            HelpTexts = helpTexts;
            AllHelpText = "";
        }

        private void CreateArgsDictionary(Type t, out Dictionary<string, bool> presence, out Dictionary<string, string> helpTexts)
        {
            presence = new();
            helpTexts = new();

            PropertyInfo[] properties = t.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option != null)
                {
                    foreach (string n in option.Names)
                    {
                        if (presence.ContainsKey(n)) throw new CommandParserException("Option name [" + n + "] occurs multiple times.");
                        presence.Add(n, false);
                        helpTexts.Add(n, "");
                    }

                    continue;
                }

                Argument argument = property.GetCustomAttributes<Argument>(false).FirstOrDefault(defaultValue: null);
                if (argument != null)
                {
                    if (presence.ContainsKey(argument.Name)) throw new CommandParserException("Argument name [" + argument.Name + "] occurs multiple times.");
                    presence.Add(argument.Name, false);
                    helpTexts.Add(argument.Name, "");
                }
            }
        }
    }
}