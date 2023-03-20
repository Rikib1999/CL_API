# First impressions

## Readme

First of all, to all my knowledge this markdown file uses invalid syntax for headings, also if you want
to put a block of code in specific language, you should use highlighting for that language. For example
instead of this:

```
[Option(names: new string[] { "-o", "--output" }
            ,HelpText = "Do not send the results to stderr, but overwrite the specified file."
            ,MinParameterCount = 1
            ,MaxParameterCount = 1
        )]
        public string Output { get; set; }
```

Use it like this:
```C#
[Option(names: new string[] { "-o", "--output" }
            ,HelpText = "Do not send the results to stderr, but overwrite the specified file."
            ,MinParameterCount = 1
            ,MaxParameterCount = 1
        )]
        public string Output { get; set; }
```

I was pleasantly surprised that the options are chained to the Command, which enables user to parse
multiple commands and it just makes sense (we, in our team, did not think of it this way)!

Installation was really poorly described.

### Usage section

A lot of the times I was really confused of what is going on. You start this section with creation of
class implementing some Interface, but the interface itself is not even mentioned in the whole readme (except for one
line in the last paragraph). Then there was example how to create simple option on the command which
gave me an idea about the whole concept of the library which was nice. Then there was just another example
how to create more complex option, but I was missing some summary of what I can set up in that option?
It was just shown in the example but how am I supposed to know of what is this capable based on 5 line example?
Also you were mentioning something about long and short options but I don't see difference in the usage from the 
example? You have both long and short synonyms used there? 

Then there was part about plain arguments which was just horribly described, basically all the issues
from the previous examples were copied there. Again no summary of what to expect and so on.

Also there are some parts which cannot even compile?!

## Key concepts section
Section about Key concepts introduced just some key concepts, but overall did not help me to understand
what kind of steps to take if I want to use this library or for example extend it. One positive thing was that this
section was brief.

Generating documentation was not even mentioned in the whole readme.

## Summary of first expressions

I was really disappointed with this readme and I could not understand a lot of concepts. Also the tutorials
did not give explanations on a lot of things such as:
- Where do I find the parsed parameters?
- How do I get the help text?
- When are some exceptions thrown?
- How to extend this library?
- How do I even know the option was present on command line?

# API review

I liked that the library uses attributes and reflection to declare commands and options chained to these
commands. The Idea is really great because it saves a lot of time for the user, but the implementation and mostly
the documentation did not match the greatness of the idea. The responsibilities of user are (as much as I get it)
create class for command implementing some blank Interface `ICommandDefinition` and then create properties which
should represent particular options and plain arguments.

However a lot of informations was missing, some of which:
- What is the actual type of the option? (int, string, bool, enum)
- What types of options are supported?
- What are all the properties you can set to the Option attribute?

I could not find these informations anywhere basically.

Basically everything this library enables you to do is to define some option, and after the parsing,
maybe this property value changes? You can't even find if the option is present on the command line.
I would say that in the current state it is harder to use this library than to parse it yourself, and 
even if someone decides to use this library, he can basically do nothing with it.

# My experience with using this library

First I had to detect if there were any options present on the command line at all. This library
does not enable user to even get the information whether specific option is present on command line, 
so I had to do the parsing myself again. Maybe it is intended to set all the option properties to null
and see if they changes? I do not know, it is really hard to guess.

Then I had to find out whether specific options were present on the command line, but again i could
not do it with the library because it does now implement such mechanism (or if it does it is not mentioned anywhere and
I personally did not find it anywhere).

Last thing I had to use some options which required parameters and even the plain argument was required,
when these options were present. Library does not allow user to specify which options are conflicting.
Also maybe the authors should take into consideration adding a way to create option, which accepts
string matching some regex and parse it accordingly (I did not think of this myself so it is not such
a big problem I would say). Then I had to check the conflicts myself, validate the values again and so on,
so basically it was really annoying.

I had terrible experience using this library and the fact that I had to use it just slowed me down.

# Problems to tackle

In this section I will state some of the biggest problems of the library, but basically everything
has to do something with the documentation:

- documentation -> The whole library contains about 70 lines of comments and I find it really insufficient.
Most of the times I was just guessing what should do what. Exceptions are not documented at all (one sentence is not
documentation to my knowledge). Also other things which I have already mentioned in API review. I generated
the documentation with doxygen but it did not contain any useful information...

- Accessibility of parsed parameters -> when I create option which takes 5 arguments, where will i find them?
How do I specify type of these parameters? I literally just do knot know. This is big problem, because when I tried
to use this library I was purely just guessing what should I do. Should I in these multiple parameter options create
the Option property as an array or list?

- Presence of option on the command line-> if the option is present on the command line, the user will not know it
and cannot act on it either (because how?). If somehow I missed that it is indeed possible, maybe it is because
it is not mentioned anywhere.

- I personally would like to have the knowledge of what I can specify in the option. Some summary,
not just in the examples and even there are some things missing.

- Accessibility of help text-> where can I find this? Do I need to access it with the reflection on each
of the option? Because if yes, that would be really hard and impractical to use.

- `public static T Parse(string command, T commandInstance)` -> parameter command is not explained at all,
 and it's name is completely misleading, I think it should contain the whole input string from the command line,
but in the C# this command line arguments are passed as an array so I do not get this decision either. When I used it
I had to create a string from the already existing array `args` of the Main method. Also there is already prepared
space to explain when and where the exceptions occur, but the author decided that maybe it is self-explanatory?

- Also there are no mentions about the expandability-> of what types can be options? Can I create
my own struct? How would such thing be parsed? Is it even possible? These questions I think should either be clear
from the implementation or be mentioned in the documentation.

- Example program-> the authors just created some options and then ran the parser. I am missing some usage after the parsing
is complete so I can see how do they access the parsed arguments, how do they use help text. Maybe add some example outputs
in the comments.

- Personally, I would add instructions on how to generate the documentation from the source code. This was even in the
assignment specification.

- Positive thing is that the user can create multiple command classes and it's chained options and can parse them separatly,
I. e. there is no global state. I just assume this because it is not mentioned anywhere.

- API is really prone to user's mistakes due to the lack of documentation...

