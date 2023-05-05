using System.Collections;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;

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

            CommandReferenceBag commandReferenceBag = CommandReferencer.AddCommandReference(commandInstance);

            ParseOptions(command, commandInstance, commandReferenceBag);
            ParseArguments(command, commandInstance, commandReferenceBag);

            CommandReferencer.CleanUnusedCommandReferences();

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

        private static T ParseOptions(string command, T commandInstance, CommandReferenceBag commandReferenceBag)
        {
            command = LeaveOneSpace(command);

            PropertyInfo[] properties = typeof(T).GetProperties();

            List<string> allOptionNames = new();

            foreach (PropertyInfo property in properties)
            {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option == null) continue;

                //type checking //TODO: dat do metody
                Type propType = property.PropertyType;

                bool isArray = propType.IsArray;
                bool isList = property is IList && propType.IsGenericType;

                if (!(isArray || isList) && option.MaxParameterCount > 1) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");

                Type coreType = propType;

                if (option.MaxParameterCount > 0)
                {
                    bool isSupported = false;
                    foreach (Type t in supportedTypes)
                    {
                        if (propType == t)
                        {
                            isSupported = true;
                            break;
                        }
                    }

                    //class with string contructor
                    if (propType.GetConstructor(new Type[] { typeof(string) }) != null) isSupported = true;

                    Type[] typeArgs = propType.GetGenericArguments();
                    if (isList && typeArgs != null && typeArgs.Length == 1 && typeArgs[0].GetConstructor(new Type[] { typeof(string) }) != null)
                    {
                        coreType = typeArgs[0];
                        isSupported = true;
                    }

                    Type typeArg = propType.GetElementType();
                    if (isArray && typeArg != null && typeArg.GetConstructor(new Type[] { typeof(string) }) != null)
                    {
                        coreType = typeArg;
                        isSupported = true;
                    }

                    if (!isSupported) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Type of option [" + propType.Name + "] is not supported.");
                }

                string helpTextNames = string.Join(", ", option.Names);

                //option name validity //TODO: dat do metody
                foreach (string name in option.Names)
                {
                    if (string.IsNullOrEmpty(name) || name.Length <= 1 || !name.StartsWith('-')) throw new CommandParserException(name + " is not a valid name for an option.");

                    if (allOptionNames.Contains(name)) throw new CommandParserException("Option name [" + name + "] occurs multiple times.");
                    allOptionNames.Add(name);

                    //helptext
                    commandReferenceBag.HelpTexts[name] = helpTextNames + " : " + option.HelpText;
                }

                commandReferenceBag.AllHelpText += helpTextNames + " : " + option.HelpText + "\n";

                //TODO: dat do metody
                //najskor ju najdem
                int startIndex = -1;
                string usedName = "";

                foreach (string name in option.Names)
                {
                    startIndex = command.IndexOf(name);
                    if (startIndex >= 0)
                    {
                        usedName = name;
                        break;
                    }
                }

                //ak som nenasiel a je optional tak continue, inak throw exception
                if (option.IsRequired && startIndex < 0) throw new CommandParserException("Required option " + option.Names[0] + " is missing.");

                //option is present
                if (startIndex >= 0)
                {
                    foreach (string name in option.Names)
                    {
                        commandReferenceBag.ArgsPresence[name] = true;
                    }
                }
                else
                {
                    continue;
                }

                //najdem vsetky parametre
                List<string> parameters = new();
                int currentPosition = startIndex + usedName.Length + 1;

                while (parameters.Count < option.MaxParameterCount && currentPosition < command.Length)
                {
                    if (command[currentPosition] == '-') break; //narazili sme na dalsiu option alebo plain arguments

                    StringBuilder sb = new();

                    while (currentPosition < command.Length && (command[currentPosition] != ' ' || command[currentPosition] != option.Delimeter))
                    {
                        sb.Append(command[currentPosition]);
                        currentPosition++;
                    }

                    parameters.Add(sb.ToString());
                    sb.Clear();

                    currentPosition++;
                }

                startIndex += usedName.Length;
                int endIndex = currentPosition - 1;

                if (parameters.Count < option.MinParameterCount) throw new CommandParserException("Option " + option.Names[0] + " has insufficient number of parameters.");

                command = command.Remove(startIndex, endIndex - startIndex); //zmazem parametre optiony pre jednoduchsie hladanie argumentov

                if (parameters.Count == 0) continue;

                var castMethod = typeof(ValueCaster).GetMethod("Cast");
                var castOfTypeMethod = castMethod.MakeGenericMethod(new[] { propType });

                if (isArray)
                {
                    Type arrayType = propType.MakeArrayType();
                    var array = (IList)Activator.CreateInstance(arrayType);

                    foreach (string p in parameters)
                    {
                        array.Add(castOfTypeMethod.Invoke(null, new object[] { p }));
                    }

                    property.SetValue(commandInstance, array);
                }
                else if (isList)
                {
                    Type genericListType = typeof(List<>).MakeGenericType(propType);
                    var list = (IList)Activator.CreateInstance(genericListType);

                    foreach (string p in parameters)
                    {
                        list.Add(castOfTypeMethod.Invoke(null, new object[] { p }));
                    }

                    property.SetValue(commandInstance, list);
                }
                else if (parameters.Count == 1)
                {
                    property.SetValue(commandInstance, castOfTypeMethod.Invoke(null, new object[] { parameters[0] }));
                }

                //skontrolovat boundaries
                Boundaries<IComparable> boundaries = (Boundaries<IComparable>)property.GetCustomAttributes(typeof(Boundaries<IComparable>), false).FirstOrDefault(defaultValue: null);
                if (boundaries == null) continue;

                if (isArray || isList)
                {
                    foreach (IComparable v in (IList)property.GetValue(commandInstance))
                    {
                        if (v.CompareTo(boundaries.LowerBound) < 0 || v.CompareTo(boundaries.UpperBound) > 0) throw new CommandParserException("Option " + option.Names[0] + " has a parameter outside of its boundaries.");
                    }
                }
                else if (parameters.Count == 1)
                {
                    IComparable v = (IComparable)property.GetValue(commandInstance);

                    if (v.CompareTo(boundaries.LowerBound) < 0 || v.CompareTo(boundaries.UpperBound) > 0) throw new CommandParserException("Option " + option.Names[0] + " has a parameter outside of its boundaries.");
                }
            }

            //skontrolovat exclusivities a dependencies, inak exception
            foreach (PropertyInfo property in properties)
            {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option == null) continue;

                if (option.Dependencies != null && commandReferenceBag.ArgsPresence[option.Names[0]])
                {
                    foreach (string dep in option.Dependencies)
                    {
                        if (!commandReferenceBag.ArgsPresence[dep]) throw new CommandParserException("Dependency " + dep + " of option " + option.Names[0] + " is missing.");
                    }
                }

                if (option.Exclusivities != null && commandReferenceBag.ArgsPresence[option.Names[0]])
                {
                    foreach (string excl in option.Exclusivities)
                    {
                        if (commandReferenceBag.ArgsPresence[excl]) throw new CommandParserException("Exclusivity " + excl + " of option " + option.Names[0] + " is present.");
                    }
                }
            }

            return commandInstance;
        }

        //ak je za tym este parameter, tak to bud bude argument alebo sa pri parsovani argumentu hodi vynimka

        //delete option arguments, so parsing plain arguments will be easier, just skipping - or --

        private static T ParseArguments(string command, T commandInstance, CommandReferenceBag commandReferenceBag)
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