using System.Reflection;

namespace CommandLineParser
{
    /// <summary>
    /// Parser for user defined command classes.
    /// </summary>
    /// <typeparam name="T">User defined command class.</typeparam>
    public static class CommandParser<T> where T : ICommandDefinition
    {
        /// <summary>
        /// Takes an empty command into which parameters will be filled in.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandInstance">Instance of an empty command into which parameters will be filled in.</param>
        /// <returns>Instance of the filled in command.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="MissingInterfaceException"></exception>
        /// <exception cref="CommandParserException"></exception>
        public static T Parse(string command, T commandInstance)
        {
            //partial implementation...
            if (commandInstance == null) throw new NullReferenceException(nameof(commandInstance));
            if (!typeof(T).GetInterfaces().Contains(typeof(ICommandDefinition))) throw new MissingInterfaceException("Class " + typeof(T).Name + " does not implement ICommandDefinition interface.");

            ParseOptions(command, commandInstance);
            ParseArguments(command, commandInstance);

            return commandInstance;
        }

        private static T ParseOptions(string command, T commandInstance)
        {
            //partial implementation...
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);

            }

            return commandInstance;
        }

        private static T ParseArguments(string command, T commandInstance)
        {
            //partial implementation...
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                property.GetCustomAttributes<Argument>(false).FirstOrDefault(defaultValue: null);
            }

            return commandInstance;
        }
    }
}