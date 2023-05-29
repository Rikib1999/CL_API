using System.Collections;
using System.Reflection;
using System.Text;

namespace CommandLineParser {
    /// <summary>
    /// Parser for user defined command classes.
    /// </summary>
    /// <typeparam name="T">User defined command class.</typeparam>
    public static class CommandParser<T> where T : ICommandDefinition {
        private enum GeneralType {
            Simple,
            Enum,
            Array,
            List
        }

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
        public static T Parse(string command, T commandInstance, bool beginsWithName = false) {
            if (commandInstance == null) {
                throw new NullReferenceException(nameof(commandInstance));
            }

            if (!typeof(T).GetInterfaces().Contains(typeof(ICommandDefinition))) {
                throw new MissingInterfaceException("Class " + typeof(T).Name + " does not implement ICommandDefinition interface.");
            }

            if (beginsWithName) {
                command = command.Substring(command.IndexOf(" ") + 1);
            }

            if (string.IsNullOrEmpty(command)) {
                throw new ArgumentException("Command can not be null or empty.");
            }

            CommandReferenceBag commandReferenceBag = CommandReferencer.AddCommandReference(commandInstance);

            int argsDelimeterIndex = FindArgsDelimeter(ref command);
            string optionsPart = command.Substring(0, argsDelimeterIndex + 1);
            string argsPart = command.Substring(argsDelimeterIndex + 1, command.Length - (argsDelimeterIndex + 1));
            if (argsDelimeterIndex == -1) {
                (argsPart, optionsPart) = (optionsPart, argsPart);
                argsDelimeterIndex = int.MaxValue;
            }

            commandInstance = ParseOptions(commandInstance, commandReferenceBag, ref optionsPart);
            string newCommand = optionsPart + argsPart;
            commandInstance = ParseArguments(commandInstance, commandReferenceBag, argsDelimeterIndex, ref newCommand);

            if (!string.IsNullOrWhiteSpace(newCommand)) {
                throw new CommandParserException("Abundant options, arguments or parameters found.");
            }

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
        public static T Parse(string[] args, T commandInstance, bool beginsWithName = false) {
            return Parse(string.Join(' ', args), commandInstance, beginsWithName);
        }

        private static string LeaveOneSpace(string s) {
            return string.Join(" ", s.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries).ToList().Select(x => x.Trim()));
        }

        private static void DelimeterCheck(Option option) {
            if (option.Delimeter == '-') {
                throw new CommandParserException("Delimeter of option " + option.Names[0] + " can not be '-'.");
            }
            if (option.Delimeter == '-') {
                throw new CommandParserException("Delimeter of option " + option.Names[0] + " can not be '.'.");
            }
        }

        private static GeneralType CheckType(PropertyInfo property, out Type propType, out Type coreType) {
            Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
            Argument argument = property.GetCustomAttributes<Argument>(false).FirstOrDefault(defaultValue: null);

            propType = property.PropertyType;

            GeneralType generalType = GeneralType.Simple;
            if (propType.IsEnum) {
                generalType = GeneralType.Enum;
            }

            if (propType.IsArray) {
                generalType = GeneralType.Array;
            }

            if (propType.IsGenericType && (propType.GetGenericTypeDefinition() == typeof(List<>))) {
                generalType = GeneralType.List;
            }

            if (option != null && !(generalType == GeneralType.Array || generalType == GeneralType.List) && option.MaxParameterCount > 1) {
                throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");
            }

            coreType = propType;

            if (option == null || option.MaxParameterCount > 0) {
                bool isSupported = false;
                foreach (Type t in supportedTypes) {
                    if (propType == t) {
                        isSupported = true;
                        break;
                    }
                }

                if (generalType == GeneralType.Enum) { 
                    isSupported = true;
                }
                //class with string contructor
                if (propType.GetConstructor(new Type[] { typeof(string) }) != null) {
                    isSupported = true;
                }

                Type[] typeArgs = propType.GetGenericArguments();
                if (generalType == GeneralType.List && typeArgs != null && typeArgs.Length == 1) {
                    if (typeArgs[0].GetConstructor(new Type[] { typeof(string) }) != null || typeArgs[0].IsEnum) { 
                        isSupported = true;
                    }
                    coreType = typeArgs[0];
                }

                Type typeArg = propType.GetElementType();
                if (generalType == GeneralType.Array && typeArg != null) {
                    if (typeArg.GetConstructor(new Type[] { typeof(string) }) != null || typeArg.IsEnum) isSupported = true;
                    coreType = typeArg;
                }

                if (!isSupported) {
                    if (option != null) {
                        throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Type of option [" + propType.Name + "] is not supported.");
                    }
                    if (argument != null) {
                        throw new CommandParserException("Argument " + argument.Name + " can not be parsed. Type of argument [" + propType.Name + "] is not supported.");
                    }
                }
            }

            return generalType;
        }

        private static void CheckNamesAndHelpText(Option option, CommandReferenceBag commandReferenceBag) {
            string helpTextNames = string.Join(", ", option.Names);

            foreach (string name in option.Names) {
                if (string.IsNullOrEmpty(name) || name.Length <= 1 || !name.StartsWith('-')) {
                    throw new CommandParserException(name + " is not a valid name for an option.");
                }

                commandReferenceBag.HelpTexts[name] = helpTextNames + " : " + option.HelpText;
            }

            commandReferenceBag.AllHelpText += helpTextNames + " : " + option.HelpText + "\n";
        }

        private static void CheckNamesAndHelpText(Argument argument, CommandReferenceBag commandReferenceBag) {
            if (string.IsNullOrEmpty(argument.Name)) {
                throw new CommandParserException("Argument name can not be empty.");
            }
            commandReferenceBag.HelpTexts[argument.Name] = argument.Name + " : " + argument.HelpText;
            commandReferenceBag.AllHelpText += argument.Name + " : " + argument.HelpText + "\n";
        }

        private static int FindArgsDelimeter(ref string command) {
            bool insideQuotes = false;
            bool escapedQuote = false;
            bool escapedBackslash = false;

            for (int i = 0; i < command.Length - 3; i++) {
                if (!insideQuotes && command.Substring(i, 4) == " -- ") {
                    command = command.Remove(i, 3);
                    return i;
                }

                char currentChar = command[i];

                if (currentChar == '\"' && !escapedQuote) {
                    insideQuotes = !insideQuotes;
                }
                else if (currentChar == '\\' && insideQuotes) {
                    if (!escapedBackslash) {
                        escapedBackslash = true;
                        escapedQuote = true;
                    }
                    else {
                        escapedBackslash = false;
                    }
                }
                else {
                    escapedQuote = false;
                    escapedBackslash = false;
                }
            }

            return -1;
        }

        private static int FindOption(Option option, string command, out string usedName) {
            int startIndex = -1;
            usedName = "";

            foreach (string name in option.Names) {
                startIndex = command.IndexOf(name);
                if (startIndex >= 0) {
                    usedName = name;
                    break;
                }
            }

            return startIndex;
        }

        private static string FindParameters(string command, Option option, string usedName, int startIndex, out List<string> parameters) {
            parameters = new();
            StringBuilder sb = new();
            int currentPosition = startIndex + usedName.Length + 1;
            bool insideQuotes = false;
            bool escapedQuote = false;
            bool escapedBackslash = false;
            bool minEmptyStr = false;

            while (parameters.Count < option.MaxParameterCount && currentPosition < command.Length) {
                if (command[currentPosition] == '-') //narazili sme na dalsiu option alebo plain arguments
                {
                    if (command.Length == currentPosition + 1 || command[currentPosition + 1] < '0' || command[currentPosition + 1] > '9') {
                        break;
                    }
                }

                while (currentPosition < command.Length && ((command[currentPosition] != ' ' && command[currentPosition] != option.Delimeter) || insideQuotes)) {
                    char currentChar = command[currentPosition];

                    if (currentChar == '\"' && !escapedQuote) {
                        insideQuotes = !insideQuotes;
                        minEmptyStr = true;
                    }
                    else if (currentChar == '\\' && insideQuotes) {
                        if (!escapedBackslash) {
                            escapedBackslash = true;
                            escapedQuote = true;
                        }
                        else {
                            sb.Append('\\');
                            escapedBackslash = false;
                        }
                    }
                    else {
                        escapedQuote = false;
                        escapedBackslash = false;
                        sb.Append(currentChar);
                    }

                    currentPosition++;
                }

                parameters.Add(sb.ToString());
                currentPosition++;
                sb.Clear();
            }

            startIndex += usedName.Length;
            int endIndex = currentPosition - 1;

            if (parameters.Count < option.MinParameterCount && !minEmptyStr) {
                throw new CommandParserException("Option " + option.Names[0] + " has insufficient number of parameters.");
            }

            return command.Remove(startIndex, endIndex - startIndex); //zmazem parametre optiony pre jednoduchsie hladanie argumentov
        }

        private static List<(string, int)> FindArgumentParameters(ref string command, int argsDelimeterIndex, bool findMultiple) {
            List<(string, int)> paramsAndIndices = new();
            if (string.IsNullOrEmpty(command)) {
                return paramsAndIndices;
            }

            bool ignoreDash = argsDelimeterIndex <= 0;
            int currentPosition = 0;

            //posuniem sa za zacinajuce optiony
            while (command[currentPosition] == '-' && !ignoreDash) {
                if (!command.Contains(' ') || command.IndexOf(' ', currentPosition + 1) == -1) {
                    return paramsAndIndices;
                }

                currentPosition = command.IndexOf(' ', currentPosition + 1) + 1;
            }

            command = command.Substring(currentPosition).Trim();
            if (argsDelimeterIndex != int.MaxValue) {
                argsDelimeterIndex -= currentPosition;
            }

            currentPosition = 0;

            StringBuilder sb = new();
            bool insideQuotes = false;
            bool escapedQuote = false;
            bool escapedBackslash = false;

            while (currentPosition < command.Length) {
                ignoreDash = currentPosition >= argsDelimeterIndex;

                if (paramsAndIndices.Count == 1 && !findMultiple) {
                    break;
                }
                else {
                    if (!ignoreDash && argsDelimeterIndex != int.MaxValue) {
                        command = command.Remove(argsDelimeterIndex).Trim();
                    }
                }

                if (command[currentPosition] == '-' && !ignoreDash) //narazili sme na dalsiu option
                {
                    if (command.Length == currentPosition + 1 || command[currentPosition + 1] < '0' || command[currentPosition + 1] > '9') {
                        break;
                    }
                }

                while (currentPosition < command.Length && (command[currentPosition] != ' ' || insideQuotes)) {
                    char currentChar = command[currentPosition];

                    if (currentChar == '\"' && !escapedQuote) {
                        insideQuotes = !insideQuotes;
                    }
                    else if (currentChar == '\\' && insideQuotes) {
                        if (!escapedBackslash) {
                            escapedBackslash = true;
                            escapedQuote = true;
                        }
                        else {
                            sb.Append('\\');
                            escapedBackslash = false;
                        }
                    }
                    else {
                        escapedQuote = false;
                        escapedBackslash = false;
                        sb.Append(currentChar);
                    }

                    currentPosition++;
                }

                paramsAndIndices.Add((sb.ToString(), currentPosition - sb.ToString().Length));
                currentPosition++;
                sb.Clear();
            }

            return paramsAndIndices;
        }

        private static int PopulateCommandInstance(ref T commandInstance, PropertyInfo property, GeneralType generalType, Type coreType, List<string> parameters, bool isOption = true) {
            var castMethod = typeof(ValueCaster).GetMethod("Cast");
            if (coreType is not IConvertible && coreType.GetConstructor(new Type[] { typeof(string) }) != null) {
                castMethod = typeof(ValueCaster).GetMethod("CastClass");
            }

            var castOfTypeMethod = castMethod.MakeGenericMethod(new[] { coreType });

            if (generalType == GeneralType.Array) {
                var array = (IList)Array.CreateInstance(coreType, parameters.Count);

                for (int i = 0; i < parameters.Count; i++) {
                    try {
                        array[i] = castOfTypeMethod.Invoke(null, new object[] { parameters[i] });
                    }
                    catch (CommandParserException e) {
                        if (isOption) {
                            throw e;
                        }

                        return i;
                    }
                }

                property.SetValue(commandInstance, array);
            }
            else if (generalType == GeneralType.List) {
                Type genericListType = typeof(List<>).MakeGenericType(coreType);
                var list = (IList)Activator.CreateInstance(genericListType);

                foreach (string p in parameters) {
                    try {
                        list.Add(castOfTypeMethod.Invoke(null, new object[] { p }));
                    }
                    catch (CommandParserException e) {
                        if (isOption) {
                            throw e;
                        }

                        return parameters.IndexOf(p);
                    }
                }

                property.SetValue(commandInstance, list);
            }
            else {
                try {
                    property.SetValue(commandInstance, castOfTypeMethod.Invoke(null, new object[] { parameters[0] }));
                }
                catch (CommandParserException e) {
                    if (isOption) {
                        throw e;
                    }

                    return 0;
                }
            }

            return -1;
        }

        private static void CheckBoundaries(T commandInstance, PropertyInfo property, Option option, GeneralType generalType, List<string> parameters) {
            Boundaries<IComparable> boundaries = (Boundaries<IComparable>)property.GetCustomAttributes(typeof(Boundaries<IComparable>), false).FirstOrDefault(defaultValue: null);
            if (boundaries == null) return;

            if (generalType == GeneralType.Array || generalType == GeneralType.List) {
                foreach (IComparable v in (IList)property.GetValue(commandInstance)) {
                    if (v.CompareTo(boundaries.LowerBound) < 0 || v.CompareTo(boundaries.UpperBound) > 0) {
                        throw new CommandParserException("Option " + option.Names[0] + " has a parameter outside of its boundaries.");
                    }
                }
            }
            else if (parameters.Count == 1) {
                IComparable v = (IComparable)property.GetValue(commandInstance);

                if (v.CompareTo(boundaries.LowerBound) < 0 || v.CompareTo(boundaries.UpperBound) > 0) {
                    throw new CommandParserException("Option " + option.Names[0] + " has a parameter outside of its boundaries.");
                }
            }
        }

        private static void CheckDependencies(Option option, CommandReferenceBag commandReferenceBag) {
            if (option.Dependencies != null && commandReferenceBag.ArgsPresence[option.Names[0]]) {
                foreach (string dep in option.Dependencies) {
                    if (!commandReferenceBag.ArgsPresence[dep]) {
                        throw new CommandParserException("Dependency " + dep + " of option " + option.Names[0] + " is missing.");
                    }
                }
            }
        }

        private static void CheckExclusivities(Option option, CommandReferenceBag commandReferenceBag) {
            if (option.Exclusivities != null && commandReferenceBag.ArgsPresence[option.Names[0]]) {
                foreach (string excl in option.Exclusivities) {
                    if (commandReferenceBag.ArgsPresence[excl]) {
                        throw new CommandParserException("Exclusivity " + excl + " of option " + option.Names[0] + " is present.");
                    }
                }
            }
        }

        private static T ParseOptions(T commandInstance, CommandReferenceBag commandReferenceBag, ref string command) {
            command = LeaveOneSpace(command);

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties) {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option == null) {
                    continue;
                }

                DelimeterCheck(option);

                GeneralType generalType = CheckType(property, out Type propType, out Type coreType);

                CheckNamesAndHelpText(option, commandReferenceBag);

                int startIndex = FindOption(option, command, out string usedName);

                if (option.IsRequired && startIndex < 0) {
                    throw new CommandParserException("Required option " + option.Names[0] + " is missing.");
                }

                //option is present
                if (startIndex >= 0) {
                    foreach (string name in option.Names) {
                        commandReferenceBag.ArgsPresence[name] = true;
                    }
                }
                else {
                    continue;
                }

                command = FindParameters(command, option, usedName, startIndex, out List<string> parameters);

                if (parameters.Count == 0) {
                    continue;
                }

                PopulateCommandInstance(ref commandInstance, property, generalType, coreType, parameters);

                CheckBoundaries(commandInstance, property, option, generalType, parameters);
            }

            foreach (PropertyInfo property in properties) {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);
                if (option == null) {
                    continue;
                }

                CheckDependencies(option, commandReferenceBag);
                CheckExclusivities(option, commandReferenceBag);
            }

            return commandInstance;
        }

        private static T ParseArguments(T commandInstance, CommandReferenceBag commandReferenceBag, int argsDelimeterIndex, ref string command) {
            PropertyInfo[] properties = typeof(T).GetProperties();

            List<(Argument, PropertyInfo)> argsAndProps = new();

            foreach (PropertyInfo property in properties) {
                Argument a = property.GetCustomAttributes<Argument>(false).FirstOrDefault(defaultValue: null);
                if (a != null) {
                    argsAndProps.Add((a, property));
                }
            }

            var tmp = argsAndProps.OrderBy(x => x.Item1.Order).ToList();
            argsAndProps = tmp;

            //pre kazdy, najdem value a naparsujem ju
            foreach ((Argument, PropertyInfo) argAndProp in argsAndProps) {
                GeneralType generalType = CheckType(argAndProp.Item2, out Type propType, out Type coreType);

                CheckNamesAndHelpText(argAndProp.Item1, commandReferenceBag);

                List<(string, int)> paramsAndIndices = FindArgumentParameters(ref command, argsDelimeterIndex, generalType == GeneralType.Array || generalType == GeneralType.List);

                if (paramsAndIndices.Count == 0) {
                    break;
                }

                //parsovat, ak to neviem sparsovat (ci uz list alebo iba simple) a argument je optional tak ho vynecham a skusim to naparsovat do dalsieho, inak vyhodim vynimku
                int indexOfExcParam = PopulateCommandInstance(ref commandInstance, argAndProp.Item2, generalType, coreType, new List<string>(paramsAndIndices.Select(x => x.Item1)), false);

                if (indexOfExcParam == 0 && argAndProp.Item1.IsRequired) {
                    throw new CommandParserException("Required argument " + argAndProp.Item1.Name + " is missing or can not be parsed.");
                }

                //zmazat vsetko pred naparsovanym a aj naparsovane a posuniem argsDelimeterIndex nizsie
                if (indexOfExcParam >= 0) {
                    command = command.Substring(paramsAndIndices[indexOfExcParam].Item2).Trim();
                    if (argsDelimeterIndex != int.MaxValue) {
                        argsDelimeterIndex -= paramsAndIndices[indexOfExcParam].Item2;
                    }
                }
                else if (generalType == GeneralType.Simple || generalType == GeneralType.Enum) {
                    command = command.Substring(paramsAndIndices[0].Item2 + paramsAndIndices[0].Item1.Length).Trim();
                    if (argsDelimeterIndex != int.MaxValue) {
                        argsDelimeterIndex -= paramsAndIndices[0].Item2 + +paramsAndIndices[0].Item1.Length;
                    }
                }
                else {
                    command = command.Substring(paramsAndIndices[paramsAndIndices.Count - 1].Item2 + paramsAndIndices[paramsAndIndices.Count - 1].Item1.Length).Trim();
                    if (argsDelimeterIndex != int.MaxValue) {
                        argsDelimeterIndex -= paramsAndIndices[paramsAndIndices.Count - 1].Item2 + +paramsAndIndices[paramsAndIndices.Count - 1].Item1.Length;
                    }
                }

                //IsPresent - iba ak naparsujem
                commandReferenceBag.ArgsPresence[argAndProp.Item1.Name] = true;
            }

            //skontrolovat isRequired
            foreach ((Argument, PropertyInfo) argAndProp in argsAndProps) {
                if (argAndProp.Item1.IsRequired && commandReferenceBag.ArgsPresence[argAndProp.Item1.Name] == false) {
                    throw new CommandParserException("Required argument " + argAndProp.Item1.Name + " is missing.");
                }
            }

            //delete remaining option names after last parsed arguments
            if (!string.IsNullOrEmpty(command)) {
                int currentPosition = 0;
                while (command[currentPosition] == '-' && currentPosition < argsDelimeterIndex) {
                    if (!command.Contains(' ') || command.IndexOf(' ', currentPosition + 1) == -1) {
                        currentPosition = command.Length;
                        break;
                    }
                    currentPosition = command.IndexOf(' ', currentPosition + 1) + 1;
                }

                command = command.Substring(currentPosition);
            }

            return commandInstance;
        }
    }
}