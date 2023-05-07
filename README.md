# Introduction

This is a simple library for parsing command-line arguments written in .NET 7 

## installation

Example can be built and run with 

``` dotnet build ``` in the *t06-api-design* folder
``` dotnet run ``` in the folder with csproj of project which uses this library, or the *..\t06-api-design\ExampleProgram* subdirectory

or 

``` dotnet run --project ..\t06-api-design\ExampleProgram\ExampleProgram.csproj ```

Or use VisualStudio

To use the CommandLineParser library just include the project with 

``` using CommandLineParser; ```

## Usage

To create a command we need to create a class which inherits the ICommandDefinition interface

```
public class SimpleCommand : ICommandDefinition
```

To create a new option we create a property inside this class. The name of the property does not need to be the name of the option. To specify that the property is an option we use attribute Option. Its constructor has to at least have an array of names. 

example usage of an option : 

``` C#
[Option(names: new string[] { "-f", "--format" })]
    public string Format { get; set; }
```

Here we create option Format with names --format and -f 

To specify the number of its parameters we specify MinParameterCount and MaxParameterCount integral roperties of the option. To use a short option we set MaxParameterCount to 0. 

MinParameterCount has 0 value by default. To specify a long option, we change the MaxParameterCount to a desired value.

Therefore the difference between short and long option is that the short option has 0 parameters and long option has positive number of parameters. 

If we use a short option, it can be generic by any object. It just basically needs to be there.

We can also specify minimal amount of arguments by setting MinParameterCount to a desired value.

example : 

``` C#
[Option(names: new string[] { "-o", "--output" }
            ,HelpText = "Do not send the results to stderr, but overwrite the specified file."
            ,MinParameterCount = 1
            ,MaxParameterCount = 1
        )]
        public string Output { get; set; }
```

Here we have also added a HelpText message for the option which is used when we use option --help with the command. 

Similiarly

To create a plain argument we use Argument attribute on a property specified in the command class. However it is a plain argument so to distinguish between plain arguments we need to specify order the Argument via constructor. We can also specify if the argument is required or optional. We can set the HelpText in the same way as the helptext of an option. If the property is numerical we can set minimal and maximal desired value of this argument via UpperBound and LowerBound properties.

### example

``` C#
[Argument(order: 0, IsRequired = true)]
        public string Command { get; set; }
```

Here we have specified that this is the first plain argument named Command and it has string type.

To use multiple, possibly endless number of arguments we can set this argument to be of type List<> or an array

### example

``` C#
[Argument(order: 1, IsRequired = true)]
    public List<string> Arguments { get; set; }
```

To use this command we create an instance of the Command. We parse a string via : 

``` C#
SimpleCommand simpleCommand = new SimpleCommand();
            string commandLineInput = Console.ReadLine();
            timeCommand = CommandParser<SimpleCommand>.Parse(commandLineInput, simpleCommand);
```

### example

``` C#
[Boundaries<int>(lowerBound: 1, upperBound: 10)]
        [Option(names: new string[] { "-n", "--number" })]
        public int Number { get; set; }
```
In this example we have used attribute named Boundaries, which is generic by IComparible interface. We specify lower and upperBound properties of the attribute, based on which the property is checked for validity. This way we can test whether the command has right parameters. If it does not, the user is thrown an exception. 
It is up to user to define the property and the Boundaries with the same type. 

### example

``` C#
[Option(names: new string[] { "-r", "--random" }
        , Dependencies = new string[] { "--verbose" }
        , HelpText = "This is a random option"
        , Exclusivities = new string[] { "-n" })]
        public object Random { get; set; }
```
In this example we have added Dependencies and Exclustivites. As the names suggests here we specify other properties which in the case of Dependencies, the property Random depends on. This is again checked for when parsing the command. 

Exclustivities similliarly checks wheter Random Option is not used with any of the Options in Exclusivities array. Therefore if the parsed command contains --number and --random the user is thrown an exception. 

## Key concepts

We use attributes to specify if a property of our command class is an option or an argument.

Then we use static generic class CommandParser to parse the command line string. The class is generic by type of user defined command class which has to inherit ICommandDefinition interface. We then parse the command line string via Parse method. 

The command line is checked for undesired behaviour such as NullValue, the command line string not containing a command of our specified type via reflection.

We use reflection to parse the command line string input into arguments and options and the user of our api is then returned object with properties that corespond to the defined command class. Options and arguments come with a variety of properties for specifying various conditions, which are checked for when we call the Parse method. Based on this the user does not need to check for any condition, which he has specified in the attributes. 

It is done for the user. If the command does not meet the defined criteria, we throw an exception, which the user can catch and work with.

User therefore accesses the values via properties of the object.