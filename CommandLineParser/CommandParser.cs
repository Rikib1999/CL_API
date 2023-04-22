using System.Collections;
using System.Reflection;

namespace CommandLineParser
{
    /// <summary>
    /// Parser for user defined command classes.
    /// </summary>
    /// <typeparam name="T">User defined command class.</typeparam>
    public static class CommandParser<T> where T : ICommandDefinition
    {
        private static readonly Type[] supportedTypes = {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            typeof(Enum),
            typeof(bool[]),
            typeof(byte[]),
            typeof(sbyte[]),
            typeof(char[]),
            typeof(string[]),
            typeof(int[]),
            typeof(uint[]),
            typeof(short[]),
            typeof(ushort[]),
            typeof(long[]),
            typeof(ulong[]),
            typeof(float[]),
            typeof(double[]),
            typeof(decimal[]),
            typeof(DateTime[]),
            typeof(Enum[]),
            typeof(List<bool>),
            typeof(List<byte>),
            typeof(List<sbyte>),
            typeof(List<char>),
            typeof(List<string>),
            typeof(List<int>),
            typeof(List<uint>),
            typeof(List<short>),
            typeof(List<ushort>),
            typeof(List<long>),
            typeof(List<ulong>),
            typeof(List<float>),
            typeof(List<double>),
            typeof(List<decimal>),
            typeof(List<DateTime>),
            typeof(List<Enum>)
        };

        /// <summary>
        /// Takes an empty command into which parameters will be filled in.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandInstance">Instance of an empty command into which parameters will be filled in.</param>
        /// <returns>Instance of the filled in command.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="MissingInterfaceException"></exception>
        /// <exception cref="CommandParserException"></exception>
        /// /// <exception cref="ArgumentException"></exception>
        public static T Parse(string command, T commandInstance)
        {
            if (commandInstance == null) throw new NullReferenceException(nameof(commandInstance));
            if (!typeof(T).GetInterfaces().Contains(typeof(ICommandDefinition))) throw new MissingInterfaceException("Class " + typeof(T).Name + " does not implement ICommandDefinition interface.");
            if (string.IsNullOrEmpty(command)) throw new ArgumentException("Command can not be null or empty.");

            ParseOptions(command, commandInstance);
            ParseArguments(command, commandInstance);

            return commandInstance;
        }

        /// <summary>
        /// Takes an empty command into which parameters will be filled in.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandInstance">Instance of an empty command into which parameters will be filled in.</param>
        /// <returns>Instance of the filled in command.</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="MissingInterfaceException"></exception>
        /// <exception cref="CommandParserException"></exception>
        public static T Parse(string[] args, T commandInstance)
        {
            return Parse(string.Join(' ', args), commandInstance);
        }

        private static T ParseOptions(string command, T commandInstance)
        {
            command = LeaveOneSpace(command);

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option == null) continue;

                //type checking //TODO: dat do metody
                Type propType = property.PropertyType;

                if (option.MaxParameterCount > 0)
                {
                    bool isSupported = false;
                    foreach (Type t in supportedTypes)
                    {
                        if (propType == t) isSupported = true; break;
                    }

                    if (!isSupported) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Type of option [" + propType.Name + "] is not supported.");
                }

                bool isArray = propType.IsArray;
                bool isList = property is IList && propType.IsGenericType;

                if (!(isArray || isList) && option.MaxParameterCount > 1) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");

                //option name validity //TODO: dat do metody
                foreach (string name in option.Names)
                {
                    if (string.IsNullOrEmpty(name) || name.Length <= 1 || !name.StartsWith('-')) throw new CommandParserException(name + " is not a valid name for an option.");
                }

                //TODO: dat do metody
                //najskor ju najdem
                //command.IndexOf(option.Names)

                //ak som nenasiel a je optional tak continue, inak throw exception
                //podla poctu parsujem
                //ak 0 tak neparsujem
                //ak 1 tak parsujem rovno - ak bude 0 skontrolujem minCount a pripadne hodim vynimku
                //ak viac tak parsujem maxCount - skontrolujem minCount a pripadne hodim vynimku
                //ak je za tym este parameter, tak to bud bude argument alebo sa pri parsovani argumentu hodi vynimka
                //ulozim si start a end index


                if (isArray)
                {

                }

                if (isList) { }
                //else if it is a single variable



                //delete option arguments, so parsing plain arguments will be easier, just skipping - or --
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

        private static string LeaveOneSpace(string s)
        {
            return string.Join(" ", s.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries).ToList().Select(x => x.Trim()));
        }
    }
}