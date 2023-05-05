namespace CommandLineParser
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Checks if given option or argument is present in this command.
        /// </summary>
        /// <param name="name">Any of the names of an option or argument.</param>
        public static bool IsPresent(this ICommandDefinition commandDefinition, string name)
        {
            foreach (var cr in CommandReferencer.commandReferences)
            {
                if (cr.Reference.Target == commandDefinition)
                {
                    if (!cr.ArgsPresence.ContainsKey(name)) return false;
                    return cr.ArgsPresence[name];
                }
            }

            return false;
        }

        /// <summary>
        /// Returns help text of given option or argument.
        /// </summary>
        /// <param name="name">Any of the names of an option or argument.</param>
        /// <returns></returns>
        public static string GetHelpText(this ICommandDefinition commandDefinition, string name)
        {
            foreach (var cr in CommandReferencer.commandReferences)
            {
                if (cr.Reference.Target == commandDefinition)
                {
                    if (!cr.HelpTexts.ContainsKey(name)) return "";
                    return cr.HelpTexts[name];
                }
            }

            return "";
        }

        /// <summary>
        /// Returns help text for the whole command.
        /// </summary>
        /// <returns></returns>
        public static string GetHelpText(this ICommandDefinition commandDefinition)
        {
            foreach (var cr in CommandReferencer.commandReferences)
            {
                if (cr.Reference.Target == commandDefinition)
                {
                    return cr.AllHelpText;
                }
            }

            return "";
        }
    }
}