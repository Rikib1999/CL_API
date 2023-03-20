##First Impression

I liked the way options and arguments were defined, it was very intuitive and easy to understand. The formatting of the code was such that I had no problem figuring out what it does, which was a huge plus. However, I'd prefer a richer interface when defining them, especially with regards to constraints on option/argument values or their mutual dependencies and exclusivities. One thing I didn't understand was how is the user expected to pass variadic arguments to the program - I don't know what the expected delimiter is or whether there's a way to set one.

##API review

The library expects the user to write their own class for a command that contains fields for all the arguments or options. This allows the user to access those fields in a very intuitive fashion. The library allows the user to specify types for each argument or option. It also lets the user define options with multiple paramaters.

The library identifies plain arguments by their order as that is really the only way to identify a plain argument passed over the command line, but it allows the user to access them by the name name of their field in the command class, providing more user comfort when accessing the values. Arguments can expect arbitrary amounts of values if defined to do so. 

##Writing numactl

First part of writing numactl was fairly simple, defining options and arguments was implemented in an elegant easy to understand way. I have encountered some difficulties with implementing restrictions on the values passed to "-m", "-i", "-p and "-C", where I had to write my own setters - while that is on one hand a very powerful feature, on the other hand it feels like writing my own parser rather than using one.

I also missed a way to make arguments' optionality dependent on specified options - I wanted to make the Command argument required only if "-s" and "-H" were not specified, but I found no way of doing so. 

##Detailed comments:

The way the user accesses parsed values of single-parameter options looks very good, but it is unclear to me how would they declare and access an option with variable number of parameters. Furthermore I don't know how is the user expected to pass those parameters over the command line, i.e. how to separate parameters of one option from each other but also group them in a way the parser can tell they belong to that particular option.

There is no mention in the documentation of restricting the value of an option beyond defining its type - that is only possible by rewriting the setters. It is possible to set bounds for an integer plain argument, but not enumeration - I think it'd be useful to implement more easy to use features for common restrictions like that. I also find it odd that it is possible to set upper and lower bounds on an argument even if the argument is not of a number type.

I wonder how the library handles multiple optional plain arguments - it allows setting any argument to optional, but I can't think of a way to tell which argument is which if more than one is made optional. 


