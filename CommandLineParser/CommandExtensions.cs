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
            return true;
        }

        /// <summary>
        /// Returns help text of given option or argument.
        /// </summary>
        /// <param name="name">Any of the names of an option or argument.</param>
        /// <returns></returns>
        public static string GetHelpText(this ICommandDefinition commandDefinition, string name)
        {
            return $"help text for command {name}";
        }

        /// <summary>
        /// Returns help text for the whole command.
        /// </summary>
        /// <returns></returns>
        public static string GetHelpText(this ICommandDefinition commandDefinition)
        {
            return "help text";
        }
    }
}