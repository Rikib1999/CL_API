# Final Review
## First Impression

My first impression with the project was less than stellar. 

The very first thing I saw was a flood of warnings when I built the library, ran the example program and tried running tests.

The README.md is very servicable, I had no issues with it. It provided examples of how to use the library as well as an installation guide and highlight of the key concepts of the library.

I was happy to see, that the authors chose reflection to achieve their goals, because I've already seen similar APIs that also used reflection which helped me with orientation.

The code itself was hard to read because many if statements and loops didn't have brackets around their body and often times the body was written on the same line as the if/loop. Some lines couldn't even fit on my ultra-wide monitor, which should speak volumes by itself.
```cs
//some examples:
if (beginsWithName) command = command.Substring(command.IndexOf(" ") + 1);

if (option != null && !(generalType == GeneralType.Array || generalType == GeneralType.List) && option.MaxParameterCount > 1) throw new CommandParserException("Option " + option.Names[0] + " can not be parsed. Option supports multiple arguments but it is not a collection.");

if (paramsAndIndices.Count == 1 && !findMultiple) break;
    else { ... }
```
So I resulted to using a code linter to help me orient myself in the code. 

What also didn't help was the fact, that private methods were very sparsely documented. The few comments that were present were also partially in slovak, which didn't cause any problems to me, but could alienate some foreign programmers that stumble upon this code.

There was no CI implemented in the git repository, the `.gitlab-ci.yml` file was missing.

Lastly I didn't find any reference documentation, which made orientating in the project even harder.

## High-level review
In `README.md` the authors state, that the library is a simple way of parsing arguments on a command line and the library does just that. I had no problems with using the library for its intended purpose. While there were some hickups here and there, for example the library being unable to parse arguments written in a _argument=parameter_ format, but other than that the library supports every basic datatype including enumerable datatypes. 
## DateTime Implementation
The conversion to DateTime was already implemented by the library authors by using a generic converter, which made my work significantly easier. I thought about formatting the date using the library, but I couldn't think of any good use cases for it, so I ended up just converting it in the program itself.
## CI Implementation
As I previously mentioned there was no CI pipeline to be found in the project. I added new `.gitlab-ci.yml` with one stage that runs my DateTimeConverter program.
## Documentation
As I mentioned in my initial impressions, there is no reference documentation to be found. The documentation of public method is incomplete, oftentimes the comment for exceptions and paramaters of said methods is missing completely. Private methods are commented very sparsely, if at all, and the comments are inconsistent in the written language. The exceptions which get thrown have messages attached to them which is nice. 
## Implementation
In my opinion the code quality was subpar. As mentioned previously all ifs that had only one line of code were written on the same line which made it excruciatingly hard to read, I had to go out of my way to use a linter to help at least a little bit with the readibility. The code was rarely commented. There were also some questionable design decision. One of them was to always parse the received arguments as a string, even when an array of strings was received. The other was the usage of try-catch blocks. Many times in the code an exception gets thrown, caught within the library and ignored (either by returning a specific value, or by leaving the catch block empty all together). This isn't a one time only thing either, try-catch blocks are used extensively throughout the library and even nested in one another in some cases. Except for some rare instances the variables are named accordingly and can be understood clearly. Some methods are on the longer side (80-100 lines), though I admit they were prolonged a bit by the linter, still extracting some of the standalone loops into their seperate methods would help tremendously with readibility. In a couple of places there was an attempt at seperating code with regions, but it didn't help much in my opinion. Nullability of variables was almost never checked nor suppresed with the ! operator which made my IDE underline every other variable and made running the programs in a console spew out hundreds of lines of warnings. Early returns were implemented well and used often. The tests weren't implemented in the main library and some tests were failing, other than that the coverage seems to be good.
## Detailed comments
- in the ParseArguments method there is wrongly used LINQ method `.ToList()` which unnecessarily allocates the whole list on GC heap.
  - Probably not a huge problem, because the list will always be rather small, but it may impact the performace slightly.
- Using exceptions to carry some sort of information internally in the API doesn't seem like a good idea, especially when the exception isn't meant to be received by the end user.
- Parsing string instead of individual arguments from an array is a questionable decision that puts a lot of stress on the garbage collector and impacts the performace.
## Final thoughts
I didn't want to come off as purely negative. I'm sure the authors tried real hard and this was a great learning experience for them. Hopefully they'll be able to take something of use from this review.