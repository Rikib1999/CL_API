using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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
            typeof(IEnumerable<bool>),
            typeof(IEnumerable<byte>),
            typeof(IEnumerable<sbyte>),
            typeof(IEnumerable<char>),
            typeof(IEnumerable<string>),
            typeof(IEnumerable<int>),
            typeof(IEnumerable<uint>),
            typeof(IEnumerable<short>),
            typeof(IEnumerable<ushort>),
            typeof(IEnumerable<long>),
            typeof(IEnumerable<ulong>),
            typeof(IEnumerable<float>),
            typeof(IEnumerable<double>),
            typeof(IEnumerable<decimal>),
            typeof(IEnumerable<DateTime>),
            typeof(IEnumerable<Enum>)
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
        public static T Parse(string[] args, T commandInstance)
        {
            if (commandInstance == null) throw new NullReferenceException(nameof(commandInstance));
            if (!typeof(T).GetInterfaces().Contains(typeof(ICommandDefinition))) throw new MissingInterfaceException("Class " + typeof(T).Name + " does not implement ICommandDefinition interface.");
            if (args.Length < 1 || args is null) throw new ArgumentException("Command can not be null or empty.");

            ParseOptions(args[1..], commandInstance);
            ParseArguments(args, commandInstance);

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
        public static T Parse(string command, T commandInstance)
        {
            return Parse(command.Split(), commandInstance);
        }

        private static bool CheckIsEnummerable(PropertyInfo property, Type propType, Option option)
        {
            bool isEnumerable = property is IEnumerable && propType.IsGenericType;

            if (!isEnumerable && option.MaxParameterCount > 1)
            {
                throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");
            }
            return true;
        }

        private static void CheckNameValidity(Option option)
        {
            foreach (string name in option.Names)
            {
                if (string.IsNullOrEmpty(name) || name.Length <= 1 || !name.StartsWith('-')) throw new CommandParserException(name + " is not a valid name for an option.");
            }
        }

        private static void CheckIsTypeSupported(Option option, Type propType)
        {
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

                if (!isSupported)
                {
                    throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Type of option [" + propType.Name + "] is not supported.");
                }
            }
        }

        private static void CheckIsRequired(Option option, int indexOfOption)
        {
            if (indexOfOption == -1 && option.IsRequired)
            {
                throw new MissingRequiredOptionException("Missing required option " + option.Names[0]);
            }
        }

        private static void CheckExtremes(Option option, bool isEnummerable)
        {
            if (option.MinParameterCount < 0 || option.MaxParameterCount < 0)
            {
                throw new IncorrectExtremesException("Min and MaxParameterCount cannot be negative");
            }

            if (!isEnummerable && option.MinParameterCount > 1)
            {
                throw new IncorrectExtremesException("Non enummerable type cannot have MinParameterCount greater than 1");
            }
        }

        private static int FindIndexOfOption(string[] command, Option option)
        {
            foreach (var name in option.Names)
            {
                int indexOfOption = Array.IndexOf(command, name);
                if (indexOfOption != -1)
                {
                    return indexOfOption;
                }
            }
            return -1;
        }

        private static bool IsDifferentOption(string commandToken, Option option, IEnumerable<string> namesOfAllProperties)
        {
            if (namesOfAllProperties.Any(x => !option.Names.Contains(x) && commandToken == x))
            {
                return true;
            }
            return false;
        }

        private static MethodInfo GetParsingMethod(Type propType) =>
            propType.GetRuntimeMethod("TryParse", new Type[] { propType.MakeByRefType() });

        private static ConstructorInfo GetParsingConstructor(Type propType) => 
            propType
            .GetConstructors()
            .FirstOrDefault(x =>
                x.GetParameters().Length == 1
                && x.GetParameters()[0].ParameterType == typeof(string));

        private static object? AssignEnummerableParser(MethodInfo parseMethod, Option option, Type propType, string[] command, int indexOfOptionInCommand, ref int commandIndex, IEnumerable<string> propertyNames, Type internalType)
        {
            var enummerable = Activator.CreateInstance(propType);

            var addMethod = propType.GetMethod("Add");

            while (!IsDifferentOption(command[commandIndex], option, propertyNames))
            {
                if (commandIndex >= command.Length)
                {
                    break;
                }
                if (commandIndex >= option.MaxParameterCount + indexOfOptionInCommand + 1)
                {
                    break;
                }
                var valueOfProperty = Activator.CreateInstance(internalType);
                try
                {
                    parseMethod.Invoke(null, new object[] { valueOfProperty });
                }
                catch
                {
                    throw new InvalidValueException("Parsed value was incorrect");
                }
                addMethod.Invoke(null, new object[] { valueOfProperty });
            }

            return enummerable;
        }

        private static object? AssignEnnumerableWithConstructor(ConstructorInfo constructor, Option option, Type propType, string[] command, int indexOfOptionInCommand, ref int commandIndex, IEnumerable<string> propertyNames, Type internalType)
        {
            var enummerable = Activator.CreateInstance(propType);

            var addMethod = propType.GetMethod("Add");

            while (!IsDifferentOption(command[commandIndex], option, propertyNames))
            {
                if (commandIndex >= command.Length)
                {
                    break;
                }
                if (commandIndex >= option.MaxParameterCount + indexOfOptionInCommand + 1)
                {
                    break;
                }
                var valueOfProperty = Activator.CreateInstance(internalType);
                if (!(bool)constructor.Invoke(new object[] { valueOfProperty }))
                {
                    throw new InvalidValueException("Parsed value was incorrect");
                }
                addMethod.Invoke(null, new object[] { valueOfProperty });
            }

            return enummerable;
        }

        private static object? AssignValue(bool isEnummerable, Option option, Type propType, string[] command, int indexOfOptionInCommand, ref int commandIndex, IEnumerable<string> propertyNames, Type internalType, PropertyInfo property)
        {
            MethodInfo parsingMethod = GetParsingMethod(internalType);

            ConstructorInfo constructorMethod = null;

            if (parsingMethod == null)
            { 
                constructorMethod = GetParsingConstructor(internalType);

                if (constructorMethod == null)
                { 
                    throw new InvalidPropertyTypeException($"{option.Names.FirstOrDefault()} can not be parsed.");
                }
            }

            Type boundariesType = typeof(Boundaries<>).MakeGenericType(internalType);

            var boundaries = Activator.CreateInstance(boundariesType);

            boundaries = property
                .GetCustomAttributes()
                .FirstOrDefault(x => x.GetType() == boundariesType.GetType());

            var valueOfProperty = Activator.CreateInstance(internalType);

            commandIndex++;

            if (parsingMethod != null)
            {
                
                parsingMethod.Invoke(null, new object[] { command[commandIndex], valueOfProperty });
            }
            else
            {
                valueOfProperty = constructorMethod?.Invoke(new object[] { command[commandIndex] });
            }

            if (isEnummerable)
            {
                if (parsingMethod != null)
                {
                    valueOfProperty = AssignEnnumerableWithConstructor(constructorMethod, option, propType, command, indexOfOptionInCommand, ref commandIndex, propertyNames, internalType);
                }
                else
                {
                    valueOfProperty = AssignEnummerableParser(parsingMethod, option, propType, command, indexOfOptionInCommand, ref commandIndex, propertyNames, internalType);
                }
            }

            return valueOfProperty;
        }

        private static T ParseOptions(string [] command, T commandInstance)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            var propertyNames = properties
                .Select(x => x.GetCustomAttributes<Option>(false))
                .FirstOrDefault(defaultValue: null)
                .SelectMany(x => x.Names);

            foreach (PropertyInfo property in properties)
            {
                Option option = property.GetCustomAttributes<Option>(false).FirstOrDefault(defaultValue: null);

                if (option == null) continue;

                Type propType = property.PropertyType;

                bool isEnummerable = CheckIsEnummerable(property, propType, option);

                Type internalType = isEnummerable ? propType.GenericTypeArguments[0] : propType;

                CheckNameValidity(option);

                var indexOfOptionInCommand = FindIndexOfOption(command, option);

                //If the property is missing in the command than it stays null

                CheckIsRequired(option, indexOfOptionInCommand);

                CheckExtremes(option, isEnummerable);

                int commandIndex = indexOfOptionInCommand;

                var valueOfProperty = AssignValue(isEnummerable, option, propType, command, indexOfOptionInCommand, ref commandIndex, propertyNames, internalType, property);
                
                //delete option arguments, so parsing plain arguments will be easier, just skipping - or --
            }

            return commandInstance;
        }

        private static T ParseArguments(string [] command, T commandInstance)
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